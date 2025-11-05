"use client";


import { Button } from "@/components/ui/button";
import { Card, CardAction, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { CreateProductDialog } from "@/features/products/admin/components/CreateProductDialog";
import { DeleteProductDialog } from "@/features/products/admin/components/DeleteProductDialog";
import { EditProductDialog } from "@/features/products/admin/components/EditProductDialog";
import Pagination from "@/features/common/Pagination";

import { useProducts } from "@/features/products/hooks";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";

export default function ProductsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const [query, setQuery] = useState(searchParams.get("query") || "");
  const [categoryId, setCategoryId] = useState(searchParams.get("categoryId") || "");
  const [minPrice, setMinPrice] = useState(searchParams.get("minPrice") || "");
  const [maxPrice, setMaxPrice] = useState(searchParams.get("maxPrice") || "");
  const [page, setPage] = useState(Number(searchParams.get("page")) || 1);
  const [pageSize , setPageSize ] = useState(Number(searchParams.get("pageSize ")) || 10);

  const [debouncedQuery, setDebouncedQuery] = useState(query);
  useEffect(() => {
    const timeout = setTimeout(() => setDebouncedQuery(query), 1000);
    return () => clearTimeout(timeout);
  }, [query]);

  useEffect(() => {
    const params = new URLSearchParams({
      page: page.toString(),
      query: query || "",
      categoryId: categoryId || "",
      minPrice: minPrice || "",
      maxPrice: maxPrice || "",
    });
    router.replace(`products?${params.toString()}`);
  }, [page, debouncedQuery, categoryId, minPrice, maxPrice]);

  const { data, isLoading, isError } = useProducts({
    page,
    pageSize,
    keyword: debouncedQuery,
    categoryId,
    minPrice: minPrice ? Number(minPrice) : undefined,
    maxPrice: maxPrice ? Number(maxPrice) : undefined,
  });

  if (isError)
    return <div className="p-8 text-red-500">Lỗi tải danh sách sản phẩm</div>;

  // Giả sử API trả về data.total (tổng số sản phẩm)
  const total = data?.total ?? 0;
  const products = Array.isArray(data) ? data : data?.items ?? [];
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  return (
    <main className="p-6 space-y-6">
      <Card className="p-0">
        <CardHeader className="flex items-center justify-between p-4">
              <div>
                <CardTitle className="text-lg">Danh sách sản phẩm</CardTitle>
                <div className="text-sm text-muted-foreground">Tổng: {total} sản phẩm</div>
              </div>
              <CardAction>
                <div className="flex items-center gap-2">
                  <Button variant="outline" size="sm" onClick={() => { setQuery(""); setMinPrice(""); setMaxPrice(""); setPage(1); }}>
                    Đặt lại
                  </Button>
                  <CreateProductDialog />
                </div>
              </CardAction>
            </CardHeader>

            <CardContent className="p-4">
              <div className="flex flex-col md:flex-row gap-3 md:items-center md:justify-between mb-4">
                <div className="flex flex-wrap items-center gap-2">
                  <Input
                    placeholder="Tìm theo tên hoặc SKU..."
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    className="max-w-xs"
                  />
                  <Input
                    placeholder="Giá từ"
                    type="number"
                    value={minPrice}
                    onChange={(e) => setMinPrice(e.target.value)}
                    className="w-28"
                  />
                  <Input
                    placeholder="Đến"
                    type="number"
                    value={maxPrice}
                    onChange={(e) => setMaxPrice(e.target.value)}
                    className="w-28"
                  />
                </div>
                <div className="text-sm text-muted-foreground">Trang {page} / {totalPages}</div>
              </div>

              {isLoading ? (
                <div className="space-y-3">
                  {Array.from({ length: 5 }).map((_, i) => (
                    <Skeleton key={i} className="h-8 w-full" />
                  ))}
                </div>
              ) : (
                <>
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Ảnh</TableHead>
                        <TableHead>Tên</TableHead>
                        <TableHead>SKU</TableHead>
                        <TableHead>Giá</TableHead>
                        <TableHead>Trạng thái</TableHead>
                        <TableHead>Ngày tạo</TableHead>
                        <TableHead className="text-right">Hành động</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {products.map((p: any) => (
                        <TableRow key={p.id}>
                          <TableCell className="w-16 p-2">
                            {(() => {
                              const src = p.mainImage?.url || "/placeholder.png";
                              return (
                                <img
                                  src={src}
                                  alt={p.name || "product"}
                                  className="h-12 w-12 rounded object-cover border"
                                  onError={(e) => {
                                    (e.target as HTMLImageElement).src = "/placeholder.png";
                                  }}
                                />
                              );
                            })()}
                          </TableCell>
                          <TableCell>{p.name}</TableCell>
                          <TableCell className="text-sm text-muted-foreground">{p.sku}</TableCell>
                          <TableCell>{p.price.toLocaleString("vi-VN")} {p.currency}</TableCell>
                          <TableCell>
                            <span className={`px-2 py-1 text-xs rounded ${p.isActive ? "bg-green-100 text-green-700" : "bg-gray-200 text-gray-600"}`}>
                              {p.isActive ? "Active" : "Inactive"}
                            </span>
                          </TableCell>
                          <TableCell className="text-sm">{new Date(p.createdAtUtc).toLocaleDateString("vi-VN")}</TableCell>
                          <TableCell className="text-right">
                            <EditProductDialog productId={p.id} />
                            <DeleteProductDialog productId={p.id} />
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>

                  <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                </>
              )}
            </CardContent>

            <CardFooter className="justify-end p-4">
              <div className="text-sm text-muted-foreground">Hiển thị {products.length} trên {total} sản phẩm</div>
            </CardFooter>
          </Card>
        </main>
      );
    }

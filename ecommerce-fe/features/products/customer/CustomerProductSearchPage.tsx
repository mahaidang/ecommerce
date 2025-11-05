"use client";
import { useEffect, useState } from "react";
import { Search } from "lucide-react";
import CustomerProductList from "./CustomerProductList";
import { useRouter, useSearchParams } from "next/navigation";
import { useProducts } from "../hooks";
import Pagination from "../../common/Pagination";

export default function CustomerProductSearchPage() {
    const router = useRouter();
    const searchParams = useSearchParams();

    const [showCategory, setShowCategory] = useState(false);
    const [query, setQuery] = useState(searchParams.get("query") || "");
    const [minPrice, setMinPrice] = useState(searchParams.get("minPrice") || "");
    const [maxPrice, setMaxPrice] = useState(searchParams.get("maxPrice") || "");

    // State tạm cho input
    const [searchInput, setSearchInput] = useState(query);
    const [minPriceInput, setMinPriceInput] = useState(minPrice);
    const [maxPriceInput, setMaxPriceInput] = useState(maxPrice);
    const [categoryId, setCategoryId] = useState(searchParams.get("categoryId") || "");
    const [page, setPage] = useState(Number(searchParams.get("page")) || 1);
    const [pageSize] = useState(Number(searchParams.get("pageSize ")) || 8);


    // Khi search/filter, cập nhật state thực tế
    const handleSearch = () => {
        setQuery(searchInput);
        setMinPrice(minPriceInput);
        setMaxPrice(maxPriceInput);
        setPage(1);
    };


    const { data, isLoading, isError } = useProducts({
        page,
        pageSize,
        keyword: query,
        categoryId,
        minPrice: minPrice ? Number(minPrice) : undefined,
        maxPrice: maxPrice ? Number(maxPrice) : undefined,
    });

    if (isError)
        return <div className="p-8 text-red-500">Lỗi tải danh sách sản phẩm</div>;


    return (
    <div className="max-w-6xl mx-auto px-4 py-8 bg-white dark:bg-neutral-900 min-h-screen">
            {/* Search Bar & Price Filter */}
            <div className="flex items-center gap-3 mb-6">
                <button
                    className="px-4 py-2 bg-gray-200 dark:bg-neutral-800 text-gray-700 dark:text-gray-100 rounded-lg hover:bg-gray-300 dark:hover:bg-neutral-700 transition-colors duration-200 font-medium whitespace-nowrap"
                    onClick={() => setShowCategory((v) => !v)}
                >
                    Danh mục
                </button>
                <div className="relative flex-1">
                    <input
                        className="w-full border border-gray-300 dark:border-neutral-700 bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 rounded-lg pl-4 pr-10 py-2 outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-shadow"
                        placeholder="Tìm kiếm sản phẩm..."
                        value={searchInput}
                        onChange={(e) => setSearchInput(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === "Enter") handleSearch();
                        }}
                    />
                    <button
                        className="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 text-gray-500 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                        onClick={handleSearch}
                        type="button"
                    >
                        <Search size={20} />
                    </button>
                </div>
                <input
                    type="number"
                    className="w-28 border border-gray-300 dark:border-neutral-700 bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 rounded-lg px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-shadow"
                    placeholder="Giá min"
                    value={minPriceInput}
                    onChange={(e) => setMinPriceInput(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === "Enter") handleSearch();
                    }}
                />
                <span className="text-gray-400 dark:text-gray-300">-</span>
                <input
                    type="number"
                    className="w-28 border border-gray-300 dark:border-neutral-700 bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 rounded-lg px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-shadow"
                    placeholder="Giá max"
                    value={maxPriceInput}
                    onChange={(e) => setMaxPriceInput(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === "Enter") handleSearch();
                    }}
                />
            </div>

            {/* Category Panel */}
            {showCategory && (
                <div className="mb-6 p-6 bg-gray-50 dark:bg-neutral-800 border border-gray-200 dark:border-neutral-700 rounded-lg shadow-sm">
                    {/* TODO: Hiển thị danh sách category ở đây */}
                    <p className="text-gray-600 dark:text-gray-200">Danh sách danh mục (category)...</p>
                </div>
            )}

            {/* Product List */}
            <CustomerProductList data={data} />
            <Pagination
                page={page}
                totalPages={Math.ceil(Number(data?.total) / pageSize)}
                onPageChange={setPage}
            />
        </div>
    );
}
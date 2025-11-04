import { useSaveItem } from "@/features/basket/hooks";
import { getUserIdFromToken } from "@/lib/auth";
import { getOrCreateSessionId } from "@/lib/session";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";

type CustomerProductListProps = {
  data?: any;
};


const CustomerProductList = ({ data }: CustomerProductListProps) => {

    const router = useRouter();
    const searchParams = useSearchParams();
        const saveItem = useSaveItem();
        const userId = getUserIdFromToken();
        const sessionId = getOrCreateSessionId();
        const [showSuccess, setShowSuccess] = useState(false);

        useEffect(() => {
            if (saveItem.isSuccess) {
                setShowSuccess(true);
                const timer = setTimeout(() => setShowSuccess(false), 2000);
                return () => clearTimeout(timer);
            }
        }, [saveItem.isSuccess]);


    const formatPrice = (price: number) => {
        return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
        }).format(price);
    };

    return (
        <div className="bg-white dark:bg-neutral-900 rounded-lg relative">
            {/* Hiển thị thông báo thêm thành công */}
            {showSuccess && (
                <div className="fixed top-6 left-1/2 -translate-x-1/2 z-50 px-6 py-3 bg-green-600 text-white rounded shadow-lg animate-fade-in-out">
                    Đã thêm vào giỏ hàng!
                </div>
            )}
            <h1 className="text-2xl font-bold mb-6 text-gray-900 dark:text-gray-100">Danh sách sản phẩm</h1>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-5">
                {data?.items.map((product: any) => (
                    <div
                        key={product.id}
                        className="border border-gray-200 dark:border-neutral-700 rounded-lg overflow-hidden bg-white dark:bg-neutral-800 shadow-sm hover:shadow-md transition-shadow duration-200 cursor-pointer"
                        onClick={() => router.push(`/customer/products/${product.id}`)}
                    >
                        {/* Product Image */}
                        <div className="relative aspect-square bg-gray-100 dark:bg-neutral-700 p-2 overflow-hidden group">
                            {(() => {
                                const src = product.mainImage?.url || "/placeholder.png";
                                return (
                                    <img
                                        src={src}
                                        alt={product.name || "product"}
                                        className="w-full h-full object-cover border border-gray-200 dark:border-neutral-700 transition-transform duration-300 group-hover:scale-110"
                                        onError={(e) => {
                                            (e.target as HTMLImageElement).src = "/placeholder.png";
                                        }}
                                    />
                                );
                            })()}
                        </div>

                        {/* Product Info */}
                        <div className="p-4">
                            <h3 className="font-medium text-gray-800 dark:text-gray-100 mb-2 line-clamp-2 min-h-[3rem]">
                                {product.name}
                            </h3>
                            <div className="flex items-center justify-between">
                                <p className="text-lg font-bold text-blue-600 dark:text-blue-400">
                                    {formatPrice(product.price)}
                                </p>
                                {/* Add to Cart Button */}
                                <button
                                    className="bg-blue-600 dark:bg-blue-500 text-white p-2 rounded-full shadow-md hover:bg-red-700 dark:hover:bg-red-600 transition-colors duration-200"
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        saveItem.mutate({
                                            userId: userId ?? undefined,
                                            sessionId,
                                            productId: product.id,
                                            quantity: 1,
                                        });
                                    }}
                                    aria-label="Thêm vào giỏ hàng"
                                >
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        className="h-5 w-5"
                                        fill="none"
                                        viewBox="0 0 24 24"
                                        stroke="currentColor"
                                    >
                                        <path
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                            strokeWidth={2}
                                            d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z"
                                        />
                                    </svg>
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default CustomerProductList;
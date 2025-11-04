"use client";
import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useProductFull } from "../hooks"
import { useParams } from "next/navigation";
import { BackButton } from "@/components/ui/BackButton";


const formatPrice = (price: number) =>
    new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);


const COLOR_OPTIONS = [
    { name: "Đỏ", value: "red", class: "bg-red-500" },
    { name: "Xanh", value: "blue", class: "bg-blue-500" },
    { name: "Đen", value: "black", class: "bg-black" },
    { name: "Trắng", value: "white", class: "bg-white border border-gray-300" },
];

const CustomerProductDetail = () => {
    const params = useParams();
    const productId = params?.id as string;
    const product = useProductFull(productId);
    const [mainImage, setMainImage] = useState("/placeholder.png");
    // Đã loại bỏ state cho modal, biến thể, số lượng

    useEffect(() => {
        if (product.data?.images?.[0]?.url) {
            setMainImage(product.data.images[0].url);
        }
    }, [product.data?.images]);


    // 👉 state cho carousel
    const [startIndex, setStartIndex] = useState(0);
    const visibleImages = product.data?.images?.slice(startIndex, startIndex + 3);

    const next = () =>
        setStartIndex((prev) =>
            Math.min(prev + 3, (product.data?.images?.length ?? 0) - 3)
        );
    const prev = () =>
        setStartIndex((prev) =>
            Math.max(prev - 3, 0)
        );

    // Thêm vào giỏ hàng trực tiếp
    const handleAddToCart = () => {
        if (!product.data) return;
        const basket = JSON.parse(localStorage.getItem("basket") || "[]");
        if (!basket.find((item: any) => item.id === productId)) {
            basket.push({
                id: productId,
                name: product.data.name,
                price: product.data.price,
                imageUrl: product.data.images?.[0]?.url || "/placeholder.png",
            });
            localStorage.setItem("basket", JSON.stringify(basket));
        }
        alert("Đã thêm vào giỏ hàng!");
    };

    // Mua ngay: lưu id vào localStorage và chuyển sang checkout
    const handleBuyNow = () => {
        if (!product.data) return;
        localStorage.setItem("checkout_selected", JSON.stringify([productId]));
        const basket = JSON.parse(localStorage.getItem("basket") || "[]");
        if (!basket.find((item: any) => item.id === productId)) {
            basket.push({
                id: productId,
                name: product.data.name,
                price: product.data.price,
                imageUrl: product.data.images?.[0]?.url || "/placeholder.png",
            });
            localStorage.setItem("basket", JSON.stringify(basket));
        }
        router.push("/customer/checkout");
    };

    const router = useRouter();
    // Đã loại bỏ handleConfirm và các biến liên quan modal/biến thể/số lượng
    if (product.isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 dark:from-neutral-950 dark:to-neutral-900">
                <div className="text-center">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-4 border-blue-500 border-t-transparent"></div>
                    <p className="mt-4 text-gray-600 dark:text-gray-400">Đang tải sản phẩm...</p>
                </div>
            </div>
        );
    }

    if (!product.data) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 dark:from-neutral-950 dark:to-neutral-900">
                <div className="text-center text-gray-500 dark:text-gray-400">
                    <svg className="w-24 h-24 mx-auto mb-4 text-gray-300 dark:text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                    </svg>
                    <p className="text-lg">Không tìm thấy sản phẩm</p>
                </div>
            </div>
        );
    }

    const p = product.data;

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50/30 to-gray-100 dark:from-neutral-950 dark:via-blue-950/20 dark:to-neutral-900">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <BackButton fallbackHref="/products" className="mb-6 inline-flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors" />

                <div className="bg-white/80 dark:bg-neutral-900/80 backdrop-blur-xl rounded-2xl shadow-2xl overflow-hidden border border-gray-200/50 dark:border-neutral-800/50">
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 p-8 lg:p-12">
                        {/* 👉 Image carousel */}
                        <div className="space-y-6">
                            {/* Ảnh chính */}
                            <div className="relative group">
                                <div className="absolute inset-0 bg-gradient-to-br from-blue-500/20 to-purple-500/20 dark:from-blue-600/20 dark:to-purple-600/20 rounded-2xl blur-2xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
                                <div className="relative bg-gradient-to-br from-gray-100 to-gray-50 dark:from-neutral-800 dark:to-neutral-900 rounded-2xl p-8 border border-gray-200 dark:border-neutral-700 overflow-hidden">
                                    <img
                                        src={mainImage}
                                        alt={p.name}
                                        className="w-full h-96 object-contain rounded-xl transition-transform duration-500 group-hover:scale-105"
                                    />
                                </div>
                            </div>

                            {/* Carousel thumbnails */}
                            <div className="flex items-center justify-center gap-3">
                                <button
                                    onClick={prev}
                                    disabled={startIndex === 0}
                                    className="w-10 h-10 flex items-center justify-center rounded-full bg-white dark:bg-neutral-800 border border-gray-300 dark:border-neutral-700 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-neutral-700 hover:border-blue-400 dark:hover:border-blue-500 disabled:opacity-30 disabled:cursor-not-allowed transition-all shadow-sm"
                                >
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                                    </svg>
                                </button>
                                <div className="flex gap-3">
                                    {visibleImages?.map((img, i) => (
                                        <button
                                            key={i}
                                            onClick={() => setMainImage(img.url)}
                                            className={`w-20 h-20 rounded-xl overflow-hidden border-2 transition-all hover:scale-105 hover:shadow-lg ${
                                                mainImage === img.url 
                                                    ? 'border-blue-500 dark:border-blue-400 shadow-lg shadow-blue-500/50' 
                                                    : 'border-gray-200 dark:border-neutral-700'
                                            }`}
                                        >
                                            <img
                                                src={img.url}
                                                alt={img.alt}
                                                className="w-full h-full object-cover bg-gray-100 dark:bg-neutral-800"
                                            />
                                        </button>
                                    ))}
                                </div>
                                <button
                                    onClick={next}
                                    disabled={startIndex + 3 >= (product.data?.images?.length ?? 0)}
                                    className="w-10 h-10 flex items-center justify-center rounded-full bg-white dark:bg-neutral-800 border border-gray-300 dark:border-neutral-700 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-neutral-700 hover:border-blue-400 dark:hover:border-blue-500 disabled:opacity-30 disabled:cursor-not-allowed transition-all shadow-sm"
                                >
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                                    </svg>
                                </button>
                            </div>
                        </div>

                        {/* Product info */}
                        <div className="flex flex-col justify-between space-y-6">
                            <div className="space-y-6">
                                <div>
                                    <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 dark:text-gray-100 mb-3 leading-tight">
                                        {p.name}
                                    </h1>
                                    <div className="flex items-center gap-3 mb-4">
                                        <span className="px-3 py-1 rounded-full text-xs font-semibold bg-gray-100 dark:bg-neutral-800 text-gray-600 dark:text-gray-400">
                                            {p.sku}
                                        </span>
                                        <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                                            p.isActive 
                                                ? 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400' 
                                                : 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400'
                                        }`}>
                                            {p.isActive ? "✓ Còn hàng" : "✕ Hết hàng"}
                                        </span>
                                    </div>
                                </div>

                                <div className="relative">
                                    <div className="absolute -left-4 -top-2 w-24 h-24 bg-blue-500/10 dark:bg-blue-500/20 rounded-full blur-2xl"></div>
                                    <div className="relative text-4xl lg:text-5xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 dark:from-blue-400 dark:to-purple-400 bg-clip-text text-transparent">
                                        {formatPrice(p.price ?? 0)}
                                    </div>
                                </div>

                                <div className="prose prose-gray dark:prose-invert max-w-none">
                                    <p className="text-gray-600 dark:text-gray-300 leading-relaxed">
                                        {p.description}
                                    </p>
                                </div>
                            </div>

                            <div className="space-y-4 pt-6 border-t border-gray-200 dark:border-neutral-800">
                                <div className="grid grid-cols-2 gap-4">
                                    <button
                                        className="group relative px-8 py-4 bg-gradient-to-r from-blue-600 to-blue-700 dark:from-blue-500 dark:to-blue-600 text-white rounded-xl font-semibold hover:from-blue-700 hover:to-blue-800 dark:hover:from-blue-600 dark:hover:to-blue-700 transition-all shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 hover:-translate-y-0.5 active:translate-y-0"
                                        onClick={handleAddToCart}
                                    >
                                        <span className="flex items-center justify-center gap-2">
                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                                            </svg>
                                            Thêm vào giỏ
                                        </span>
                                    </button>
                                    <button
                                        className="group relative px-8 py-4 bg-gradient-to-r from-red-600 to-red-700 dark:from-red-500 dark:to-red-600 text-white rounded-xl font-semibold hover:from-red-700 hover:to-red-800 dark:hover:from-red-600 dark:hover:to-red-700 transition-all shadow-lg shadow-red-500/30 hover:shadow-xl hover:shadow-red-500/40 hover:-translate-y-0.5 active:translate-y-0"
                                        onClick={handleBuyNow}
                                    >
                                        <span className="flex items-center justify-center gap-2">
                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                            </svg>
                                            Mua ngay
                                        </span>
                                    </button>
                                </div>
                                <p className="text-xs text-center text-gray-500 dark:text-gray-500">
                                    Miễn phí vận chuyển cho đơn hàng trên 500.000đ
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Đã loại bỏ modal chọn biến thể và số lượng */}
        </div>
    );
};
export default CustomerProductDetail;
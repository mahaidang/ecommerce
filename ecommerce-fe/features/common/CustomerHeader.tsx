"use client";
import Link from "next/link";
import { useAuth } from "../auth/useAuth";
import { useTheme } from "next-themes";
import { Moon, Sun, ShoppingBag, FileText, Package, User, LogOut, ShoppingCart } from "lucide-react";

const CustomerHeader = () => {
    const { logout } = useAuth();
    const { theme, setTheme } = useTheme();

    return (
        <header className="sticky top-0 z-50 w-full border-b bg-white/80 dark:bg-gray-950/80 backdrop-blur-md supports-[backdrop-filter]:bg-white/60 dark:supports-[backdrop-filter]:bg-gray-950/60">
            <div className="container mx-auto px-6 py-4">
                <div className="flex items-center justify-between">
                    {/* Logo & Navigation */}
                    <div className="flex items-center gap-8">
                        <Link
                            href="/customer/products"
                            className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 dark:from-blue-400 dark:to-purple-400 bg-clip-text text-transparent hover:opacity-80 transition-opacity"
                        >
                            E-Shop
                        </Link>

                        <nav className="hidden md:flex items-center gap-1">
                            <Link
                                href="/customer/orders"
                                className="flex items-center gap-2 px-4 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-950/30 font-medium transition-all duration-200"
                            >
                                <Package size={18} />
                                <span>Đơn hàng</span>
                            </Link>

                            <Link
                                href="/customer/basket"
                                className="flex items-center gap-2 px-4 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-950/30 font-medium transition-all duration-200"
                            >
                                <ShoppingCart size={18} />
                                <span>Giỏ hàng</span>
                            </Link>

                            <Link
                                href="/customer/products"
                                className="flex items-center gap-2 px-4 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-950/30 font-medium transition-all duration-200"
                            >
                                <ShoppingBag size={18} />
                                <span>Mua sắm</span>
                            </Link>
                        </nav>
                    </div>

                    {/* User Actions */}
                    <div className="flex items-center gap-2">
                        <Link
                            href="/customer/profile"
                            className="flex items-center gap-2 px-4 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-gray-100 dark:hover:bg-gray-800 font-medium transition-all duration-200"
                        >
                            <User size={18} />
                            <span className="hidden sm:inline">Trang cá nhân</span>
                        </Link>

                        <button
                            onClick={() => setTheme(theme === "light" ? "dark" : "light")}
                            className="p-2.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-700 dark:text-gray-300 transition-all duration-200"
                            aria-label="Toggle theme"
                        >
                            {theme === "light" ? <Moon size={20} /> : <Sun size={20} />}
                        </button>

                        <button
                            onClick={logout}
                            className="flex items-center gap-2 px-4 py-2 rounded-lg text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-950/30 font-medium transition-all duration-200"
                        >
                            <LogOut size={18} />
                            <span className="hidden sm:inline">Đăng xuất</span>
                        </button>
                    </div>
                </div>
            </div>
        </header>
    );
};

export default CustomerHeader;
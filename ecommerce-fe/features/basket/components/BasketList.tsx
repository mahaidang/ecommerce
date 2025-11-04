"use client";

import { BasketItem } from "../types";
import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction,
} from "@/components/ui/alert-dialog";
import { useState } from "react";

interface BasketListProps {
  items: BasketItem[];
  onRemove: (id: string) => void;
  onUpdate: (id: string, quantity: number) => void;
  selected: string[];
  onSelect: (id: string, checked: boolean) => void;
}

export default function BasketList({ items, onRemove, onUpdate, selected, onSelect }: BasketListProps) {
  const [deleteId, setDeleteId] = useState<string | null>(null);

  if (items.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 px-4">
        <div className="w-24 h-24 mb-4 rounded-full bg-gray-100 dark:bg-gray-800 flex items-center justify-center">
          <svg className="w-12 h-12 text-gray-400 dark:text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
        </div>
        <p className="text-gray-500 dark:text-gray-300 text-lg font-medium">Giỏ hàng trống</p>
        <p className="text-gray-400 dark:text-gray-500 text-sm mt-1">Thêm sản phẩm để bắt đầu mua sắm</p>
      </div>
    );
  }

  return (
  <div className="space-y-3">
      {items.map((item, idx) => (
        <div
          key={idx}
          className="group relative bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-500 hover:shadow-md transition-all duration-200 overflow-hidden"
        >
          <div className="flex items-center gap-4 p-4">
            {/* Checkbox với animation */}
            <div className="flex-shrink-0">
              <label className="relative inline-flex items-center cursor-pointer">
                <input
                  type="checkbox"
                  checked={selected.includes(item.id)}
                  onChange={e => onSelect(item.id, e.target.checked)}
                  className="sr-only peer"
                />
                <div className="w-5 h-5 border-2 border-gray-300 rounded peer-checked:bg-blue-600 peer-checked:border-blue-600 transition-all duration-200 flex items-center justify-center">
                  {selected.includes(item.id) && (
                    <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                    </svg>
                  )}
                </div>
              </label>
            </div>

            {/* Ảnh sản phẩm với hover effect */}
            <div className="flex-shrink-0 w-20 h-20 bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-800 dark:to-gray-900 rounded-lg overflow-hidden border border-gray-200 dark:border-gray-700">
              <img
                src={item.imageUrl?.url || "/placeholder.png"}
                alt={item.imageUrl?.alt || item.name}
                className="w-full h-full object-contain p-2 group-hover:scale-110 transition-transform duration-300"
              />
            </div>

            {/* Thông tin sản phẩm */}
            <div className="flex-1 min-w-0">
              <h3 className="font-semibold text-gray-900 dark:text-gray-100 text-base mb-1 truncate">
                {item.name}
              </h3>
              <div className="flex items-baseline gap-2">
                <span className="text-lg font-bold text-blue-600 dark:text-blue-400">
                  {item.price.toLocaleString("vi-VN")}₫
                </span>
              </div>
            </div>

            {/* Quantity controls */}
            <div className="flex items-center gap-2 flex-shrink-0">
              <button
                onClick={() => onUpdate(item.id, Math.max(1, item.quantity - 1))}
                className="w-8 h-8 rounded-lg border border-gray-300 dark:border-gray-700 hover:border-blue-500 dark:hover:border-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/30 transition-colors duration-200 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 12H4" />
                </svg>
              </button>
              
              <input
                type="number"
                min={1}
                value={item.quantity}
                onChange={e => onUpdate(item.id, Math.max(1, Number(e.target.value)))}
                className="w-14 h-8 border border-gray-300 dark:border-gray-700 rounded-lg text-center font-medium text-gray-900 dark:text-gray-100 bg-white dark:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent"
              />
              
              <button
                onClick={() => onUpdate(item.id, item.quantity + 1)}
                className="w-8 h-8 rounded-lg border border-gray-300 dark:border-gray-700 hover:border-blue-500 dark:hover:border-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/30 transition-colors duration-200 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
              </button>
            </div>

            {/* Nút xóa */}
            <AlertDialog>
              <AlertDialogTrigger asChild>
                <button
                  onClick={() => setDeleteId(item.id)}
                  className="flex-shrink-0 w-9 h-9 rounded-lg hover:bg-red-50 dark:hover:bg-red-900/30 transition-colors duration-200 flex items-center justify-center text-gray-400 dark:text-gray-500 hover:text-red-600 dark:hover:text-red-400 group/delete"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </AlertDialogTrigger>
              <AlertDialogContent className="rounded-2xl bg-white dark:bg-gray-900">
                <AlertDialogHeader>
                  <AlertDialogTitle className="text-xl text-gray-900 dark:text-gray-100">Xác nhận xóa sản phẩm</AlertDialogTitle>
                  <AlertDialogDescription className="text-base text-gray-600 dark:text-gray-300">
                    Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng?
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel className="rounded-lg bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-200 hover:bg-gray-200 dark:hover:bg-gray-700 px-6 py-2 text-base font-medium">Hủy</AlertDialogCancel>
                  <AlertDialogAction
                    onClick={() => {
                      if (deleteId) onRemove(deleteId);
                      setDeleteId(null);
                    }}
                    className="bg-red-600 hover:bg-red-700 dark:bg-red-700 dark:hover:bg-red-800 text-white rounded-lg px-6 py-2 text-base font-medium"
                  >
                    Xóa
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </div>

          {/* Subtle gradient overlay on hover */}
          <div className="absolute inset-0 bg-gradient-to-r from-transparent via-blue-50/0 to-blue-50/0 dark:via-blue-900/0 dark:to-blue-900/0 group-hover:via-blue-50/30 group-hover:to-blue-50/10 dark:group-hover:via-blue-900/20 dark:group-hover:to-blue-900/10 pointer-events-none transition-all duration-300"></div>
        </div>
      ))}
    </div>
  );
}
"use client";
import { useCustomerOrdersPaging } from "../hooks";
import { useState } from "react";
import { useRouter } from "next/navigation";
import Pagination from "@/features/products/shared/components/Pagination";


function formatDate(date: string) {
  return new Date(date).toLocaleString("vi-VN");
}

function formatVND(amount: number) {
  return amount.toLocaleString("vi-VN", { maximumFractionDigits: 0 }) + "₫";
}

const statusMap: Record<string, { label: string; color: string; icon: string }> = {
  Pending: { 
    label: "Đang xử lý", 
    color: "bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-900/20 dark:text-amber-300 dark:border-amber-800",
    icon: "M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
  },
  Paid: { 
    label: "Đã thanh toán", 
    color: "bg-blue-50 text-blue-700 border-blue-200 dark:bg-blue-900/20 dark:text-blue-300 dark:border-blue-800",
    icon: "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
  },
  Completed: { 
    label: "Hoàn tất", 
    color: "bg-emerald-50 text-emerald-700 border-emerald-200 dark:bg-emerald-900/20 dark:text-emerald-300 dark:border-emerald-800",
    icon: "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
  },
  Cancelled: { 
    label: "Đã hủy", 
    color: "bg-red-50 text-red-700 border-red-200 dark:bg-red-900/20 dark:text-red-300 dark:border-red-800",
    icon: "M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"
  },
};

export default function CustomerOrderList() {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<string>("");
  const pageSize = 10;
  const userId = "8a27f2e7-05ab-f011-b964-88a4c2217c4a"; // TODO: lấy từ context đăng nhập
  const { data: items, total, isLoading } = useCustomerOrdersPaging({ userId, page, pageSize, status });
  const router = useRouter();
  const totalPages = Math.ceil(total / pageSize);

  if (isLoading) {
    return (
      <div className="p-6 max-w-4xl mx-auto">
        <div className="animate-pulse space-y-4">
          <div className="h-8 bg-gray-200 dark:bg-gray-700 rounded w-48"></div>
          <div className="space-y-3">
            {[1, 2, 3].map(i => (
              <div key={i} className="h-20 bg-gray-100 dark:bg-gray-800 rounded-xl"></div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (!items || items.length === 0) {
    return (
      <div className="p-6 max-w-4xl mx-auto">
        <div className="flex flex-col items-center justify-center py-16 px-4">
          <div className="w-24 h-24 mb-6 rounded-full bg-gradient-to-br from-gray-100 to-gray-200 dark:from-gray-800 dark:to-gray-700 flex items-center justify-center">
            <svg className="w-12 h-12 text-gray-400 dark:text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </div>
          <h3 className="text-xl font-semibold text-gray-700 dark:text-gray-300 mb-2">Chưa có đơn hàng</h3>
          <p className="text-gray-500 dark:text-gray-400 text-center max-w-md">
            Bạn chưa có đơn hàng nào. Hãy khám phá sản phẩm và tạo đơn hàng đầu tiên của bạn!
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="mb-6 flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-2">Đơn hàng của bạn</h1>
          <p className="text-gray-600 dark:text-gray-400">Quản lý và theo dõi các đơn hàng của bạn</p>
        </div>
        <div>
          <label htmlFor="order-status" className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Lọc theo trạng thái</label>
          <select
            id="order-status"
            className="block w-44 rounded-lg border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-900 px-3 py-2 text-sm text-gray-700 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={status}
            onChange={e => { setStatus(e.target.value); setPage(1); }}
          >
            <option value="">Tất cả</option>
            <option value="Pending">Đang xử lý</option>
            <option value="Paid">Đã thanh toán</option>
            <option value="Completed">Hoàn tất</option>
            <option value="Cancelled">Đã hủy</option>
          </select>
        </div>
      </div>

      <div className="space-y-4">
        {items.map((order: any) => {
          const status = statusMap[order.status] || { 
            label: order.status, 
            color: "bg-gray-100 text-gray-700 border-gray-300 dark:bg-gray-800 dark:text-gray-300 dark:border-gray-700",
            icon: "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          };
          
          return (
            <div
              key={order.id}
              className="group bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 hover:border-blue-300 dark:hover:border-blue-700 hover:shadow-lg transition-all duration-300 overflow-hidden"
            >
              <div className="p-3">
                <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
                  {/* Left section */}
                  <div className="flex-1 space-y-3">
                    <div className="flex items-start gap-4">
                      <div className="flex-shrink-0 w-12 h-12 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center">
                        <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                        </svg>
                      </div>
                      
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-3 mb-2">
                          <h3 className="text-lg font-semibold text-gray-900 dark:text-white font-mono">
                            #{order.orderNo}
                          </h3>
                          <div className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold border ${status.color}`}>
                            <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={status.icon} />
                            </svg>
                            {status.label}
                          </div>
                        </div>
                        
                        <div className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                          </svg>
                          {formatDate(order.createdAtUtc)}
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Right section */}
                  <div className="flex items-center gap-6">
                    <div className="text-right">
                      <div className="text-sm text-gray-500 dark:text-gray-400 mb-1">Tổng tiền</div>
                      <div className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-blue-500 bg-clip-text text-transparent">
                        {formatVND(order.grandTotal)}
                      </div>
                    </div>

                    <button
                      onClick={() => router.push(`/customer/orders/${order.id}`)}
                      className="flex items-center gap-2 px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-medium transition-all duration-200 hover:shadow-lg hover:scale-105 active:scale-95"
                    >
                      <span>Chi tiết</span>
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>

              {/* Hover gradient effect */}
              <div className="h-1 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 transform scale-x-0 group-hover:scale-x-100 transition-transform duration-500 origin-left"></div>
            </div>
          );
        })}
      </div>

      <div className="mt-8">
        <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
      </div>
    </div>
  );
}
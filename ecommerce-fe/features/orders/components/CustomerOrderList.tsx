"use client";
import { useCustomerOrders } from "../hooks";
import { CustomerOrder } from "../types";
import { useRouter } from "next/navigation";

function formatDate(date: string) {
  return new Date(date).toLocaleString("vi-VN");
}

const statusMap: Record<CustomerOrder["status"], { label: string; color: string }> = {
  pending: { label: "Chờ xác nhận", color: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200" },
  processing: { label: "Đang xử lý", color: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200" },
  shipped: { label: "Đã gửi hàng", color: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200" },
  delivered: { label: "Đã giao", color: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200" },
  cancelled: { label: "Đã hủy", color: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200" },
};


export default function CustomerOrderList() {
  const { data: orders, isLoading } = useCustomerOrders();
  const router = useRouter();

  if (isLoading) return <div className="p-6 text-gray-500">Đang tải...</div>;
  if (!orders || orders.length === 0)
    return (
      <div className="p-6 text-gray-500 dark:text-gray-400">Bạn chưa có đơn hàng nào.</div>
    );

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Đơn hàng của bạn</h1>
      <div className="overflow-x-auto">
        <table className="min-w-full border rounded-xl overflow-hidden bg-white dark:bg-gray-900">
          <thead>
            <tr className="bg-gray-100 dark:bg-gray-800">
              <th className="px-4 py-2 text-left">Mã đơn</th>
              <th className="px-4 py-2 text-left">Ngày tạo</th>
              <th className="px-4 py-2 text-left">Trạng thái</th>
              <th className="px-4 py-2 text-right">Tổng tiền</th>
              <th className="px-4 py-2"></th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id} className="border-b last:border-b-0 hover:bg-gray-50 dark:hover:bg-gray-800 transition cursor-pointer">
                <td className="px-4 py-2 font-mono">{order.orderNo}</td>
                <td className="px-4 py-2">{formatDate(order.createdAtUtc)}</td>
                <td className="px-4 py-2">
                  <span className={`px-2 py-1 rounded text-xs font-semibold ${statusMap[order.status].color}`}>
                    {statusMap[order.status].label}
                  </span>
                </td>
                <td className="px-4 py-2 text-right font-bold text-blue-600 dark:text-blue-400">
                  {order.grandTotal.toLocaleString("vi-VN")}₫
                </td>
                <td className="px-4 py-2 text-right">
                  <button
                    className="text-blue-600 dark:text-blue-400 hover:underline text-sm"
                    onClick={() => router.push(`/customer/orders/${order.id}`)}
                  >
                    Xem chi tiết
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

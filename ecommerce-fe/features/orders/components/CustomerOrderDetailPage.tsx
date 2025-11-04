"use client";
import { useCustomerOrderDetail } from "../hooks";
import { CustomerOrder } from "../types";

const statusMap: Record<CustomerOrder["status"], { label: string; color: string }> = {
  pending: { label: "Chờ xác nhận", color: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200" },
  processing: { label: "Đang xử lý", color: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200" },
  shipped: { label: "Đã gửi hàng", color: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200" },
  delivered: { label: "Đã giao", color: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200" },
  cancelled: { label: "Đã hủy", color: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200" },
};

export default function CustomerOrderDetailPage({ orderId }: { orderId: string }) {
  const { data: order, isLoading, error } = useCustomerOrderDetail(orderId);
  if (isLoading) return <div className="p-6 text-gray-500">Đang tải...</div>;
  if (!order) return <div className="p-6 text-red-500">Không tìm thấy đơn hàng.</div>;

  return (
    <div className="max-w-2xl mx-auto p-6">
  <h1 className="text-2xl font-bold mb-2">Chi tiết đơn hàng {order.orderNo}</h1>
  <div className="mb-2 text-gray-500 text-sm">Mã đơn: <span className="font-mono">{order.id}</span></div>
  <div className="mb-2 text-gray-500 text-sm">Ngày tạo: {new Date(order.createdAtUtc).toLocaleString("vi-VN")}</div>
  <div className="mb-2 text-gray-500 text-sm">Ngày cập nhật: {new Date(order.updatedAtUtc).toLocaleString("vi-VN")}</div>
      <div className="mb-2">
        <span className={`px-2 py-1 rounded text-xs font-semibold ${statusMap[order.status].color}`}>
          {statusMap[order.status].label}
        </span>
      </div>
      <div className="mb-4">
        <div className="font-semibold mb-1">Sản phẩm:</div>
        <ul className="divide-y">
          {order.items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 py-2">
              <img src={item.imageUrl || "/placeholder.png"} alt={item.productName} className="w-10 h-10 rounded border bg-gray-50 dark:bg-gray-800" />
              <div className="flex-1">
                <div className="font-medium text-gray-900 dark:text-gray-100">{item.productName}</div>
                <div className="text-xs text-gray-500 dark:text-gray-400">SKU: {item.sku}</div>
                <div className="text-xs text-gray-500 dark:text-gray-400">x{item.quantity}</div>
              </div>
              <div className="text-right">
                <div className="font-semibold text-blue-600 dark:text-blue-400">
                  {item.unitPrice.toLocaleString("vi-VN")}₫
                </div>
                <div className="text-xs text-gray-500 dark:text-gray-400">Tổng: {item.lineTotal.toLocaleString("vi-VN")}₫</div>
              </div>
            </li>
          ))}
        </ul>
      </div>
      <div className="mb-4 border-t pt-4">
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Tạm tính:</span>
          <span>{order.subtotal.toLocaleString("vi-VN")}₫</span>
        </div>
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Giảm giá:</span>
          <span>-{order.discountTotal.toLocaleString("vi-VN")}₫</span>
        </div>
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Phí vận chuyển:</span>
          <span>{order.shippingFee.toLocaleString("vi-VN")}₫</span>
        </div>
        <div className="flex justify-between font-bold text-lg text-blue-600 dark:text-blue-400 mt-2">
          <span>Thành tiền:</span>
          <span>{order.grandTotal.toLocaleString("vi-VN")}₫</span>
        </div>
      </div>
      {/* Các nút thao tác mock */}
      <div className="flex gap-3 justify-end">
        <button className="px-5 py-2 rounded-lg bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-200 hover:bg-gray-200 dark:hover:bg-gray-700 font-medium">Liên hệ hỗ trợ</button>
        {order.status === "pending" && (
          <button className="px-5 py-2 rounded-lg bg-red-600 hover:bg-red-700 text-white font-medium">Hủy đơn hàng</button>
        )}
        {order.status === "delivered" && (
          <button className="px-5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-medium">Mua lại</button>
        )}
      </div>
    </div>
  );
}

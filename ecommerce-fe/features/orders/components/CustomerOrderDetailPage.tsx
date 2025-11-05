"use client";

import { useCustomerOrderDetail } from "../hooks";

const statusMap: Record<string, { label: string; color: string }> = {
  Pending: { label: "Chờ xác nhận", color: "bg-amber-100 text-amber-700" },
  Paid: { label: "Đã thanh toán", color: "bg-blue-100 text-blue-700" },
  Completed: { label: "Hoàn tất", color: "bg-emerald-100 text-emerald-700" },
  Cancelled: { label: "Đã hủy", color: "bg-red-100 text-red-700" },
};

const formatPrice = (price: number, currency?: string) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: currency || "VND" }).format(price);


export default function CustomerOrderDetailPage({ orderId }: { orderId: string }) {
  const { data: order, isLoading, error } = useCustomerOrderDetail(orderId);
  if (isLoading) return <div className="p-6 text-gray-500">Đang tải...</div>;
  if (!order) return <div className="p-6 text-red-500">Không tìm thấy đơn hàng.</div>;


  // Normalize status to PascalCase for mapping and UI logic
  const normalizeStatus = (s: string) => {
    if (!s) return s;
    return s.charAt(0).toUpperCase() + s.slice(1).toLowerCase();
  };
  const normalizedStatus = normalizeStatus(order.status);
  const status = statusMap[normalizedStatus] || { label: order.status, color: "bg-gray-100 text-gray-700" };

  return (
    <div className="max-w-2xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-2">Chi tiết đơn hàng {order.orderNo}</h1>
      <div className="mb-2 text-gray-500 text-sm">Mã đơn: <span className="font-mono">{order.id}</span></div>
      <div className="mb-2 text-gray-500 text-sm">Ngày tạo: {order.createdAtUtc && new Date(order.createdAtUtc).toLocaleString("vi-VN")}</div>
      <div className="mb-2 text-gray-500 text-sm">Ngày cập nhật: {order.updatedAtUtc && new Date(order.updatedAtUtc).toLocaleString("vi-VN")}</div>
      {/* Thông tin nhận hàng */}
      <div className="mb-2 text-gray-700 dark:text-gray-300">
  <div><span className="font-semibold">Địa chỉ nhận hàng:</span> {order.address || "-"}</div>
  <div><span className="font-semibold">Tên người nhận:</span> {order.name || "-"}</div>
  <div><span className="font-semibold">Số điện thoại:</span> {order.phone || "-"}</div>
      </div>
      <div className="mb-2">
        <span className={`px-2 py-1 rounded text-xs font-semibold ${status.color}`}>{status.label}</span>
      </div>
      <div className="mb-4">
        <div className="font-semibold mb-1">Sản phẩm:</div>
        {order.items && order.items.length > 0 ? (
          <ul className="divide-y">
            {order.items.map((item: any) => (
              <li key={item.id} className="flex items-center gap-3 py-2">
                <img src={item.imageUrl || "/placeholder.png"} alt={item.productName} className="w-10 h-10 rounded border bg-gray-50 dark:bg-gray-800" />
                <div className="flex-1">
                  <div className="font-medium text-gray-900 dark:text-gray-100">{item.productName}</div>
                  <div className="text-xs text-gray-500 dark:text-gray-400">SKU: {item.sku}</div>
                  <div className="text-xs text-gray-500 dark:text-gray-400">x{item.quantity}</div>
                </div>
                <div className="text-right">
                  <div className="font-semibold text-blue-600 dark:text-blue-400">
                    {formatPrice(item.unitPrice, order.currency)}
                  </div>
                  <div className="text-xs text-gray-500 dark:text-gray-400">Tổng: {formatPrice(item.lineTotal, order.currency)}</div>
                </div>
              </li>
            ))}
          </ul>
        ) : (
          <div className="text-gray-500 dark:text-gray-400">Không có sản phẩm nào trong đơn hàng này.</div>
        )}
      </div>
      <div className="mb-4 border-t pt-4">
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Tạm tính:</span>
          <span>{formatPrice(order.subtotal, order.currency)}</span>
        </div>
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Giảm giá:</span>
          <span>-{formatPrice(order.discountTotal, order.currency)}</span>
        </div>
        <div className="flex justify-between text-gray-700 dark:text-gray-300 mb-1">
          <span>Phí vận chuyển:</span>
          <span>{formatPrice(order.shippingFee, order.currency)}</span>
        </div>
        <div className="flex justify-between font-bold text-lg text-blue-600 dark:text-blue-400 mt-2">
          <span>Thành tiền:</span>
          <span>{formatPrice(order.grandTotal, order.currency)}</span>
        </div>
      </div>
      <div className="mb-4 text-gray-700 dark:text-gray-300">
        <span className="font-semibold">Ghi chú:</span> {order.note || "-"}
      </div>
      {/* Các nút thao tác mock */}
      <div className="flex gap-3 justify-end">
        <button className="px-5 py-2 rounded-lg bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-200 hover:bg-gray-200 dark:hover:bg-gray-700 font-medium">Liên hệ hỗ trợ</button>
  {normalizedStatus === "Pending" && (
          <button className="px-5 py-2 rounded-lg bg-red-600 hover:bg-red-700 text-white font-medium">Hủy đơn hàng</button>
        )}
  {normalizedStatus === "Completed" && (
          <button className="px-5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-medium">Mua lại</button>
        )}
      </div>
    </div>
  );
}

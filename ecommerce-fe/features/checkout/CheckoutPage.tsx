"use client";
import React, { useState, useEffect } from "react";
import { useCreateOrder } from "@/features/orders/hooks";
import { getUserIdFromToken } from "@/lib/auth";
import { useRouter } from "next/navigation";
import { BackButton } from "@/components/ui/BackButton";

function formatVND(amount: number) {
  return amount.toLocaleString("vi-VN", { maximumFractionDigits: 0 }) + "₫";
}


// Nhận prop products là danh sách sản phẩm đã chọn
import { BasketItem } from "@/features/basket/types";

interface CheckoutPageProps {
  products: BasketItem[];
}

export default function CheckoutPage({ products }: CheckoutPageProps) {
  const router = useRouter();
  const [productList, setProductList] = useState<BasketItem[]>(products);
  const [quantities, setQuantities] = useState<Record<string, number>>(() => {
    const initial: Record<string, number> = {};
    products.forEach((p: BasketItem) => { initial[p.id] = p.quantity; });
    return initial;
  });

  // Luôn đồng bộ lại state khi products thay đổi (khi vào lại trang checkout)
  React.useEffect(() => {
    setProductList(products);
    const initial: Record<string, number> = {};
    products.forEach((p: BasketItem) => { initial[p.id] = p.quantity; });
    setQuantities(initial);
  }, [products]);
  const [specialRequests, setSpecialRequests] = useState({
    transferData: false,
    invoice: false,
    other: false,
    otherText: "",
  });
  const [paymentMethod, setPaymentMethod] = useState("cod");
  const [agree, setAgree] = useState(false);
  const [discountCode, setDiscountCode] = useState("");

  const userId = getUserIdFromToken() ?? "";
  const createOrderMutation = useCreateOrder();

  // Tính tổng tiền
  const subtotal = productList.reduce((sum, p) => sum + p.price * (quantities[p.id] || 1), 0);
  const total = subtotal;

  // Xử lý xóa sản phẩm
  const handleRemove = (id: string) => {
    const newList = productList.filter(p => p.id !== id);
    setProductList(newList);
    // Nếu xóa hết thì quay lại trang trước
    if (newList.length === 0) {
      setTimeout(() => {
        router.back();
      }, 300);
    }
  };

  // Tổng hợp note
  function getNote() {
    let note = "Yêu cầu đặc biệt";
    if (specialRequests.invoice) note += "\nXuất hóa đơn công ty";
    if (specialRequests.other && specialRequests.otherText) note += `\nYêu cầu khác: ${specialRequests.otherText}`;
    return note;
  }

  // Xử lý đặt hàng
  const handleOrder = async () => {
    if (!agree || productList.length === 0 || !userId) return;
    const items = productList.map(p => ({
      productId: p.id,
      productName: p.name,
      unitPrice: p.price,
      quantity: quantities[p.id] || 1,
    }));
    try {
      await createOrderMutation.mutateAsync({
        userId,
        discountTotal: 0, // Có thể xử lý giảm giá nếu có
        shippingFee: 0, // Có thể bổ sung nếu có
        note: getNote(),
        items,
      });
      // Sau khi đặt hàng thành công, quay về trang đơn hàng hoặc trang chủ
      router.push("/customer/orders");
    } catch (err) {
      alert((err as Error).message || "Đặt hàng thất bại");
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-neutral-900 py-6">
      <div className="max-w-xl mx-auto bg-white dark:bg-neutral-900 rounded-2xl shadow-xl border border-gray-200 dark:border-neutral-800 p-6">
        <BackButton fallbackHref="/" className="mb-4" />
        {/* Địa chỉ nhận hàng */}
        <div className="mb-4 p-4 rounded-xl bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-700 flex items-center gap-3">
          <svg className="w-6 h-6 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a2 2 0 01-2.828 0l-4.243-4.243a8 8 0 1111.314 0z" /><circle cx="12" cy="11" r="3" /></svg>
          <span>Vui lòng cung cấp thông tin nhận hàng</span>
          <span className="ml-auto text-blue-600 dark:text-blue-400 font-medium cursor-pointer">Thành phố Hồ Chí Minh</span>
        </div>
        {/* Sản phẩm */}
        {productList.map((product, idx) => (
          <div key={product.id} className="mb-4 bg-gray-50 dark:bg-neutral-800 rounded-xl p-4 border border-gray-200 dark:border-neutral-700">
            <div className="flex gap-4 items-center">
              <img src={product.imageUrl?.url || "/placeholder.png"} alt={product.imageUrl?.alt || product.name} className="w-20 h-20 rounded-xl bg-white border border-gray-200 dark:bg-neutral-900 dark:border-neutral-700" />
              <div className="flex-1 min-w-0">
                <div className="font-semibold text-gray-900 dark:text-white">{product.name}</div>
                {/* Có thể bổ sung thuộc tính khác nếu cần */}
              </div>
              <div className="flex flex-col items-center gap-2">
                <button className="text-gray-400 hover:text-red-500" onClick={() => handleRemove(product.id)}>Xoá</button>
                <div className="flex items-center gap-2">
                  <button onClick={() => setQuantities(q => ({ ...q, [product.id]: Math.max(1, (q[product.id] || 1) - 1) }))} className="w-7 h-7 rounded bg-gray-100 dark:bg-neutral-700 text-lg">-</button>
                  <span>{quantities[product.id] || 1}</span>
                  <button onClick={() => setQuantities(q => ({ ...q, [product.id]: (q[product.id] || 1) + 1 }))} className="w-7 h-7 rounded bg-gray-100 dark:bg-neutral-700 text-lg">+</button>
                </div>
              </div>
            </div>
            <div className="mt-2 text-right text-gray-700 dark:text-gray-200 text-sm">Tạm tính ({quantities[product.id] || 1} sản phẩm): <span className="font-semibold">{formatVND(product.price * (quantities[product.id] || 1))}</span></div>
          </div>
        ))}
        {/* Yêu cầu đặc biệt */}
        <div className="mb-4">
          <div className="font-semibold mb-2">Yêu cầu đặc biệt</div>
          <div className="flex flex-col gap-2">
            <label className="flex items-center gap-2">
              <input type="checkbox" checked={specialRequests.invoice} onChange={e => setSpecialRequests(r => ({ ...r, invoice: e.target.checked }))} />
              Xuất hóa đơn công ty
            </label>
            <label className="flex items-center gap-2">
              <input type="checkbox" checked={specialRequests.other} onChange={e => setSpecialRequests(r => ({ ...r, other: e.target.checked }))} />
              Yêu cầu khác
            </label>
            {specialRequests.other && (
              <textarea
                className="mt-2 p-2 border border-gray-300 dark:border-gray-700 rounded-lg resize-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:outline-none"
                rows={2}
                placeholder="Nhập yêu cầu khác..."
                value={specialRequests.otherText}
                onChange={e => setSpecialRequests(r => ({ ...r, otherText: e.target.value }))}
              />
            )}
          </div>
        </div>
        {/* Mã giảm giá */}
        <div className="mb-4">
          <div className="font-semibold mb-2">Nhập mã giảm giá</div>
          <div className="flex gap-2">
            <input
              type="text"
              className="flex-1 p-2 border border-gray-300 dark:border-gray-700 rounded-lg focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:outline-none"
              placeholder="Nhập mã giảm giá..."
              value={discountCode}
              onChange={e => setDiscountCode(e.target.value)}
            />
            <button className="px-4 py-2 rounded-lg bg-blue-600 text-white font-medium hover:bg-blue-700 transition" type="button" disabled>Áp dụng</button>
          </div>
        </div>
        {/* Tổng tiền */}
        <div className="mb-4 bg-gray-50 dark:bg-neutral-800 rounded-xl p-4 border border-gray-200 dark:border-neutral-700">
          <div className="flex justify-between items-center mb-1">
            <span className="font-semibold text-lg">Tổng tiền</span>
            <span className="text-2xl font-bold text-red-600 dark:text-red-400">{formatVND(total)}</span>
          </div>
        </div>
        {/* Hình thức thanh toán */}
        <div className="mb-4">
          <div className="font-semibold mb-2">Hình thức thanh toán</div>
          <div className="flex flex-col gap-2">
            <label className="flex items-center gap-2">
              <input type="radio" name="payment" value="cod" checked={paymentMethod === "cod"} onChange={() => setPaymentMethod("cod")} />
              <span>Thanh toán tiền mặt khi nhận hàng</span>
            </label>
            <label className="flex items-center gap-2">
              <input type="radio" name="payment" value="bank" checked={paymentMethod === "bank"} onChange={() => setPaymentMethod("bank")} />
              <span>Chuyển khoản ngân hàng</span>
            </label>
            <div className="text-blue-600 dark:text-blue-400 text-xs cursor-pointer">7 hình thức thanh toán khác</div>
          </div>
        </div>
        {/* Chính sách & đặt hàng */}
        <div className="mb-4 flex items-center gap-2">
          <input type="checkbox" checked={agree} onChange={e => setAgree(e.target.checked)} />
          <span className="text-xs">Tôi đồng ý với <span className="text-blue-600 dark:text-blue-400 underline cursor-pointer">Chính sách xử lý dữ liệu cá nhân của E-Shop</span></span>
        </div>
        <button
          className="w-full py-3 rounded-xl bg-blue-600 hover:bg-blue-700 text-white font-bold text-lg disabled:opacity-60"
          disabled={!agree || productList.length === 0 || createOrderMutation.isPending}
          onClick={handleOrder}
        >
          {createOrderMutation.isPending ? "Đang đặt hàng..." : "Đặt hàng"}
        </button>
      </div>
    </div>
  );
}

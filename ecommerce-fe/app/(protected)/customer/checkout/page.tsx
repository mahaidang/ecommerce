"use client";

import CheckoutPage from "@/features/checkout/CheckoutPage";
import { useEffect, useState } from "react";
import { getUserIdFromToken } from "@/lib/auth";
import { getOrCreateSessionId } from "@/lib/session";
import { useBasket } from "@/features/basket/hooks";

export default function CheckoutRoute() {
  const userId = getUserIdFromToken();
  const sessionId = getOrCreateSessionId();
  const { data: basket, isLoading } = useBasket(userId ?? undefined, sessionId);
  const [selected, setSelected] = useState<string[]>([]);

  useEffect(() => {
    if (typeof window !== "undefined") {
      const ids = localStorage.getItem("checkout_selected");
      if (ids) setSelected(JSON.parse(ids));
    }
  }, []);

  if (isLoading) return <div className="p-8 text-gray-500">Đang tải giỏ hàng...</div>;
  if (!basket) return <div className="p-8 text-gray-500">Không tìm thấy giỏ hàng.</div>;

  // Lọc sản phẩm đã chọn
  const products = basket.items.filter(item => selected.includes(item.id));

  return <CheckoutPage products={products} />;
}

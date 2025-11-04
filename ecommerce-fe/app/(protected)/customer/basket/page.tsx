"use client";

import { useState } from "react";
import BasketList from "@/features/basket/components/BasketList";
import BasketActions from "@/features/basket/components/BasketActions";
import { getUserIdFromToken } from "@/lib/auth";
import { getOrCreateSessionId } from "@/lib/session";
import { useBasket, useUpdateAllBasket, useRemoveItem } from "@/features/basket/hooks";


export default function BasketPage() {
    const userId = getUserIdFromToken();
    const sessionId = getOrCreateSessionId();
    const { data: basket, isLoading } = useBasket(userId ?? undefined, sessionId);
    const [selected, setSelected] = useState<string[]>([]); // id làm key
    // State tạm cho số lượng chỉnh sửa
    const [editQuantities, setEditQuantities] = useState<Record<string, number>>({});
    const [isEditing, setIsEditing] = useState(false);

    // Tick chọn
    const handleSelect = (id: string, checked: boolean) => {
        setSelected((prev) =>
            checked ? [...prev, id] : prev.filter((n) => n !== id)
        );
    };

    // Khi thay đổi số lượng chỉ update local state
    const handleUpdate = (id: string, quantity: number) => {
        setEditQuantities((prev) => ({ ...prev, [id]: quantity }));
        setIsEditing(true);
    };

    // Lưu số lượng đã chỉnh: gọi API updateAllBasket
    const updateAllMutation = useUpdateAllBasket();
    const handleSave = () => {
        if (!basket) return;
        // Map từ id -> productId
        const items = basket.items.map(item => ({
            productId: item.id,
            quantity: editQuantities[item.id] ?? item.quantity,
        }));
        updateAllMutation.mutate({
            userId,
            sessionId,
            items,
        });
        setIsEditing(false);
        setEditQuantities({});
    };

    // Thanh toán nhiều
    const handleCheckout = () => {
        alert("Thanh toán các sản phẩm: " + selected.join(", "));
    };

    // Xóa từng item
    const removeItemMutation = useRemoveItem();
    const handleRemove = (id: string) => {
        removeItemMutation.mutate({
            userId: userId ?? undefined,
            sessionId,
            productId: id,
        });
        setSelected((prev) => prev.filter((n) => n !== id));
    };

    if (isLoading) return <div className="p-8 text-gray-500">Đang tải giỏ hàng...</div>;
    if (!basket) return <div className="p-8 text-gray-500">Không tìm thấy giỏ hàng.</div>;

    // Dữ liệu hiển thị: nếu có chỉnh sửa thì lấy số lượng từ editQuantities, không thì lấy từ basket
    const displayItems = basket.items.map((item) => ({
        ...item,
        quantity: editQuantities[item.id] ?? item.quantity,
    }));

    return (
        <div className="max-w-2xl mx-auto p-4">
            <h1 className="text-xl font-semibold mb-4">Giỏ hàng của bạn</h1>

            <BasketList
                items={displayItems}
                onRemove={handleRemove}
                onUpdate={handleUpdate}
                selected={selected}
                onSelect={handleSelect}
            />



            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 mt-8">
                <BasketActions
                    selected={selected}
                    onCheckout={handleCheckout}
                />
                {isEditing && (
                    <button
                        className="px-6 py-2 rounded bg-green-700 hover:bg-green-900 text-white font-medium self-end sm:self-auto"
                        onClick={handleSave}
                    >
                        Lưu số lượng
                    </button>
                )}
            </div>

            <div className="mt-6 text-right font-medium">
                Tổng tiền: {" "}
                <span className="text-blue-600">
                    {displayItems
                        .filter((item) => selected.length === 0 || selected.includes(item.id))
                        .reduce((sum, item) => sum + item.price * item.quantity, 0)
                        .toLocaleString("vi-VN")}
                    ₫
                </span>
            </div>
        </div>
    );
}

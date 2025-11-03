"use client";
import { BasketItem } from "../types";

interface BasketListProps {
  items: BasketItem[];
  onRemove: (name: string) => void;
  onUpdate: (name: string, quantity: number) => void;
  selected: string[];
  onSelect: (name: string, checked: boolean) => void;
}

export default function BasketList({ items, onRemove, onUpdate, selected, onSelect }: BasketListProps) {
  if (items.length === 0) {
    return <div className="text-center text-gray-400 py-8">Giỏ hàng trống</div>;
  }

  return (
    <ul className="divide-y">
      {items.map((item, idx) => (
        <li key={idx} className="flex items-center gap-4 py-3">
          {/* Tick chọn */}
          <input
            type="checkbox"
            checked={selected.includes(item.name)}
            onChange={e => onSelect(item.name, e.target.checked)}
            className="accent-blue-600 w-4 h-4"
          />

          {/* Ảnh sản phẩm */}
          <img
            src={item.imageUrl?.url || "/placeholder.png"}
            alt={item.imageUrl?.alt || item.name}
            className="w-14 h-14 object-contain rounded border bg-gray-50"
          />

          {/* Tên & thông tin */}
          <div className="flex-1">
            <div className="font-medium text-gray-800">{item.name}</div>
            <div className="text-sm text-gray-500">
              Giá: {item.price.toLocaleString("vi-VN")}₫
            </div>
          </div>

          {/* Input số lượng */}
          <input
            type="number"
            min={1}
            value={item.quantity}
            onChange={e => onUpdate(item.name, Number(e.target.value))}
            className="w-16 border rounded px-2 py-1 text-center"
          />

          {/* Nút xoá */}
          <button
            onClick={() => onRemove(item.name)}
            className="ml-2 text-red-500 hover:underline text-xs"
          >
            Xóa
          </button>
        </li>
      ))}
    </ul>
  );
}

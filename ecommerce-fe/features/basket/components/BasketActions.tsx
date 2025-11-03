"use client";

interface BasketActionsProps {
  selected: string[];
  onDelete: () => void;
  onCheckout: () => void;
}

export default function BasketActions({ selected, onDelete, onCheckout }: BasketActionsProps) {
  return (
    <div className="flex gap-2 ">
      <button
        className="px-4 py-2 rounded bg-red-500 text-white disabled:opacity-50"
        disabled={selected.length === 0}
        onClick={onDelete}
      >
        Xóa mục đã chọn
      </button>
      <button
        className="px-4 py-2 rounded bg-blue-600 text-white disabled:opacity-50"
        disabled={selected.length === 0}
        onClick={onCheckout}
      >
        Thanh toán mục đã chọn
      </button>
    </div>
  );
}

"use client";

interface BasketActionsProps {
  selected: string[];
  onCheckout: () => void;
}

export default function BasketActions({ selected, onCheckout }: BasketActionsProps) {
  return (
    <div className="flex gap-2 ">
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

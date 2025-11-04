"use client";
import { useRouter } from "next/navigation";

interface BasketActionsProps {
  selected: string[];
  onCheckout?: () => void;
}

export default function BasketActions({ selected, onCheckout }: BasketActionsProps) {
  const router = useRouter();
  const handleCheckout = () => {
    if (onCheckout) onCheckout();
    if (typeof window !== "undefined") {
      localStorage.setItem("checkout_selected", JSON.stringify(selected));
    }
    router.push("checkout");
  };
  return (
    <div className="flex gap-2 ">
      <button
        className="px-4 py-2 rounded bg-blue-600 text-white disabled:opacity-50"
        disabled={selected.length === 0}
        onClick={handleCheckout}
      >
        Tiến hành đặt hàng
      </button>
    </div>
  );
}

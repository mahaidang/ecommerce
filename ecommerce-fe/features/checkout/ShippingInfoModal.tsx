import React from "react";

interface ShippingInfoModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (info: { name: string; address: string; phone: string }) => void;
  initialInfo?: { name: string; address: string; phone: string };
}

export default function ShippingInfoModal({ open, onClose, onSave, initialInfo }: ShippingInfoModalProps) {
  const [info, setInfo] = React.useState({
    name: initialInfo?.name || "",
    address: initialInfo?.address || "",
    phone: initialInfo?.phone || ""
  });
  const [errors, setErrors] = React.useState<{ name?: string; address?: string; phone?: string }>({});

  React.useEffect(() => {
    if (open) {
      setInfo({
        name: initialInfo?.name || "",
        address: initialInfo?.address || "",
        phone: initialInfo?.phone || ""
      });
    }
  }, [open, initialInfo]);

  if (!open) return null;

  // Validate
  const validate = () => {
    const newErrors: { name?: string; address?: string; phone?: string } = {};
    if (!info.name.trim()) newErrors.name = "Vui lòng nhập họ tên";
    if (!info.address.trim()) newErrors.address = "Vui lòng nhập địa chỉ";
    if (!/^0\d{8,10}$/.test(info.phone)) newErrors.phone = "Số điện thoại phải bắt đầu bằng 0 và có 9-11 số";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleConfirm = () => {
    if (validate()) {
      onSave(info);
      onClose();
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
      <div className="bg-white dark:bg-neutral-900 rounded-2xl p-8 max-w-md w-full shadow-2xl border border-gray-200 dark:border-neutral-800">
        <h2 className="text-xl font-bold mb-4 text-gray-900 dark:text-gray-100">Thông tin nhận hàng</h2>
        <div className="space-y-4">
          <div>
            <input
              type="text"
              className={`w-full p-3 rounded-lg border ${errors.name ? "border-red-500" : "border-gray-300 dark:border-gray-700"} focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:outline-none`}
              placeholder="Họ và tên"
              value={info.name}
              onChange={e => setInfo(i => ({ ...i, name: e.target.value }))}
            />
            {errors.name && <div className="text-red-500 text-xs mt-1">{errors.name}</div>}
          </div>
          <div>
            <input
              type="text"
              className={`w-full p-3 rounded-lg border ${errors.phone ? "border-red-500" : "border-gray-300 dark:border-gray-700"} focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:outline-none`}
              placeholder="Số điện thoại"
              value={info.phone}
              onChange={e => setInfo(i => ({ ...i, phone: e.target.value }))}
            />
            {errors.phone && <div className="text-red-500 text-xs mt-1">{errors.phone}</div>}
          </div>
          <div>
            <input
              type="text"
              className={`w-full p-3 rounded-lg border ${errors.address ? "border-red-500" : "border-gray-300 dark:border-gray-700"} focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:outline-none`}
              placeholder="Địa chỉ nhận hàng"
              value={info.address}
              onChange={e => setInfo(i => ({ ...i, address: e.target.value }))}
            />
            {errors.address && <div className="text-red-500 text-xs mt-1">{errors.address}</div>}
          </div>
        </div>
        <div className="flex gap-3 mt-6">
          <button
            className="flex-1 px-6 py-3 rounded-xl bg-gray-100 dark:bg-neutral-800 text-gray-700 dark:text-gray-300 font-semibold hover:bg-gray-200 dark:hover:bg-neutral-700 border border-gray-200 dark:border-neutral-700"
            onClick={onClose}
          >
            Hủy
          </button>
          <button
            className="flex-1 px-6 py-3 rounded-xl bg-gradient-to-r from-blue-600 to-blue-700 text-white font-semibold hover:from-blue-700 hover:to-blue-800 transition-all shadow-lg shadow-blue-500/30"
            onClick={handleConfirm}
          >
            Xác nhận
          </button>
        </div>
      </div>
    </div>
  );
}

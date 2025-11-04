// Lấy chi tiết đơn hàng mock
export function useCustomerOrderDetail(orderId: string) {
  const order = mockOrders.find((o) => o.id === orderId);
  return { data: order, isLoading: false, error: null };
}
import { CustomerOrder, CustomerOrderItem } from "./types";

// Mock data
const mockOrders: CustomerOrder[] = [
  {
    id: "1",
    userId: "u1",
    orderNo: "DH20231101",
    status: "delivered",
    currency: "VND",
    subtotal: 1200000,
    discountTotal: 50000,
    shippingFee: 30000,
    grandTotal: 1180000,
    note: "Giao giờ hành chính",
    createdAtUtc: "2025-11-01T10:00:00Z",
    updatedAtUtc: "2025-11-01T12:00:00Z",
    items: [
      {
        id: "oi1",
        orderId: "1",
        productId: "p1",
        productName: "Áo thun nam",
        sku: "ATN001",
        unitPrice: 200000,
        quantity: 2,
        lineTotal: 400000,
        imageUrl: "/placeholder.png",
      },
      {
        id: "oi2",
        orderId: "1",
        productId: "p2",
        productName: "Quần jeans nữ",
        sku: "QJN002",
        unitPrice: 800000,
        quantity: 1,
        lineTotal: 800000,
        imageUrl: "/placeholder.png",
      },
    ],
  },
  {
    id: "2",
    userId: "u1",
    orderNo: "DH20231102",
    status: "processing",
    currency: "VND",
    subtotal: 350000,
    discountTotal: 0,
    shippingFee: 20000,
    grandTotal: 370000,
    note: "",
    createdAtUtc: "2025-11-02T14:30:00Z",
    updatedAtUtc: "2025-11-02T15:00:00Z",
    items: [
      {
        id: "oi3",
        orderId: "2",
        productId: "p3",
        productName: "Giày thể thao",
        sku: "GST003",
        unitPrice: 350000,
        quantity: 1,
        lineTotal: 350000,
        imageUrl: "/placeholder.png",
      },
    ],
  },
];

export function useCustomerOrders() {
  // In real app, replace with data fetching logic
  return { data: mockOrders, isLoading: false, error: null };
}

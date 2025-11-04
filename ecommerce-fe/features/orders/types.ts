export interface CustomerOrderItem {
  id: string;
  orderId: string;
  productId: string;
  productName: string;
  sku: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
  imageUrl?: string;
}

export interface CustomerOrder {
  id: string;
  userId: string;
  orderNo: string;
  status: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled';
  currency: string;
  subtotal: number;
  discountTotal: number;
  shippingFee: number;
  grandTotal: number;
  note?: string;
  createdAtUtc: string;
  updatedAtUtc: string;
  items: CustomerOrderItem[];
}

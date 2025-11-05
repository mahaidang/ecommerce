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
  address: string,
  name: string,
  phone: string,
  createdAtUtc: string;
  updatedAtUtc: string;
  items: CustomerOrderItem[];
}

export interface CreateOrderCommand {
  userId: string;
  discountTotal: number;
  shippingFee: number;
  note: string;
  address: string;
  name: string;
  phone: string;
  items: Array<{
    productId: string;
    productName: string;
    unitPrice: number;
    quantity: number;
  }>;
}

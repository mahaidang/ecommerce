// Dto cho cập nhật giỏ hàng (chuẩn backend)
export interface UpsertBasket {
  userId?: string | null;
  sessionId?: string | null;
  items: UpsertBasketItem[];
}

export interface UpsertBasketItem {
  productId: string;
  quantity: number;
}

export interface ProductImage {
  // thêm field theo cấu trúc thực tế của bạn, ví dụ:
  url: string;
  alt?: string;
  isMain: boolean;
  publicId: string;
}

export interface BasketItem {
  imageUrl: ProductImage;
  name: string;
  price: number;
  quantity: number;
  id: string;
}

export interface Basket {
  userId?: string | null;
  sessionId?: string | null;
  items: BasketItem[];
}



import api from "@/lib/api";
import { Basket, UpsertBasket } from "./types";

export const basketApi = {
    //lấy basket theo user
  async detail(params: { userId?: string; sessionId?: string }): Promise<Basket> {
    const { userId, sessionId } = params;
    const res = await api.get("/api/basket/baskets", {
      params: { userId, sessionId },
    });
    return res.data;
  },

  // update toàn bộ giỏ hàng
  async updateAllBasket(dto: UpsertBasket) {
    const res = await api.put("/api/basket/baskets/update-all", dto);
    return res.data;
  },
  // Lưu 1 item vào giỏ hàng
  async saveItem(params: { userId?: string; sessionId?: string; productId: string; quantity: number }) {
    const { userId, sessionId, productId, quantity } = params;
    const res = await api.post("/api/basket", { productId, quantity }, {
      params: { userId, sessionId },
    });
    return res.data;
  },
};
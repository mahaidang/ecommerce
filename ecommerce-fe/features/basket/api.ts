
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
        const res = await api.post("/api/basket/baskets", { productId, quantity }, {
            params: { userId, sessionId },
        });
        return res.data;
    },

    // Xóa 1 sản phẩm khỏi giỏ hàng
    async removeItem(params: { userId?: string; sessionId?: string; productId: string }) {
        const { userId, sessionId, productId } = params;
        const res = await api.delete(`/api/basket/baskets/items/${productId}`, {
            params: { userId, sessionId },
        });
        return res.data;
    },
};
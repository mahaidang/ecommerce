// React hooks for customer basket/cart
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { basketApi } from "./api";
import { Basket } from "./types";

export function useBasket(userId?: string, sessionId?: string) {
  return useQuery<Basket>({
    queryKey: ["basket", userId || sessionId],
    queryFn: () => basketApi.detail({ userId, sessionId }),
    enabled: !!(userId || sessionId), // chỉ gọi nếu có 1 trong 2
  });
}

// Hook update toàn bộ giỏ hàng
export function useUpdateAllBasket() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: basketApi.updateAllBasket,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["basket"] });
    },
  });
}


// export function useAddToBasket() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: addToBasket,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }

// export function useRemoveFromBasket() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: removeFromBasket,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }

// export function useUpdateBasketItem() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: updateBasketItem,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }

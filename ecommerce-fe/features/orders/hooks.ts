import { useQuery, useMutation } from "@tanstack/react-query";
import { ordersApi, OrdersListParams, OrdersListResult, CreateOrderCommand } from "./api";

// Lấy chi tiết đơn hàng từ API
export function useCustomerOrderDetail(orderId: string) {
  const query = useQuery({
    queryKey: ["order-detail", orderId],
    queryFn: () => ordersApi.getById(orderId),
    enabled: !!orderId,
    staleTime: 1000 * 30,
  });
  return {
    data: query.data,
    isLoading: query.isLoading,
    error: query.error,
  };
}

// Hook lấy danh sách đơn hàng có phân trang
export function useCustomerOrdersPaging(params: OrdersListParams) {
  const query = useQuery({
    queryKey: ["orders", params],
    queryFn: () => ordersApi.listWithPaging(params),
    staleTime: 1000 * 30,
  });
  const result = query.data as OrdersListResult | undefined;
  return {
    data: result?.items ?? [],
    total: result?.total ?? 0,
    page: result?.page ?? params.page ?? 1,
    pageSize: result?.pageSize ?? 20,
    isLoading: query.isLoading,
    error: query.error,
  };
}

export function useCreateOrder() {
  return useMutation({
    mutationFn: (cmd: CreateOrderCommand) => ordersApi.create(cmd),
  });
}


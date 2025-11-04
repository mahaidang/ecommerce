import { CustomerOrder } from "./types";
import api from "@/lib/api";
import axios from "axios";

export interface OrdersListParams {
	userId: string;
	page?: number;
	pageSize?: number;
	status?: string;
}

export interface OrdersListResult {
	items: CustomerOrder[];
	total: number;
	page: number;
	pageSize: number;
}

export interface CreateOrderItem {
	productId: string;
	productName: string;
	unitPrice: number;
	quantity: number;
}

export interface CreateOrderCommand {
	userId: string;
	discountTotal: number;
	shippingFee: number;
	note: string;
	items: CreateOrderItem[];
}

export const ordersApi = {
	async listWithPaging(params: OrdersListParams): Promise<OrdersListResult> {
		const queryParams: Record<string, string> = {
			userId: params.userId,
			page: String(params.page ?? 1),
			pageSize: String(params.pageSize ?? 20),
		};
		if (params.status !== undefined && params.status !== "") {
			queryParams.status = params.status;
		}
		const query = new URLSearchParams(queryParams).toString();
		const res = await api.get<OrdersListResult>(`/api/ordering/orders?${query}`);
		return res.data;
	},
	async getById(id: string) {
		const res = await api.get<CustomerOrder>(`/api/ordering/orders/${id}`);
		return res.data;
	},
	async create(cmd: CreateOrderCommand, signal?: AbortSignal) {
		try {
			const res = await api.post("/api/ordering/orders", cmd, { signal });
			return res.data;
		} catch (err: any) {
			if (err.response && err.response.data && err.response.data.title) {
				throw new Error(err.response.data.detail || err.response.data.title);
			}
			throw err;
		}
	},
};

// Đã chuyển vào ordersApi.create

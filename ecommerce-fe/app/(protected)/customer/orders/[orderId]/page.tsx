import { notFound } from "next/navigation";
import CustomerOrderDetailPage from "@/features/orders/components/CustomerOrderDetailPage";

export default function Page({ params }: { params: { orderId: string } }) {
  if (!params.orderId) return notFound();
  return <CustomerOrderDetailPage orderId={params.orderId} />;
}

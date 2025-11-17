# 🛒 ECommerce Microservices Platform  
### Nền Tảng Thương Mại Điện Tử Kiến Trúc Microservices

Hệ thống eCommerce phân tán được xây dựng với **.NET 8**, **YARP Gateway**, **Saga Orchestration (RabbitMQ)**, **gRPC**, và mô hình **Polyglot Persistence**, mô phỏng đầy đủ quy trình của một nền tảng thương mại điện tử hiện đại.

---

## ⭐ Mục tiêu của dự án

- Xây dựng nền tảng eCommerce backend hoàn chỉnh theo kiến trúc **Microservices**.  
- Áp dụng **Clean Architecture**, **CQRS**, **DDD** để tối ưu khả năng mở rộng và bảo trì.  
- Triển khai **Saga Orchestration** với RabbitMQ để quản lý workflow đặt hàng có rollback.  
- Tích hợp **YARP API Gateway** để định tuyến, xác thực và bảo mật tập trung.  
- Sử dụng **gRPC** nhằm giảm độ trễ trong giao tiếp nội bộ giữa các service.  
- Áp dụng **Polyglot Persistence** (SQL Server, MongoDB, Redis) phù hợp với từng bài toán dữ liệu.  
- Tăng độ ổn định và hiệu năng hệ thống, hướng tới **API response < 100ms**.  
- Container hóa toàn bộ hệ thống bằng **Docker Compose**.  
- Mô phỏng đầy đủ quy trình eCommerce: đăng nhập, giỏ hàng, kiểm kho, thanh toán, xác nhận đơn hàng.  
- Nâng cao kỹ năng thiết kế distributed system, quản lý workflow, bảo mật API và xử lý sự kiện realtime.  

---

## 🛠️ Công nghệ sử dụng

### **Backend & Framework**
- .NET 8 – ASP.NET Core Web API  
- Clean Architecture, DDD (Domain-Driven Design)  
- CQRS + MediatR  
- FluentValidation  

### **Service Communication**
- **YARP API Gateway** – định tuyến & bảo mật tập trung  
- **gRPC** – giao tiếp nội bộ tốc độ cao  
- **RabbitMQ** – Event Bus & Saga Orchestration  

### **Authentication & Security**
- JWT Authentication  
- RBAC (Role-Based Access Control)  
- Idempotency Handling (đặc biệt cho module thanh toán)  

### **Databases (Polyglot Persistence)**
- **SQL Server** – Lưu trữ giao dịch (Orders, Users, Inventory)  
- **MongoDB** – Dữ liệu sản phẩm  
- **Redis** – Cache & giỏ hàng  

### **Payments**
- Sepay Payment Webhook – xử lý thanh toán realtime  

### **Containerization**
- Docker & Docker Compose  

---

## 🔌 Port Mapping

| Service / Component       | Port      |
|---------------------------|-----------|
| API Gateway (YARP)        | **8000**  |
| UserService               | **8001**  |
| ProductService            | **8002**  |
| OrderService              | **8003**  |
| InventoryService          | **8004**  |
| BasketService             | **8005**  |
| PaymentService            | **8006**  |
| Saga Orchestrator         | internal  |
| SQL Server                | **1433**  |
| MongoDB                   | **27017** |
| Redis                     | **6379**  |
| RabbitMQ (AMQP)           | **5672**  |
| RabbitMQ Management UI    | **15672** |

---

## 📦 Kiến trúc tổng quan (Overview)
**Coming soon ...**

---
<!--
## 🚀 Cách chạy dự án

### 1. Clone repo  
```bash
git clone https://github.com/mahaidang/ecommerce.git
cd ecommerce
### 2. Chạy hệ thống bằng Docker Compose  
docker-compose up -d
-->

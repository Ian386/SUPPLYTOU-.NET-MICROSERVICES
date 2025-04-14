# 🌍 SUPPLYTOU - GIS-Based Food Supply Chain Tracking Platform

This project is a modular, microservices-based platform that enables end-to-end tracking of food products — from farm to retailer — with role-based access, live logistics, and traceability features. Built in .NET Core, it supports JWT-based authentication and secure inter-service communication.

---

## 🧩 Microservices Overview

This system is composed of **4 main microservices**:

| Microservice | Responsibility |
|--------------|----------------|
| **UserService** | Manages user registration, login, roles, and JWT authentication |
| **FarmService** | Handles farm registration, crop data, and harvest tracking |
| **ProductTrackingService** | Tracks the lifecycle of food products (status, location, delivery) |
| **LogisticsService** | Manages truck logistics, driver assignments, and shipping status |

Planned:
- **NotificationService** for email/SMS updates

Each service runs independently and communicates using secure HTTP requests, with future support for asynchronous event messaging.

---

## 🛠️ Technologies Used

- ASP.NET Core 7 Web API
- Entity Framework Core
- SQL Server (via Docker or local)
- ASP.NET Core Identity
- JWT Authentication & Authorization
- HTTP Clients with typed client support
- (Planned) RabbitMQ or Azure Service Bus
- Swagger/OpenAPI for API exploration

---

## 🔐 Authentication & Authorization

- **User roles**: `Admin`, `Farmer`, `Driver`
- **Login** issues a **JWT token** with embedded claims.
- Tokens must be passed in the `Authorization` header as a `Bearer` token.

```http
Authorization: Bearer {token}

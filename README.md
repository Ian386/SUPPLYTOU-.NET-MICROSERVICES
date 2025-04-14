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


---

## 🧩 Microservices Descriptions

### 1. **UserService**

- **Purpose**: Manages authentication and authorization using ASP.NET Identity and JWT.
- **Roles**: Admin, Farmer, Driver
- **Core Features**: 
  - Registration/Login
  - Token issuance
  - Role management

### 2. **FarmService**

- **Purpose**: Registers farms and records harvests and crop information.
- **Key Features**:
  - Farm profile management
  - Harvest tracking
  - Syncs new product info with ProductTrackingService

### 3. **ProductTrackingService**

- **Purpose**: Manages the lifecycle of food batches, including source, destination, and current status.
- **Key Features**:
  - Product status tracking
  - Location updates
  - Interacts with LogisticsService for dispatching

### 4. **LogisticsService**

- **Purpose**: Manages shipping logistics, including truck assignments and live asset tracking.
- **Key Features**:
  - Assign trucks and drivers
  - Track departure/arrival
  - Sync with ProductTrackingService upon dispatch

---

## 🔌 API Definitions

### UserService

| Endpoint | Method | Access | Description |
|----------|--------|--------|-------------|
| `/api/auth/register` | POST | Public | Register new users |
| `/api/auth/login` | POST | Public | Authenticate user and return JWT |
| `/api/users/me` | GET | Authenticated | Get logged-in user profile |

### FarmService

| Endpoint | Method | Access | Description |
|----------|--------|--------|-------------|
| `/api/farms` | POST | Farmer | Register new farm |
| `/api/farms` | GET | Admin/Farmer | View farms |
| `/api/harvests` | POST | Farmer | Submit new harvest and sync to ProductTrackingService |

### ProductTrackingService

| Endpoint | Method | Access | Description |
|----------|--------|--------|-------------|
| `/api/tracking` | POST | FarmService (secured) | Create tracking record |
| `/api/tracking` | GET | Admin | View all tracking entries |
| `/api/tracking/{id}/status` | PUT | LogisticsService | Update product shipping status |

### LogisticsService

| Endpoint | Method | Access | Description |
|----------|--------|--------|-------------|
| `/api/logistics` | POST | ProductTrackingService | Create logistics entry |
| `/api/logistics` | GET | Admin | View logistics records |

---

## 🗃️ Data Ownership & Persistence

| Service | Owns These Entities |
|---------|---------------------|
| UserService | ApplicationUser, Role (via ASP.NET Identity) |
| FarmService | Farm, Harvest |
| ProductTrackingService | ProductTrackingRecord |
| LogisticsService | LogisticsRecord |

Each service uses its **own SQL Server database**, with no direct sharing of schemas or tables.

---

## 🔄 Communication Patterns & Resilience

### ✅ Sync HTTP

- FarmService → ProductTrackingService (new harvest)
- ProductTrackingService → LogisticsService (ready to ship)
- UserService → All others for authentication

### 🔁 Planned Async Messaging

- ProductTrackingService → NotificationService (status updates)
- LogisticsService → ProductTrackingService (arrival events)

### 🔐 Security & Resilience

- All inter-service requests are secured with **JWT** tokens.
- Use of **Typed HTTP Clients** for clean, resilient service calls.
- Retry strategies and fallbacks (to be added for async handlers).
- Status tracking and internal retries are supported for logistics and shipping events.

---


## 🔐 Authentication & Authorization

- **Implemented via ASP.NET Core Identity**
- **JWT** is issued by the `/api/auth/login` endpoint
- Roles enforced via `[Authorize(Roles = "Farmer")]` etc.
- Include JWT in the `Authorization` header:

```http
Authorization: Bearer {your-token-here}


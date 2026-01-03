
---

# 📦 Event-Driven Distributed Order Processing Platform

> **Production-grade microservices system built using .NET, RabbitMQ, Kafka, SQL Server, PostgreSQL, Docker, and Azure Blob Storage.**

This project demonstrates **real-world distributed system design**, focusing on **reliability, scalability, and eventual consistency** using **event-driven architecture**.

---

## 🚀 Key Highlights 

* ✔️ Event-Driven Microservices Architecture
* ✔️ RabbitMQ for transactional workflows
* ✔️ Kafka for high-throughput analytics
* ✔️ Outbox Pattern for reliable event publishing
* ✔️ Saga (Choreography) for distributed transactions
* ✔️ Idempotent consumers
* ✔️ Retry & failure handling
* ✔️ SQL Server + PostgreSQL
* ✔️ Azure Blob Storage (Free Tier)
* ✔️ Docker-ready services
* ✔️ Clean Architecture & SOLID principles

---

## 🧠 System Architecture

### 🔷 High-Level Architecture Diagram

```
Client
  |
  v
Order Service (SQL Server)
  |
  |-- RabbitMQ --> Payment Service (PostgreSQL)
  |                 |
  |                 v
  |            Inventory Service (SQL Server)
  |
  |-- Kafka --> Analytics Service --> Azure Blob Storage
```

---

## 🧩 Services Overview

### 1️⃣ Order Service (Core Entry Point)

* ASP.NET Core Web API
* SQL Server (SSMS)
* Implements **Outbox Pattern**
* Publishes events to:

  * RabbitMQ (business workflows)
  * Kafka (analytics)

**Key Concepts**

* Atomic DB + event persistence
* Reliable message publishing
* Clean separation of concerns

---

### 2️⃣ Payment Service

* ASP.NET Worker Service
* PostgreSQL
* RabbitMQ consumer
* **Idempotent payment processing**

**Key Concepts**

* Unique constraint on `OrderId`
* Retry mechanism
* At-least-once delivery handling

---

### 3️⃣ Inventory Service

* ASP.NET Worker Service
* SQL Server
* RabbitMQ consumer
* Implements **Saga (Choreography)**

**Key Concepts**

* Stock reservation
* Compensation on order cancellation
* Event-driven consistency

---

### 4️⃣ Analytics Service

* ASP.NET Worker Service
* Kafka consumer
* Azure Blob Storage (Free Tier)

**Key Concepts**

* Event streaming
* Non-blocking analytics
* Immutable event archival

---

## 🔁 Event Flow

### ✅ Happy Path

```
Order Created
   ↓
RabbitMQ → Payment Service
   ↓
RabbitMQ → Inventory Service
   ↓
Kafka → Analytics Service
```

### ❌ Failure Path (Payment Failure)

```
Payment Failed
   ↓
Retry (MassTransit)
   ↓
Dead Letter Queue
   ↓
Order Cancelled Event
   ↓
Inventory Compensation
```

---

## 🧠 Design Patterns Used

| Pattern                   | Usage                       |
| ------------------------- | --------------------------- |
| Event-Driven Architecture | Entire system               |
| Outbox Pattern            | Order Service               |
| Saga (Choreography)       | Order → Payment → Inventory |
| Idempotency               | Payment Service             |
| Retry Pattern             | RabbitMQ consumers          |
| Dead Letter Queue         | Failure handling            |
| Clean Architecture        | All services                |

---

## 🛠️ Technology Stack

| Category   | Technology             |
| ---------- | ---------------------- |
| Backend    | ASP.NET Core (.NET 8)  |
| Messaging  | RabbitMQ               |
| Streaming  | Apache Kafka           |
| Databases  | SQL Server, PostgreSQL |
| Cloud      | Azure Blob Storage     |
| Containers | Docker                 |
| Logging    | Serilog                |
| ORM        | Entity Framework Core  |

---

## 🐳 Docker Usage

* RabbitMQ (local Docker)
* Kafka + Zookeeper (local Docker)
* Services are **Docker-ready**

> Business databases are intentionally hosted locally to demonstrate **polyglot persistence**.

---

## ☁️ Azure Usage (Free Tier)

* Azure Blob Storage – analytics event storage
* Azure App Service (optional)
* Azure Monitor (basic logs)

---

## 📂 Repository Structure

```
distributed-order-platform/
│
├── OrderService/
├── PaymentService/
├── InventoryService/
├── AnalyticsService/
│
├── docs/
│   └── architecture.png
│
├── docker-compose.yml (optional)
└── README.md
```

---

## 🧪 How to Run Locally

1. Start RabbitMQ (Docker)
2. Start Kafka + Zookeeper (Docker)
3. Run OrderService
4. Run PaymentService
5. Run InventoryService
6. Run AnalyticsService
7. POST `/api/orders`

---
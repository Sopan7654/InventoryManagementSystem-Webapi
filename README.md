<div align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/MySQL-005C84?style=for-the-badge&logo=mysql&logoColor=white" alt="MySQL" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=.net&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/Web_API-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="Web API" />
  <img src="https://img.shields.io/badge/xUnit-000000?style=for-the-badge&logo=xunit&logoColor=white" alt="xUnit" />

  <h1>📦 Enterprise Inventory Management System</h1>
  <p><strong>A robust, Clean Architecture-based inventory and stock management solution built with ASP.NET Core 8 Web API, Entity Framework Core, MySQL, and Redis.</strong></p>
</div>

---

## 📖 Overview

The **Enterprise Inventory Management System** is a high-performance application designed to handle end-to-end inventory operations. From tracking real-time stock levels and managing multiple warehouses, to processing purchase orders and calculating expiring batches, the system guarantees ACID-compliant transactions and provides comprehensive analytics.

Originally built as a console application with raw ADO.NET, the system has been **modernized** with a **Clean Architecture Web API**, **Code-First Entity Framework Core**, and **Redis Caching**, making it highly maintainable, scalable, and database-agnostic. 

Note: A legacy Console UI is also preserved in the repository for quick administrative tasks.

---

## ✨ Key Features

- **🚀 Real-Time Stock Management** – Add, update, and track product inventory across multiple warehouses.
- **🔐 JWT Authentication & Authorization** – Secure endpoints with role-based and policy-based access control.
- **⚡ Redis Caching** – High-performance caching for frequently accessed data.
- **🛡️ ACID-Compliant Transactions** – Ensures data integrity for all stock movements (Stock In, Out, Transfers, Adjustments).
- **🛒 Purchase Order Workflow** – Create POs, add line items, and seamlessly receive goods to auto-update stock.
- **⏱️ Batch & Lot Tracking** – Monitor expiring products with automated warning systems for batches expiring within 30 days.
- **🏗️ Code-First Migrations** – Fast, reliable database schema updates using EF Core.
- **🐳 Docker Support** – Fast local development environment with `docker-compose`.

---

## 🏗️ Architecture Stack

This project strictly adheres to **Clean Architecture** patterns, maximizing separation of concerns:

- **API Layer:** ASP.NET Core 8 Web API with Swagger, JWT Auth, and Serilog.
- **Application Layer:** Use cases, DTOs, interfaces, and business logic.
- **Domain Layer:** Core enterprise rules, entities, and constants.
- **Infrastructure Layer:** Generic & Specific Repositories, EF Core 8 (Pomelo MySQL), and Redis Cache implementation.
- **Shared Layer:** Shared utilities and constants.
- **Database Subsystem:** **MySQL 8.0+** and **Redis**.
- **Testing:** **xUnit** framework with Integration and Unit Tests using EF Core InMemory & Moq.

---

## 🚀 Getting Started

Follow these instructions to get the project up and running on your local machine.

### 1️⃣ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop) (Recommended for easiest setup)
- IDE: Visual Studio / VS Code / JetBrains Rider

### 2️⃣ Clone the Repository

```bash
git clone https://github.com/your-username/InventoryManagementSystem.git
cd InventoryManagementSystem
```

### 3️⃣ Start Dependencies with Docker

The easiest way to get MySQL and Redis running is via the included `docker-compose.yml`:

```bash
docker-compose up -d
```
*This starts MySQL on port 3307 and Redis on port 6379.*

### 4️⃣ Configure Appsettings

Check the connection strings in `src/InventoryManagement.API/appsettings.json`. If using Docker, defaults should work.
Update `src/InventoryManagementSystem/appsettings.json` if using the Console UI.

### 5️⃣ Apply EF Core Migrations

Ensure the database schema is up-to-date. Run the EF Core migrations from the API project:

```bash
dotnet tool install --global dotnet-ef  # If not already installed
cd src/InventoryManagement.API
dotnet ef database update --project ../InventoryManagement.Infrastructure
cd ../..
```

### 6️⃣ Run the Application

You can run the full Web API:

```bash
dotnet run --project src/InventoryManagement.API/InventoryManagement.API.csproj
```
The API will be available at `https://localhost:5001/swagger` (or as configured by your launchSettings).

Alternatively, to run the legacy Console UI:
```bash
dotnet run --project src/InventoryManagementSystem/InventoryManagementSystem.csproj
```

---

## 🧪 Testing Framework

The project is backed by a robust automated test suite testing the API, Domain, and Application layers.

```bash
# Run all unit tests
dotnet test tests/InventoryManagement.UnitTests/InventoryManagement.UnitTests.csproj

# Run all integration tests
dotnet test tests/InventoryManagement.IntegrationTests/InventoryManagement.IntegrationTests.csproj

# Run legacy tests
dotnet test tests/InventoryManagementSystem.Tests/InventoryManagementSystem.Tests.csproj
```

---

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<p align="center">Made with ❤️ by the Inventory Management Team</p>

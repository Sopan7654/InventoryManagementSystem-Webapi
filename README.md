<div align="center">

  <!-- Technology Badges -->
  <p>
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
    <img src="https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react&logoColor=black" alt="React 19" />
    <img src="https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white" alt="MySQL" />
    <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
    <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
    <img src="https://img.shields.io/badge/Kubernetes-326CE5?style=for-the-badge&logo=kubernetes&logoColor=white" alt="Kubernetes" />
  </p>

  <h1>📦 Inventory Management System</h1>
  <p><strong>A full-stack, production-ready inventory management solution with ASP.NET Core 8 Web API, React 19 frontend, Redis caching, and Kubernetes deployment.</strong></p>

</div>

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Local Development](#option-1-local-development)
  - [Docker Compose](#option-2-docker-compose-recommended)
  - [Kubernetes](#option-3-kubernetes-deployment)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Configuration](#-configuration)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

The **Inventory Management System** is a modern, enterprise-grade application designed to handle end-to-end inventory operations. Built with a clean architecture approach using CQRS pattern with MediatR, it provides:

- **Real-time stock tracking** across multiple warehouses
- **Purchase order workflow** with supplier management
- **Batch & lot tracking** with expiry monitoring
- **JWT authentication** with role-based access control
- **Redis caching** for high-performance data retrieval
- **Comprehensive reporting** including ABC analysis and inventory valuation

---

## ✨ Features

| Module                 | Capabilities                                                                             |
| ---------------------- | ---------------------------------------------------------------------------------------- |
| **🔐 Authentication**  | JWT-based auth, user registration, login/logout, token refresh, role management          |
| **📦 Products**        | CRUD operations, soft delete, category assignment, SKU management                        |
| **🏷️ Categories**      | Hierarchical product categorization                                                      |
| **🏭 Warehouses**      | Multi-warehouse support, location tracking                                               |
| **📊 Inventory**       | Stock In/Out, transfers, adjustments, real-time quantity tracking                        |
| **📋 Purchase Orders** | PO creation, line items, supplier assignment, receiving workflow                         |
| **🤝 Suppliers**       | Supplier directory, contact management                                                   |
| **📅 Batches**         | Lot tracking, expiry date monitoring, batch-level inventory                              |
| **📈 Reports**         | Low stock alerts, overstock reports, expiring batches, ABC analysis, inventory valuation |

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              FRONTEND                                    │
│                     React 19 + Vite + Tailwind CSS                      │
│                         (Port 3000 / Nginx)                             │
└─────────────────────────────────┬───────────────────────────────────────┘
                                  │ HTTP/REST
┌─────────────────────────────────▼───────────────────────────────────────┐
│                              WEB API                                     │
│                   ASP.NET Core 8 (Port 5000/8080)                       │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  Controllers → MediatR → Command/Query Handlers → Repositories   │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                    │                              │                      │
│           FluentValidation              Redis Cache Service             │
└────────────────────┼──────────────────────────────┼─────────────────────┘
                     │                              │
┌────────────────────▼────────────┐  ┌─────────────▼──────────────────────┐
│           MySQL 8.0             │  │            Redis                    │
│    (Primary Data Store)         │  │   (Caching & Token Blacklist)      │
└─────────────────────────────────┘  └────────────────────────────────────┘
```

**Design Patterns Used:**

- **CQRS** (Command Query Responsibility Segregation) with MediatR
- **Repository Pattern** for data access abstraction
- **Chain of Responsibility** for validation pipeline (FluentValidation)
- **Dependency Injection** throughout

---

## 🛠️ Tech Stack

### Backend

| Technology              | Purpose                     |
| ----------------------- | --------------------------- |
| ASP.NET Core 8          | Web API framework           |
| MediatR                 | CQRS implementation         |
| FluentValidation        | Request validation          |
| Entity Framework Core 8 | ORM (Pomelo MySQL provider) |
| MySQL.Data (ADO.NET)    | Direct database access      |
| BCrypt.Net              | Password hashing            |
| JWT Bearer              | Authentication              |
| Redis                   | Distributed caching         |
| Swagger/OpenAPI         | API documentation           |

### Frontend

| Technology     | Purpose                 |
| -------------- | ----------------------- |
| React 19       | UI framework            |
| Vite 8         | Build tool & dev server |
| Tailwind CSS 4 | Styling                 |
| React Router 7 | Client-side routing     |
| Radix UI       | Accessible components   |
| Recharts       | Data visualization      |
| Lucide React   | Icons                   |

### Infrastructure

| Technology     | Purpose                                 |
| -------------- | --------------------------------------- |
| Docker         | Containerization                        |
| Docker Compose | Local orchestration                     |
| Kubernetes     | Production orchestration                |
| Nginx          | Frontend static serving & reverse proxy |

---

## 📁 Project Structure

```
InventoryManagementSystemwebapi/
├── src/
│   └── InventoryManagementSystem/          # ASP.NET Core Web API
│       ├── Common/                         # Shared interfaces & utilities
│       ├── Data/                           # Database context & seeds
│       ├── Domain/                         # Entities & domain models
│       ├── Features/                       # Feature modules (CQRS)
│       │   ├── Auth/                       # Authentication & authorization
│       │   ├── Batches/                    # Batch/lot management
│       │   ├── Categories/                 # Product categories
│       │   ├── Inventory/                  # Stock operations
│       │   ├── Products/                   # Product catalog
│       │   ├── PurchaseOrders/             # PO workflow
│       │   ├── Reports/                    # Analytics & reports
│       │   ├── Suppliers/                  # Supplier management
│       │   └── Warehouses/                 # Warehouse management
│       ├── Infrastructure/                 # Cross-cutting concerns
│       └── Program.cs                      # Application entry point
│
├── inventory-frontend/                     # React SPA
│   ├── src/
│   │   ├── components/                     # Reusable UI components
│   │   ├── pages/                          # Page components
│   │   ├── context/                        # React context providers
│   │   └── lib/                            # Utilities & API client
│   ├── Dockerfile                          # Frontend container
│   └── nginx.conf                          # Nginx configuration
│
├── tests/
│   └── InventoryManagementSystem.Tests/    # xUnit test project
│
├── k8s/                                    # Kubernetes manifests
│   ├── namespace.yaml
│   ├── mysql-secret.yaml
│   ├── mysql-pvc.yaml
│   ├── mysql-deployment.yaml
│   ├── api-configmap.yaml
│   ├── api-deployment.yaml
│   ├── frontend-deployment.yaml
│   ├── ingress.yaml
│   ├── hpa.yaml
│   └── README.md
│
├── docker-compose.yml                      # Docker Compose orchestration
├── Dockerfile                              # API container (multi-stage)
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) (for frontend development)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Redis](https://redis.io/download) (optional for local dev)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for containerized deployment)

---

### Option 1: Local Development

#### 1. Clone the Repository

```bash
git clone https://github.com/your-username/InventoryManagementSystemwebapi.git
cd InventoryManagementSystemwebapi
```

#### 2. Configure the Database

Create a MySQL database and update `src/InventoryManagementSystem/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "InventoryDB": "Server=localhost;Port=3306;Database=InventoryManagementDB;Uid=root;Pwd=YOUR_PASSWORD;",
    "Redis": "localhost:6379"
  }
}
```

> **Note:** The application auto-creates required tables on startup via `AppUserTableMigration`.

#### 3. Run the API

```bash
cd src/InventoryManagementSystem
dotnet run
```

API will be available at: `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

#### 4. Run the Frontend

```bash
cd inventory-frontend
npm install
npm run dev
```

Frontend will be available at: `http://localhost:5173`

---

### Option 2: Docker Compose (Recommended)

The easiest way to run the entire stack:

```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

**Services:**

| Service  | URL                           | Description          |
| -------- | ----------------------------- | -------------------- |
| Frontend | http://localhost:3000         | React application    |
| API      | http://localhost:5000         | ASP.NET Core Web API |
| Swagger  | http://localhost:5000/swagger | API documentation    |
| Redis    | localhost:6379                | Cache server         |

**Stop services:**

```bash
docker-compose down        # Stop containers
docker-compose down -v     # Stop and remove volumes
```

---

### Option 3: Kubernetes Deployment

For production deployment, see the [Kubernetes README](./k8s/README.md).

```bash
# Apply all manifests
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/mysql-secret.yaml
kubectl apply -f k8s/mysql-pvc.yaml
kubectl apply -f k8s/mysql-deployment.yaml
kubectl apply -f k8s/api-configmap.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/frontend-deployment.yaml
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml
```

---

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
| POST   | `/api/auth/register` | Register new user        |
| POST   | `/api/auth/login`    | Login and get JWT token  |
| POST   | `/api/auth/logout`   | Logout (blacklist token) |
| POST   | `/api/auth/refresh`  | Refresh JWT token        |

### Resource Endpoints

| Resource        | Endpoints                                                               |
| --------------- | ----------------------------------------------------------------------- |
| Products        | `GET/POST /api/products`, `GET/PUT/DELETE /api/products/{id}`           |
| Categories      | `GET/POST /api/categories`, `GET/PUT/DELETE /api/categories/{id}`       |
| Warehouses      | `GET/POST /api/warehouses`, `GET/PUT/DELETE /api/warehouses/{id}`       |
| Suppliers       | `GET/POST /api/suppliers`, `GET/PUT/DELETE /api/suppliers/{id}`         |
| Batches         | `GET/POST /api/batches`, `GET/PUT/DELETE /api/batches/{id}`             |
| Inventory       | `POST /api/inventory/stock-in`, `POST /api/inventory/stock-out`, etc.   |
| Purchase Orders | `GET/POST /api/purchaseorders`, `POST /api/purchaseorders/{id}/receive` |

### Report Endpoints

| Method | Endpoint                           | Description                  |
| ------ | ---------------------------------- | ---------------------------- |
| GET    | `/api/reports/low-stock`           | Products below reorder level |
| GET    | `/api/reports/overstock`           | Overstocked products         |
| GET    | `/api/reports/expiring-batches`    | Batches expiring soon        |
| GET    | `/api/reports/abc-analysis`        | ABC inventory classification |
| GET    | `/api/reports/inventory-valuation` | Total inventory value        |

> **Full API documentation available at `/swagger` when running the API.**

---

## 🧪 Testing

Run the automated test suite:

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific test project
dotnet test tests/InventoryManagementSystem.Tests/InventoryManagementSystem.Tests.csproj
```

**Test Coverage:**

- Entity model validation
- Command/Query handlers
- Business logic validation
- Repository operations

---

## ⚙️ Configuration

### Environment Variables (Docker/K8s)

| Variable                         | Description             | Default                     |
| -------------------------------- | ----------------------- | --------------------------- |
| `ASPNETCORE_ENVIRONMENT`         | Runtime environment     | `Production`                |
| `ConnectionStrings__InventoryDB` | MySQL connection string | -                           |
| `ConnectionStrings__Redis`       | Redis connection string | -                           |
| `Jwt__Key`                       | JWT signing key         | -                           |
| `Jwt__Issuer`                    | JWT issuer              | `InventoryManagementSystem` |
| `Jwt__Audience`                  | JWT audience            | `InventoryManagementSystem` |
| `Jwt__ExpiryMinutes`             | Token expiry            | `480`                       |

### Application Settings

```json
{
  "AppSettings": {
    "DefaultValuationMethod": "FIFO",
    "LowStockAlertEnabled": true,
    "AppName": "Inventory Management System",
    "Version": "1.0.0"
  }
}
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines

- Follow C# coding conventions and .NET best practices
- Write unit tests for new features
- Update documentation as needed
- Use meaningful commit messages

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<div align="center">
  <p>Built with ❤️ using .NET 8 and React 19</p>
  <p>
    <a href="#-table-of-contents">Back to Top ↑</a>
  </p>
</div>

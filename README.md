<div align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/MySQL-005C84?style=for-the-badge&logo=mysql&logoColor=white" alt="MySQL" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=.net&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/xUnit-000000?style=for-the-badge&logo=xunit&logoColor=white" alt="xUnit" />

  <h1>📦 Enterprise Inventory Management System</h1>
  <p><strong>A robust, console-based inventory and stock management solution built with C# .NET 8, Entity Framework Core, and MySQL.</strong></p>
</div>

---

## 📖 Overview

The **Enterprise Inventory Management System** is a high-performance console application designed to handle end-to-end inventory operations. From tracking real-time stock levels and managing multiple warehouses, to processing purchase orders and calculating expiring batches, the system guarantees ACID-compliant transactions and provides comprehensive analytics.

Originally built with raw ADO.NET, the system has been **modernized** with a **Code-First Entity Framework Core** architecture, making it highly maintainable and database-agnostic.

---

## ✨ Key Features

- **🚀 Real-Time Stock Management** – Add, update, and track product inventory across multiple warehouses.
- **🛡️ ACID-Compliant Transactions** – Ensures data integrity for all stock movements (Stock In, Out, Transfers, Adjustments).
- **🛒 Purchase Order Workflow** – Create POs, add line items, and seamlessly receive goods to auto-update stock.
- **⏱️ Batch & Lot Tracking** – Monitor expiring products with automated warning systems for batches expiring within 30 days.
- **📊 Business Analytics & Reports** – Generate low stock alerts, ABC analysis reports, and real-time inventory valuations.
- **🏗️ Code-First Migrations** – Fast, reliable database schema updates using EF Core.

---

## 🏗️ Architecture Stack

This project strictly adheres to a **Layered Architecture** pattern, maximizing separation of concerns:

- **Frontend:** Interactive Console UI (Structured Text Menus, Tables, Color Coded Output)
- **Business Logic:** C# Services Layer applying strict domain logic and validations
- **Data Access:** Generic & Specific Repositories using **EF Core 8** (Pomelo MySQL)
- **Database:** **MySQL 8.0+**
- **Testing:** **xUnit** framework with **EF Core InMemory** & **Moq** for rigorous automated unit tests.

---

## 🚀 Getting Started

Follow these instructions to get the project up and running on your local machine.

### 1️⃣ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/installer/) (v8.0+)
- IDE: Visual Studio / VS Code / JetBrains Rider

### 2️⃣ Clone the Repository

```bash
git clone https://github.com/your-username/InventoryManagementSystem.git
cd InventoryManagementSystem
```

### 3️⃣ Configure Database Settings

Navigate to `src/InventoryManagementSystem/appsettings.json` and update the connection string with your local MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=InventoryManagementDB;Uid=root;Pwd=YOUR_PASSWORD_HERE;"
  }
}
```

### 4️⃣ Apply EF Core Migrations

Since the project uses an EF Core Code-First approach, automatically create your database and schema by running:

```bash
dotnet tool install --global dotnet-ef  # If not already installed
cd src/InventoryManagementSystem
dotnet ef database update
cd ../..
```

### 5️⃣ Run the Application

```bash
dotnet run --project src/InventoryManagementSystem/InventoryManagementSystem.csproj
```

---

## 🧪 Testing Framework

The project is backed by a robust automated test suite avoiding a real database by leveraging the `EF Core InMemory` provider.

```bash
# Run all unit tests
dotnet test tests/InventoryManagementSystem.Tests/InventoryManagementSystem.Tests.csproj
```

_Current Coverage:_ Validates `ProductService`, `GenericRepository`, batch expiries, low stock triggers, transaction limits, and complex business logic.

---

## 📱 Application Menu Structure

When you launch the system, you'll be greeted with an intuitive command-line interface:

- **1. Product Management** _(Create, View, Update catalogs)_
- **2. Inventory Operations** _(Stock In, Out, Hold, Transfer, Adjustments)_
- **3. Purchase Orders** _(Raise POs to suppliers, Receive Shipments)_
- **4. Supplier Management** _(Supplier tracking)_
- **5. Warehouse Management** _(Create multiple storage locations and batches)_
- **6. Reports & Alerts** _(Valuation, Low Stock Alerts, ABC Analysis, Expiry tracking)_

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

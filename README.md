<div align="center">

# 🛒 Ghafar Tajhiz

### A Modern Multi-Layer E-Commerce Platform Built with ASP.NET Core MVC

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-68217A?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

*A multi-layer e-commerce platform consisting of two web applications: a Customer Store and an Admin Dashboard.*

</div>

---

# 📑 Table of Contents

- Overview
- Applications
- Architecture
- Features
- Solution Structure
- Database Design
- Technology Stack
- Screenshots

---

# 📖 Overview

**Ghafar Tajhiz** is a modern multi-layer e-commerce platform built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core**, and **SQL Server**.

The solution contains two independent web applications that share the same Business Logic and Data Access layers.

The project was developed to simulate a real-world online shopping platform while applying software engineering principles such as layered architecture, dependency injection, repository pattern, asynchronous programming, and clean separation of concerns.

---

# 🖥 Applications

## 🛍 Customer Website

- User Registration & Login
- Browse Products
- Product Details
- Product Search & Sorting
- Pagination
- Shopping Cart
- Checkout
- Order History
- Product Comments
- Profile Management

---

## ⚙️ Admin Dashboard

- Product Management (CRUD)
- Category Management (CRUD)
- Product Image Upload/Delete
- Order Management
- Order Approval & Cancellation
- Order Search & Sorting

---

# 🏗 Architecture

```mermaid
graph TD

Presentation["Presentation Layer<br/>MVC Controllers & Razor Views"]

Business["Business Logic Layer<br/>Services"]

Data["Data Access Layer<br/>Repositories"]

Database[(SQL Server)]

Presentation --> Business

Business --> Data

Data --> Database
```

---

# ✨ Features

## Architecture & Backend

- Layered Architecture
- Repository Pattern
- Service Layer
- Dependency Injection
- Entity Framework Core
- ASP.NET Identity
- Cookie Authentication
- DTO Pattern
- Async Programming
- LINQ

---

## Frontend

- Razor Views
- Partial Views
- HTML5
- CSS3
- JavaScript
- AJAX Requests

---

## Business Features

- Authentication
- Product Catalog
- Shopping Cart
- Checkout
- Customer Profile
- Order Management
- Product Comments
- File Upload
- Pagination
- Product Search & Sorting
- Data Validation

---

# 📁 Solution Structure

```text
Ghafar-Tajhiz

├── BusinessLogic
│   ├── BasketServices
│   ├── BasketItemServices
│   ├── CategoryServices
│   ├── CommentServices
│   ├── ProductServices
│   ├── ProfileServices
│   └── FileUpload
│
├── DataAccess
│   ├── Data
│   ├── Models
│   ├── Repositories
│   ├── Enums
│   └── Migrations
│
├── Ghafar-Tajhiz
│   ├── Controllers
│   ├── Views
│   └── wwwroot
│
└── Ghafar-Tajhiz-Admin
    ├── Controllers
    ├── Views
    └── wwwroot
```

---

# 🗄 Database Design

### Main Entities

- User
- Role
- Category
- Product
- Basket
- BasketItem
- Comment

```mermaid
erDiagram

CATEGORY ||--o{ PRODUCT : contains

PRODUCT ||--o{ COMMENT : has

USER ||--o{ BASKET : owns

BASKET ||--o{ BASKETITEM : contains

PRODUCT ||--o{ BASKETITEM : ordered
```

### Database Features

- Entity Framework Core Code First
- Foreign Key Constraints
- Composite Unique Index
- Cascade Delete
- Restrict Delete
- DataAnnotations Validation

---

# 🛠 Technology Stack

## Backend

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server
- ASP.NET Identity

## Frontend

- Razor Views
- HTML5
- CSS3
- JavaScript

## Development Tools

- Visual Studio 2022
- Git
- GitHub

---

# 📸 Screenshots


> Product List

<img width="1897" height="949" alt="Menu1" src="https://github.com/user-attachments/assets/ca28dcd7-1d07-40e5-91c1-62f2918335c7" />

---

> Product Details

<img width="1915" height="978" alt="ProductPage" src="https://github.com/user-attachments/assets/d80e1283-cebc-4055-b778-e3941bc8b7fa" />

---

> Shopping Cart

<img width="1914" height="981" alt="Basket" src="https://github.com/user-attachments/assets/22c25dd4-51c6-473d-9f98-7761a08b128e" />

---

> Customer Profile

<img width="1920" height="983" alt="Profile" src="https://github.com/user-attachments/assets/ffcca4db-f7ea-46ba-aaf4-0b7748547b12" />

---

> Login Page

<img width="1920" height="984" alt="Login" src="https://github.com/user-attachments/assets/981bfa20-e9d7-4613-94d2-ff9c5ec1cfcd" />

---

> Admin Dashboard

<img width="1920" height="957" alt="AdminCategories" src="https://github.com/user-attachments/assets/829fab9b-7951-4fb6-a836-e084ba9c4ba8" />

---

> Product Management

<img width="1915" height="982" alt="AdminProducts" src="https://github.com/user-attachments/assets/ad1759ac-8a87-4e7a-aa4b-a3eb1f8ce068" />

---

> Order Management

<img width="1920" height="985" alt="AdminOrders" src="https://github.com/user-attachments/assets/b8665170-fc86-43a1-9d52-2206f38dd984" />


---


<div align="left">

__Made with <img src="https://raw.githubusercontent.com/Tarikul-Islam-Anik/Telegram-Animated-Emojis/main/Symbols/Heart%20On%20Fire.webp" width="20" height="20" />__

</div>

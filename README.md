# 📱 E-commerce Mobile Store

A full-stack E-commerce application built with **ASP.NET Core 8.0**, featuring a distinct **Web API Backend** and an **MVC Frontend**. This project demonstrates a modern architecture for managing a mobile phone store inventory, users, and shopping experience.

## 🚀 Key Features

*   **Product Management:** Add, edit, remove, and list mobile devices.
*   **Inventory Control:** Track stock levels, pricing, and launch dates.
*   **User Management:** Role-based access control (Admin/User).
*   **Shopping Cart:** Add items to cart, manage quantities, and view order summaries.
*   **Category Management:** Organize products into specific categories.
*   **Modern UI:** Responsive design with a custom purple gradient theme, glassmorphism effects, and dynamic icons.
*   **Secure Authentication:** User login and session management.

## 🛠️ Technology Stack

*   **Backend:** ASP.NET Core Web API (.NET 8.0)
*   **Frontend:** ASP.NET Core MVC (.NET 8.0)
*   **Database:** SQL Server (via Entity Framework Core)
*   **Styling:** Bootstrap 5, Custom CSS, Font Awesome
*   **Scripting:** jQuery, SweetAlert2 for notifications

## 📂 Project Structure

The repository is organized into two main solutions:

1.  **`E-commerce_MobileStore/` (Backend)**
    *   RESTful API endpoints for Users, Products, Categories, Cart, etc.
    *   Handles business logic and database interactions.
    *   Swagger UI integration for API documentation.

2.  **`E-commerce_Mobile/` (Frontend)**
    *   MVC Web Application.
    *   Consumes the Backend API.
    *   Provides the user interface and experience.

## ⚙️ Prerequisites

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)
*   Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/keval2310/Mobile-Store.git
cd Mobile-Store
```

### 2. Setup Database & Backend
Navigate to the backend directory and update the connection string in `appsettings.json` if necessary.

```bash
cd E-commerce_MobileStore
# Run database migrations (if applicable)
dotnet ef database update
# Start the API
dotnet run
```
*The Backend API usually runs on `http://localhost:5001` or `https://localhost:7001`.*

### 3. Setup & Run Frontend
Open a new terminal and navigate to the frontend directory:

```bash
cd E-commerce_Mobile
# Start the MVC App
dotnet run
```
*The Frontend App usually runs on `http://localhost:5000` or `https://localhost:7000`.*

## 📸 Usage

1.  **Login:** Access the application and log in with your credentials.
2.  **Explore:** Browse the catalog of mobile phones.
3.  **Manage:**
    *   Go to **Products** to manage inventory (requires Admin/Manager role).
    *   Go to **Categories** to organize items.
4.  **Shop:** Add items to your **Cart** and review them.

## 🎨 UI & Design

The application features a consistent **Purple Gradient Theme** (`#667eea` → `#764ba2`) across all major headers and components, ensuring a professional and branded look.
- **Product Images:** Displays actual product photos for specific models (e.g., Vivo V27, V29) with auto-fallback to stylized icons.
- **Interactive Elements:** Hover effects, animated transitions, and mobile-responsive layouts.



# FixItNow 🛠️

A full-featured **service-providing and booking platform** built with **ASP.NET Core 8.0**, where both technical and non-technical professionals can offer services and users can easily browse, book, and manage them.

---

## 📌 Project Information

* **Project Name:** FixItNow
* **Framework:** ASP.NET Core 8.0
* **Architecture:** MVC + Identity + EF Core

🔗 **GitHub Repository:**
[https://github.com/TahaKhan192004/FixItNow](https://github.com/TahaKhan192004/FixItNow)

📄 **Project Documentation:**
[https://docs.google.com/document/d/1nMZWUEOPICtSC6OT1K-xdNL6CRMq1rcQUeA2YyqARQc/edit](https://docs.google.com/document/d/1nMZWUEOPICtSC6OT1K-xdNL6CRMq1rcQUeA2YyqARQc/edit)

---

## 🧠 Project Overview

**FixItNow** is a service-booking platform designed to connect users with skilled professionals such as:

* Plumbers
* Electricians
* Carpenters
* Developers
* Other technical & non-technical service providers

The platform allows users to **search, filter, book, review, and communicate** with service providers, while providers and admins get dedicated panels to manage services and platform activity.

---

## ✨ Core Features

### 1️⃣ Service Browsing & Booking

* Browse services by category
* View service details, pricing, and reviews
* Book services with date validation

### 2️⃣ User & Provider Profiles

**Users can:**

* Register and manage their profiles
* Save favorite services
* Book services and view history
* Leave reviews and ratings

**Service Providers can:**

* Create professional profiles
* Add, edit, or delete services
* Manage bookings and user queries
* View analytics from provider dashboard

### 3️⃣ Admin Management Panel

* Approve or reject service providers
* Manage users and providers
* View platform-wide analytics
* Monitor bookings and activity

### 4️⃣ Advanced Search & Filters

* Category-based filtering
* Sort services by price
* AJAX-powered real-time search (no page reloads)

### 5️⃣ Role-Based Authentication

* ASP.NET Core Identity
* Separate roles:

  * Admin
  * User
  * Service Provider
* Role-specific access and functionality

### 6️⃣ Cart & Booking Management

* Add services to cart before booking
* View past and current bookings
* Review completed services

### 7️⃣ Messaging System

* Secure inbox for users and providers
* Direct communication between both parties

### 8️⃣ User Engagement Features

* Save services to favorites
* Post reviews and suggestions
* Discover related services

---

## 🚀 Enhancements & Technical Highlights

* 🔐 **ASP.NET Core Identity**

  * Secure authentication
  * Google Login support
  * Password recovery

* ⚡ **AJAX Integration**

  * Dynamic filtering
  * Live search
  * Messaging without page reloads

* 🛡️ **Claims & Policies**

  * Fine-grained authorization
  * Policy-based access control

* 🧩 **Layouts & Partial Views**

  * Reusable UI components
  * Consistent design across pages

* 🔗 **Dependency Injection**

  * Clean, maintainable, loosely-coupled architecture

* 🗄️ **Entity Framework Core**

  * ORM-based data handling
  * LINQ queries
  * Migrations support

---

## 🛠️ Technologies Used

| Technology            | Purpose                        |
| --------------------- | ------------------------------ |
| HTML                  | Page structure                 |
| CSS                   | Styling and layout             |
| Bootstrap             | Responsive UI components       |
| JavaScript            | Client-side interactivity      |
| AJAX                  | Real-time updates              |
| ASP.NET Core 8.0      | Backend logic                  |
| ASP.NET Core Identity | Authentication & authorization |
| Entity Framework Core | Database ORM                   |
| Microsoft SQL Server  | Database                       |

---

## 🖥️ Screens Overview

* **Homepage:** Navbar, reviews, best-selling services, footer
* **FAQ Page:** Frequently asked questions
* **About Us:** Team, introduction, achievements
* **Contact Page:** Contact form (working hours only)
* **Authentication Pages:**

  * Sign In
  * User Registration
  * Provider Registration

### User Side

* Search & filter services
* Service details page with reviews
* Messaging inbox
* Booking form with validations
* Cart with bookings & favorites

### Provider Side

* Provider dashboard
* Add / Edit / Delete services
* View bookings & queries
* Analytics overview

### Admin Side

* Admin dashboard
* Provider approval system
* User & booking management
* Platform analytics

---

## ⚙️ Setup & Installation

### Prerequisites

* .NET SDK 8.0
* Microsoft SQL Server
* Visual Studio 2022 or later

### Steps to Run

1. **Clone the repository**

```bash
git clone https://github.com/TahaKhan192004/FixItNow.git
```

2. **Open the solution in Visual Studio**

3. **Configure the database connection** in `appsettings.json`

4. **Apply EF Core migrations**

```bash
dotnet ef database update
```

5. **Run the project**

```bash
dotnet run
```

---

## 📌 Conclusion

FixItNow is a complete, scalable, and secure service-booking platform showcasing modern **ASP.NET Core web development practices**, including authentication, role management, AJAX interactions, and clean architecture. It effectively bridges the gap between service providers and customers through a user-friendly and robust system.

---

⭐ If you like this project, feel free to star the repository and explore the code!

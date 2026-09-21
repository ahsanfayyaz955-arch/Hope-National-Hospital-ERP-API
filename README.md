# 🏥 Hospital ERP System

A modern **Hospital Enterprise Resource Planning (ERP)** system designed to digitize and simplify hospital operations through a centralized web-based platform.

The system provides dedicated functionality for managing **patients, doctors, appointments, medical information, hospital users, and administrative operations** while maintaining a secure and scalable architecture.

## ✨ Overview

The Hospital ERP is built to provide healthcare organizations with a centralized platform for managing day-to-day hospital workflows.

Instead of maintaining patient and operational information across separate systems, the platform brings important hospital processes together into one application.

### 🎯 Main Goals

* Digitize hospital management workflows
* Centralize patient and medical information
* Simplify appointment management
* Improve communication between hospital staff
* Provide secure role-based access
* Create a scalable foundation for future healthcare features
* Provide administrators with centralized control over hospital operations

## 🚀 Key Features

### 👨‍⚕️ Doctor Management

* Doctor registration and management
* Doctor profiles
* Specialty management
* Doctor availability
* Doctor-related patient workflows

### 🧑‍🤝‍🧑 Patient Management

* Patient registration
* Patient profiles
* Patient information management
* Medical-related patient data
* Patient history and records

### 📅 Appointment Management

* Appointment scheduling
* Doctor availability
* Patient appointment workflows
* Appointment status management

### 🔐 Authentication & Authorization

* Secure user authentication
* JWT-based authorization
* Role-based access control
* Protected API endpoints
* Administrative access management

### 👨‍💼 Administration

* Centralized administration
* User management
* Hospital system management
* Dashboard-oriented administrative workflows

## 🏗️ Architecture

The backend follows a **layered / clean architecture approach** to keep business logic, data access, and API responsibilities separated.

```text
Hospital ERP
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Business Models
│
├── Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   └── Business Logic
│
├── Infrastructure
│   ├── Entity Framework Core
│   ├── Database
│   ├── Repositories
│   └── Authentication
│
├── API
│   ├── Controllers
│   ├── Middleware
│   ├── Swagger
│   └── Configuration
│
└── React Frontend
    ├── Pages
    ├── Components
    ├── Services
    ├── Authentication
    └── Dashboard
```

## 🛠️ Technology Stack

### Backend

* **C#**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **JWT Authentication**
* **Swagger / OpenAPI**

### Frontend

* **React.js**
* **JavaScript**
* **Axios**
* **Responsive UI**

### Development Tools

* Visual Studio
* Visual Studio Code
* Git & GitHub
* SQL Server / SQL Server Management Studio

## 🔄 Application Flow

```text
React Frontend
       │
       ▼
ASP.NET Core Web API
       │
       ▼
Application Services
       │
       ▼
Entity Framework Core
       │
       ▼
SQL Server Database
```

Authentication is handled through JWT tokens, allowing protected resources and role-based access throughout the application.

📌 Project Status

🚧 **Active Development**

The project is continuously being improved with additional healthcare workflows, frontend functionality, validation, security improvements, and administrative features.

🔮 Future Improvements

* Advanced hospital dashboard
* Billing and payment management
* Prescription management
* Pharmacy management
* Laboratory management
* Medical reports
* Notifications
* Advanced reporting and analytics
* Audit logging
* Deployment and CI/CD
* Enhanced responsive UI

🤝 Contribution

Contributions, suggestions, and improvements are welcome.

For development, create a feature branch, implement the changes, and submit a pull request for review.

 👨‍💻 Author

Ahsan Fayyaz

Junior ASP.NET Core Developer
C# • ASP.NET Core • EF Core • SQL Server • React



⭐ If you find this project useful, consider giving the repository a star.

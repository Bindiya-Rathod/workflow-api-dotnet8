# WorkFlow API - Professional Task Management System

A production-ready RESTful API built with .NET 8, implementing Clean Architecture, JWT Authentication, and industry best practices.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server)

## 📌 Project Overview

WorkFlow API is a full-featured task management system demonstrating enterprise-level .NET development practices. Built as a learning project showcasing modern backend development skills.

**Key Highlights:**
- Clean Architecture implementation
- JWT-based authentication & authorization
- Repository pattern with EF Core
- Global exception handling
- Comprehensive data validation
- RESTful API design principles
- Swagger/OpenAPI documentation

## 🛠️ Tech Stack

**Backend Framework:**
- .NET 8 (LTS)
- ASP.NET Core Web API
- C# 12

**Database:**
- Entity Framework Core 8
- SQL Server 2019
- Code-First Migrations

**Security:**
- JWT (JSON Web Tokens)
- BCrypt.Net for password hashing

**Tools & Libraries:**
- Swagger/OpenAPI
- FluentValidation (planned)
- AutoMapper (planned)

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:
```
WorkFlow/
├── WorkFlow.API/                 # Presentation Layer
│   ├── Controllers/              # API endpoints
│   ├── Middleware/               # Custom middleware (exception handling)
│   └── Program.cs                # App configuration
│
├── WorkFlow.Application/         # Application Layer
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Services/                 # Business logic services
│   ├── Helpers/                  # Utility classes
│   ├── Settings/                 # Configuration models
│   └── Exceptions/               # Custom exceptions
│
├── WorkFlow.Domain/              # Domain Layer
│   ├── Entities/                 # Domain models
│   ├── Enums/                    # Enumerations
│   └── Interfaces/               # Repository contracts
│
└── WorkFlow.Infrastructure/      # Infrastructure Layer
    ├── Data/                     # DbContext
    ├── Repositories/             # Repository implementations
    └── Configurations/           # Entity configurations
```

**Design Patterns Implemented:**
- Repository Pattern
- Dependency Injection
- Options Pattern
- Middleware Pattern

## 📝 API Endpoints

### 🔐 Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | ❌ |
| POST | `/api/auth/login` | Login & get JWT token | ❌ |
| GET | `/api/auth/profile` | Get current user profile | ✅ |

### 📋 Task Management Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/tasks` | Get all user's tasks | ✅ |
| GET | `/api/tasks/{id}` | Get specific task | ✅ |
| POST | `/api/tasks` | Create new task | ✅ |
| PUT | `/api/tasks/{id}` | Update task | ✅ |
| DELETE | `/api/tasks/{id}` | Delete task | ✅ |

**🔒 Protected endpoints require a JWT token in the Authorization header:**
```
Authorization: Bearer <your-jwt-token>
```

## 💾 Database Schema

### Users Table
- `Id` (int, PK)
- `Name` (nvarchar(100))
- `Email` (nvarchar(100), unique)
- `PasswordHash` (nvarchar(max))
- `CreatedDate` (datetime2)

### TaskItems Table
- `Id` (int, PK)
- `Title` (nvarchar(200))
- `Description` (nvarchar(1000))
- `Status` (int) - Pending(1), InProgress(2), Completed(3)
- `Priority` (int) - Low(1), Medium(2), High(3)
- `DueDate` (datetime2, nullable)
- `CreatedDate` (datetime2)
- `UpdatedDate` (datetime2, nullable)
- `UserId` (int, FK)

**Relationships:**
- One User → Many Tasks (CASCADE DELETE)

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
- [Postman](https://www.postman.com/) (for API testing)

### Installation & Setup

1. **Clone the repository**
```bash
git clone https://github.com/Bindiya-Rathod/workflow-api-dotnet8.git
cd workflow-api-dotnet8
```

2. **Update Database Connection String**

Open `WorkFlow.API/appsettings.json` and update:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=WorkFlowDB;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

3. **Apply Database Migrations**

Open Package Manager Console in Visual Studio:
```powershell
# Set WorkFlow.Infrastructure as default project
Update-Database
```

Or using .NET CLI:
```bash
dotnet ef database update --project WorkFlow.Infrastructure --startup-project WorkFlow.API
```

4. **Run the Application**
```bash
cd WorkFlow.API
dotnet run
```

5. **Access Swagger UI**

Open browser: `https://localhost:7XXX/swagger`

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to `https://localhost:7XXX/swagger`
2. Register a new user via `/api/auth/register`
3. Login via `/api/auth/login` and copy the token
4. Click "Authorize" button (🔓) at the top
5. Enter: `Bearer YOUR_TOKEN_HERE`
6. Now you can test all protected endpoints!

### Using Postman

Import the Postman collection (if provided) or:

**1. Register User:**
```http
POST https://localhost:7XXX/api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**2. Login:**
```http
POST https://localhost:7XXX/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**3. Create Task (with token):**
```http
POST https://localhost:7XXX/api/tasks
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "title": "Complete Project Documentation",
  "description": "Write comprehensive README",
  "priority": 2,
  "dueDate": "2025-02-01T18:00:00Z"
}
```

## 🔒 Security Features

- ✅ Passwords hashed using BCrypt (salt rounds: 10)
- ✅ JWT tokens with configurable expiry (default: 60 minutes)
- ✅ Claims-based authorization
- ✅ User isolation (users can only access their own tasks)
- ✅ HTTPS enforcement
- ✅ CORS configuration
- ✅ SQL injection protection (EF Core parameterized queries)

## ✨ Key Features Implemented

### 1. Clean Architecture
- Clear separation of concerns
- Domain-centric design
- Dependency inversion
- Testable code structure

### 2. Repository Pattern
- Generic repository for common operations
- Specific repositories for entity-specific queries
- Abstraction over data access

### 3. Global Exception Handling
- Centralized error handling middleware
- Custom exception types
- Consistent error responses
- Environment-aware error details

### 4. Data Validation
- DTO-level validation using Data Annotations
- Model state validation
- Business rule validation

### 5. JWT Authentication
- Stateless authentication
- Token-based authorization
- Secure claim-based identity

## 📊 Project Statistics

- **Lines of Code:** ~2,500+
- **Controllers:** 2 (Auth, Tasks)
- **Entities:** 2 (User, TaskItem)
- **DTOs:** 7
- **Repositories:** 3 (Generic + 2 specific)
- **Endpoints:** 8
- **Development Time:** 5 days

## 🎯 Learning Outcomes

Through this project, I've demonstrated proficiency in:

✅ .NET 8 Web API development  
✅ Clean Architecture implementation  
✅ Entity Framework Core (Code-First)  
✅ JWT Authentication & Authorization  
✅ Repository Pattern  
✅ Dependency Injection  
✅ RESTful API design  
✅ Database design & migrations  
✅ Error handling & logging  
✅ API documentation with Swagger  
✅ Git version control  

## 🚀 Future Enhancements

- [ ] Add FluentValidation for complex validation rules
- [ ] Implement AutoMapper for DTO mappings
- [ ] Add Unit Tests (xUnit + Moq)
- [ ] Add Integration Tests
- [ ] Implement Serilog for structured logging
- [ ] Add API versioning
- [ ] Implement refresh tokens
- [ ] Add role-based authorization
- [ ] Deploy to Azure (App Service + Azure SQL)
- [ ] Add Angular frontend
- [ ] Implement SignalR for real-time updates
- [ ] Add caching (Redis)
- [ ] Implement pagination & filtering

## 📅 Development Timeline

| Day | Focus Area | Achievements |
|-----|-----------|-------------|
| Day 1 | Project Setup | Clean Architecture structure, Domain entities |
| Day 2 | Database Layer | EF Core setup, Migrations, Repository Pattern |
| Day 3 | API Layer | Controllers, DTOs, CRUD operations, Validation |
| Day 4 | Authentication | JWT implementation, Login/Register, Protected routes |
| Day 5 | Polish & Deploy | Exception handling, Logging, Documentation |

## 📄 License

This project is for educational and portfolio purposes.

---

**⭐ If you find this project helpful, please give it a star!**

**Built with ❤️ as part of my .NET learning journey - January 2026**

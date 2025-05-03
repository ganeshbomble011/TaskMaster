# TaskMaster - Task Management System Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Database Design](#database-design)
4. [API Endpoints](#api-endpoints)
5. [Features](#features)
6. [Technical Stack](#technical-stack)
7. [Setup Guide](#setup-guide)

## Project Overview
TaskMaster is a robust task management system built using ASP.NET Core with Clean Architecture principles. The system allows users to manage their tasks efficiently with features like task creation, assignment, tracking, and notifications.

## Architecture
The project follows Clean Architecture principles with the following layers:

### 1. Core (Domain) Layer
- Contains business entities and interfaces
- Independent of other layers
- Defines the business rules

### 2. Application Layer
- Implements business logic
- Contains DTOs, interfaces, and services
- Orchestrates the flow of data

### 3. Infrastructure Layer
- Implements interfaces defined in Core layer
- Handles data access using ADO.NET
- Manages external services (email, file storage)

### 4. Presentation Layer
- ASP.NET Core Web API
- Handles HTTP requests
- Implements authentication and authorization

## Database Design

### Tables

#### Users
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
```

#### Tasks
```sql
CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DueDate DATETIME NULL,
    Priority INT NOT NULL, -- 0=Low, 1=Med, 2=High
    IsCompleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
```

#### Attachments
```sql
CREATE TABLE Attachments (
    AttachmentId INT PRIMARY KEY IDENTITY,
    TaskId INT NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId)
);
```

#### Notifications
```sql
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
```

## API Endpoints

### Authentication
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token

### Tasks
- GET /api/tasks
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}
- GET /api/tasks/user/{userId}
- GET /api/tasks/filter

### Attachments
- POST /api/tasks/{taskId}/attachments
- GET /api/tasks/{taskId}/attachments
- DELETE /api/tasks/{taskId}/attachments/{attachmentId}

### Notifications
- GET /api/notifications
- PUT /api/notifications/{id}/read
- DELETE /api/notifications/{id}

## Features

### 1. User Management
- User registration and login
- JWT-based authentication
- Password hashing and security
- User profile management

### 2. Task Management
- Create, read, update, delete tasks
- Assign due dates
- Set task priorities
- Mark tasks as complete/incomplete
- Task filtering and sorting

### 3. File Attachments
- Upload files to tasks
- Download attachments
- Delete attachments
- File type validation

### 4. Notifications
- Email notifications for task deadlines
- In-app notifications
- Notification preferences

### 5. Reporting
- Task completion statistics
- User activity reports
- Due date reports

## Technical Stack

### Backend
- ASP.NET Core 8.0
- ADO.NET for data access
- JWT Authentication
- Swagger/OpenAPI
- FluentValidation
- AutoMapper

### Database
- SQL Server
- Stored Procedures
- Indexes for performance

### Security
- JWT Token Authentication
- Password Hashing
- Role-based Authorization
- Input Validation

## Setup Guide

### Prerequisites
- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Database Setup
1. Create database named "Task_Master"
2. Run the SQL scripts to create tables
3. Update connection string in appsettings.json

### Application Setup
1. Clone the repository
2. Restore NuGet packages
3. Update connection string
4. Run the application

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-VJ5P1HT;Database=Task_Master;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Development Guidelines

### Code Organization
- Follow Clean Architecture principles
- Use proper naming conventions
- Implement proper error handling
- Write unit tests

### Best Practices
- Use async/await for database operations
- Implement proper logging
- Follow SOLID principles
- Use dependency injection

### Security Considerations
- Validate all inputs
- Implement proper authentication
- Use parameterized queries
- Implement proper error handling

## Future Enhancements
1. Real-time notifications using SignalR
2. Mobile app development
3. Advanced reporting features
4. Task templates
5. Team collaboration features
6. Calendar integration
7. Email integration
8. API rate limiting
9. Caching implementation
10. Performance optimization

## Support and Maintenance
- Regular security updates
- Performance monitoring
- Database maintenance
- Backup procedures
- Documentation updates 
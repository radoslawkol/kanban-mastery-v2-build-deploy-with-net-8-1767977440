# Kanban Board Application

A modern, full-stack collaborative task management application built with **React** (frontend) and **.NET** (backend). Organize your work efficiently with intuitive kanban boards featuring real-time task management, team collaboration, and role-based access control.

## 🌐 Demo

[Your Demo Link Here](http://replace-with-your-link.com)

**Demo Credentials (If applicable):**

- **Username:** alek@example.pl
- **Password:** 12345678Ab!

## 📖 About this Software

This Kanban Board application provides teams with a powerful yet intuitive platform to visualize, organize, and manage their work. Whether you're managing software development sprints, marketing campaigns, or any collaborative project, this application streamlines task organization with drag-and-drop functionality, real-time updates, and comprehensive access control.

The application is built with modern web technologies, following best practices for security, performance, and user experience. It supports multi-user collaboration with role-based permissions (Board Owner, Board Member) and maintains data integrity through robust backend validation and database constraints.

### Key Features:

1. **Board Management:** Create and manage multiple kanban boards with customizable workflows. Each board serves as a container for organizing related tasks and team collaboration.

2. **Drag-and-Drop Interface:** Intuitive drag-and-drop functionality powered by react-beautiful-dnd for seamless task movement between columns. Users can instantly reorganize priorities and workflow states.

3. **Columns & Cards:** Organize work into customizable columns representing different workflow stages (e.g., "To Do", "In Progress", "Done"). Create, update, and manage individual task cards within each column.

4. **Team Collaboration:** Invite team members to boards and assign tasks. Assign multiple team members to individual cards for better collaboration and accountability.

5. **Role-Based Access Control:** Manage board permissions with two primary roles:
    - **Board Owner:** Full control over board settings, member management, and all content
    - **Board Member:** Can view and manage cards and columns within their boards

6. **User Authentication & Authorization:** Secure authentication system with token-based API endpoints. Custom authorization handlers ensure role-based access to protected resources.

7. **Responsive Design:** Beautiful, responsive UI built with React and TailwindCSS that works seamlessly across desktop, tablet, and mobile devices.

## 🏗️ Tech Stack

### Frontend

- **Framework:** React 19 with TypeScript
- **Build Tool:** Vite
- **Styling:** TailwindCSS
- **State Management:** React Query (@tanstack/react-query)
- **Routing:** React Router v7
- **Form Handling:** React Hook Form with Zod validation
- **Drag & Drop:** @hello-pangea/dnd
- **HTTP Client:** Axios
- **Notifications:** React Toastify
- **Modal Management:** React Modal

### Backend

- **Framework:** ASP.NET Core Minimal API
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Authorization:** Custom policy-based authorization handlers

## 📁 Project Structure

```
Kanban/
├── KanbanAPI/                 # Backend (.NET API)
│   ├── Models/               # Database models (Board, Card, Column, User)
│   ├── Data/                 # Entity Framework DbContext
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Endpoints/            # API endpoints (BoardEndpoints, CardEndpoints, etc.)
│   ├── Services/             # Business logic services
│   ├── Authorization/        # Custom authorization handlers & policies
│   ├── Migrations/           # Entity Framework migrations
│   ├── Exceptions/           # Custom exception classes
│   └── Program.cs            # Application configuration
├── kanban-web/               # Frontend (React + TypeScript)
│   ├── src/
│   │   ├── components/       # Reusable React components
│   │   ├── pages/            # Page components
│   │   ├── services/         # API service layer
│   │   ├── hooks/            # Custom React hooks
│   │   ├── types/            # TypeScript type definitions
│   │   ├── lib/              # Utility functions
│   │   └── App.tsx           # Root component
│   └── vite.config.ts        # Vite configuration
├── KanbanAPI.Tests/          # Unit tests for backend
└── README.md                 # Documentation
```

## 🖼️ API Endpoints

### Authentication

- `POST /api/identity/login` - User login
- `POST /api/identity/register` - User registration
- `POST /api/identity/logout` - User logout

### Boards

- `GET /api/boards` - Get all user's boards
- `POST /api/boards` - Create new board
- `GET /api/boards/{id}` - Get board details
- `PUT /api/boards/{id}` - Update board
- `DELETE /api/boards/{id}` - Delete board
- `POST /api/boards/{id}/members` - Add board member
- `DELETE /api/boards/{id}/members/{userId}` - Remove board member

### Columns

- `GET /api/columns/{boardId}` - Get columns for board
- `POST /api/columns` - Create column
- `PUT /api/columns/{id}` - Update column
- `DELETE /api/columns/{id}` - Delete column

### Cards

- `GET /api/cards/{columnId}` - Get cards in column
- `POST /api/cards` - Create card
- `PUT /api/cards/{id}` - Update card
- `DELETE /api/cards/{id}` - Delete card
- `POST /api/cards/{id}/assign` - Assign user to card

## 📊 Test Coverage

View the latest coverage report in the `coveragereport/` directory. Generate a new report with:

```bash
cd KanbanAPI.Tests
dotnet test /p:CollectCoverage=true
```

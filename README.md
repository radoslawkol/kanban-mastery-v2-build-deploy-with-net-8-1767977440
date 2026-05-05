# Kanban Board Application

A modern, full-stack collaborative task management application built with **React** (frontend) and **.NET** (backend). Organize your work efficiently with intuitive kanban boards featuring real-time task management, team collaboration, and role-based access control.

## 🌐 Demo

[Your Demo Link Here](http://replace-with-your-link.com)

**Demo Credentials:**

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

## Screenshots
#### Register form
<img width="1494" height="919" alt="image" src="https://github.com/user-attachments/assets/67dace2a-1372-4de0-9ecb-e4cc7f36a807" />

#### Login form
<img width="1496" height="889" alt="image" src="https://github.com/user-attachments/assets/ab3b4340-2458-4cf0-ae67-eab4500c1c3f" />

#### Dashboard Page
<img width="1499" height="483" alt="image" src="https://github.com/user-attachments/assets/d52dcbae-4ead-4a6f-b419-20bdb65a3c13" />

##### Kanban Board Page
<img width="1186" height="596" alt="image" src="https://github.com/user-attachments/assets/e6bd1080-7fc2-4ecb-877c-603cb78c5933" />

#### Invite Board Members Modal
<img width="1497" height="738" alt="image" src="https://github.com/user-attachments/assets/0bfb6005-8365-4b0d-a386-d6e791fcd8a5" />

#### Add New Column
<img width="1492" height="696" alt="image" src="https://github.com/user-attachments/assets/f08c2d9d-a819-4dc2-bff0-3da322e88772" />

#### Delete Confirmation Modal
<img width="1507" height="731" alt="image" src="https://github.com/user-attachments/assets/c90e63f2-25e3-4cf6-8ac1-0757126a992c" />


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

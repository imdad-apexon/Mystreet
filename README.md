# MyStreeT Capstone

A full-stack sneaker shopping and order management app built with:

- Frontend: React + Vite + TypeScript
- State: React Hooks + Context API
- Routing: React Router
- API: Axios
- Persistence: localStorage for cart and auth token
- Backend: ASP.NET Core Web API
- ORM: Entity Framework Core
- Database: SQLite for local development
- Authentication: JWT Bearer
- Password hashing: BCrypt.Net-Next
- Testing: xUnit, Moq, FluentAssertions

## Features

### User
- Browse products
- Filter products by brand and size
- View product details
- Add items to cart
- Cart persistence in localStorage
- Register, login, logout
- Checkout with mock payment method
- View order history and details

### Admin
- Add, edit, delete products
- Manage product stock
- View orders

## Project Structure

```text
backend/
frontend/
```

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm
- SQLite local database support (built in with EF Core provider)

## Backend Setup

### 1. Restore packages
```bash
cd backend/Mystreet
dotnet restore
```

### 2. Run database migrations
If you are using EF Core migrations, install the tool once:
```bash
dotnet tool install --global dotnet-ef
```

Then create and apply migrations:
```bash
cd backend/Mystreet
dotnet ef migrations add InitialCreate -p Mystreet.Infrastructure -s Mystreet.Api
dotnet ef database update -p Mystreet.Infrastructure -s Mystreet.Api
```

### 3. Run the API
```bash
cd backend/Mystreet
dotnet run --project Mystreet.Api
```

The API will run on the configured local URL in `launchSettings.json`.

### 4. Swagger
Open Swagger in the browser from the API URL to test endpoints.

## Backend Packages

### Mystreet.Api
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt
- Swashbuckle.AspNetCore

### Mystreet.Infrastructure
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Tools
- BCrypt.Net-Next
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt

### Mystreet.Tests
- Microsoft.NET.Test.Sdk
- xunit
- xunit.runner.visualstudio
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory
- coverlet.collector

## Frontend Setup

### 1. Install dependencies
```bash
cd frontend/mystreet-ui
npm install
```

### 2. Run the frontend
```bash
npm run dev
```

### 3. Build for production
```bash
npm run build
```

## Frontend Environment

Update the API base URL in:
```text
src/services/api.ts
```

Example:
```ts
baseURL: 'https://localhost:5001/api'
```

## Default Seed Data

The backend seeds:
- **Admin User**: `admin@mystreet.com` / `Admin@123` (IsAdmin: true)
- **Test Users**:
  - `john@mystreet.com` / `John@123`
  - `sarah@mystreet.com` / `Sarah@123`
  - `mike@mystreet.com` / `Mike@123`
  - `emma@mystreet.com` / `Emma@123`
- Sample products for testing

## Testing

Run backend tests:
```bash
cd backend/Mystreet
dotnet test Mystreet.Tests
```

Collect coverage:
```bash
dotnet test Mystreet.Tests --collect:"XPlat Code Coverage"
```

## Notes

- Cart is stored in localStorage.
- JWT token and logged-in user are also stored in localStorage.
- Admin access is controlled with the `IsAdmin` flag.
- Payment is mocked; no external gateway is required.
- SQLite is used for simple local setup, but SQL Server can be added later with provider changes.
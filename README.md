# Staff Management System

A small full-stack app to manage staff records: list, search, create, update, and delete staff via a REST API and a web UI.

- **Backend:** ASP.NET Core 8 Web API, Entity Framework Core, SQL Server (or Docker)
- **Frontend:** Next.js 16 with React 19

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20](https://nodejs.org/) and npm
- [Docker](https://www.docker.com/) (optional but easiest way to run SQL Server and the API together)

## Run the backend

### Option A — Docker (API + database)

From the `backend` folder:

```bash
docker compose up --build
```

- API: [http://localhost:8080](http://localhost:8080)
- Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)

### Option B — Local .NET

1. Run SQL Server locally with a database and credentials that match `backend/src/StaffManagement.API/appsettings.json` (or adjust the connection string). You can start only the database with Docker:

   ```bash
   cd backend
   docker compose up sqlserver db-init
   ```

2. Run the API:

   ```bash
   cd backend
   dotnet restore
   dotnet run --project src/StaffManagement.API/StaffManagement.API.csproj
   ```

- API: [http://localhost:5001](http://localhost:5001)
- Swagger: [http://localhost:5001/swagger](http://localhost:5001/swagger)

## Run the frontend

```bash
cd frontend
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000).

Point the UI at your API by setting `NEXT_PUBLIC_API_URL` (for example in `frontend/.env.local`):

- Docker API: `NEXT_PUBLIC_API_URL=http://localhost:8080/api`
- Local API: `NEXT_PUBLIC_API_URL=http://localhost:5001/api`

If you omit this, the app defaults to `http://localhost:5001/api`.

## Tests

Backend (from `backend`):

```bash
dotnet test
```

Frontend (from `frontend`):

```bash
npm run lint
npm run typecheck
npm run build
```

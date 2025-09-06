# OrderProcessingSystem

A simple order processing example built with ASP.NET Core (API) and a background worker service that processes order-placed messages from RabbitMQ. The repo contains:

- `OrderProcessingSystem.Api` — REST API (Products, Cart, Orders, Authentication).  
- `OrderProcessingSystem.OrderPlacedService` — background consumer (reads messages from RabbitMQ and sends email/log).  
- `docker-compose.yml` to run everything (API, consumer, SQL Server, RabbitMQ, Seq).

---

## Table of contents

- [Quick start (Docker)](#quick-start-docker)  
- [Prerequisites](#prerequisites)  
- [Ports & services](#ports--services)  
- [Environment / configuration](#environment--configuration)  
- [Run locally (dev)](#run-locally-dev)  
- [API overview (endpoints)](#api-overview-endpoints)  
- [Background worker](#background-worker)  
- [Database & migrations](#database--migrations)  
- [Notes & TODOs](#notes--todos)  
- [Contact / next steps](#contact--next-steps)

---

## Quick start (Docker)

1. From repository root run:

```bash
docker compose up --build
```

This will build and start the API service, the background consumer, RabbitMQ, SQL Server and Seq as defined in `docker-compose.yml`.

2. Open API swagger (when container healthy):  
   - API is exposed inside containers on ports `8080`/`8081` and mapped to host ports `5001`/`5002` in `docker-compose.yml`. Check `http://localhost:5001/swagger` (or `https://localhost:5002/swagger`) depending on your environment.

---

## Prerequisites

- Docker & Docker Compose (Docker Desktop or Docker Engine with Compose V2).  
- (Optional) .NET 8 SDK if you want to run projects locally without containers.

---

## Ports & services (defaults from docker-compose)

- API: host `5001` -> container `8080` (and `5002` -> `8081`).  
- Consumer worker: host `6001`/`6002` mapped to worker ports.  
- SQL Server: `1433` (container uses `mcr.microsoft.com/mssql/server:2022-latest`).  
- RabbitMQ management: `15672`, broker port: `5672`.  
- Seq (logging UI): `9090`.

---

## Environment / configuration

Configuration lives in each project `appsettings.json` and is also injected via `docker-compose.yml` environment variables.

Important example settings:

- Connection string (SQL Server) — set in API `appsettings.json` and overridden from docker-compose: `ConnectionStrings__DefaultConnection`.  
- JWT signing key (API): set in `OrderProcessingSystem.Api/appsettings.json` under `Jwt:Key`.  
- RabbitMQ config: `RabbitMQ:Host`, `RabbitMQ:User`, `RabbitMQ:Pass` (used by API and worker). Defaults point to the `rabbitmq` service in `docker-compose.yml`.  
- Email sender (consumer): `OrderProcessingSystem.OrderPlacedService/appsettings.json` contains `EmailSender` credentials used by the worker to send notification emails. Replace with secure values.

---

## Run locally (without Docker)

1. Ensure you have a SQL Server instance and RabbitMQ accessible.  
2. Update `OrderProcessingSystem.Api/appsettings.json` connection string and `RabbitMQ` settings to point to your services.  
3. From `OrderProcessingSystem.Api` run:

```bash
dotnet restore
dotnet build
dotnet run
```

4. For the worker service, run `OrderProcessingSystem.OrderPlacedService` similarly. The worker subscribes to `order_exchange` / `order_queue` and processes messages.

---

## API overview (main endpoints)

> Swagger is configured in `Program.cs` and exposes the routes. Authentication is JWT-based.

### Auth
- `POST /api/Account/Register` — register a user (body: `RegisterUserRequestDto`).  
- `POST /api/Account/Login` — login and receive JWT (body: `LoginUserRequestDto`).  

### Products (public / admin)
- `GET /api/Products` — list products.  
- `GET /api/Products/{id}` — get product by id.  
- `POST /api/Products` — (Admin) create product.  
- `PUT /api/Products/{id}` — (Admin) update product.  
- `DELETE /api/Products/{id}` — (Admin) delete product.  

### Cart (user / admin)
- `GET /api/User/cart` — get authenticated user's cart items.  
- `POST /api/User/cart` — add item to authenticated user's cart.  

### Orders
- `POST /api/User/order` — place order (uses user's cart items; sends message to RabbitMQ).  
- Admin endpoints (in `OrdersController`) provide the ability to CRUD orders.  

---

## Background worker / Message flow

- When an order is placed, the API constructs and sends an `OrderPlacedMessage` to RabbitMQ. The worker (`OrderProcessingSystem.OrderPlacedService`) consumes messages from `order_queue`, acknowledges them and sends an email (via `IEmailSender`) or logs the result.

---

## Database & Migrations

- EF Core migrations are present under `OrderProcessingSystem.Api/Migrations`. The initial migration creates `Users`, `Products`, `Orders`, `OrderItems`, `CartItems` tables with the indexes used by the app (unique cart item per user/product, unique order item per order/product).  

If running locally, you can apply migrations with:

```bash
cd OrderProcessingSystem.Api
dotnet ef database update
```

(Ensure `dotnet-ef` is installed and `appsettings.json` connection string points to your DB.)

---

## Notes & TODOs (from code)

- The `PlaceOrder` flow has comments indicating planned improvements: stock quantity checks, transactional behavior, batch add/remove of order items, decrementing product stock when order placed.  
- Secrets (SQL SA password, JWT key, email creds) are currently in `docker-compose.yml` and `appsettings.json`. **Do not** keep these in a public repo — move them to secrets or environment variables for production.

---

## Example curl flows

Register:

```bash
curl -X POST http://localhost:5001/api/Account/Register  -H "Content-Type: application/json"  -d '{"FirstName":"John","LastName":"Doe","UserName":"johnd","Email":"john@example.com","Password":"P@ssw0rd","ConfirmPassword":"P@ssw0rd"}'
```

Login:

```bash
curl -X POST http://localhost:5001/api/Account/Login  -H "Content-Type: application/json"  -d '{"Email":"john@example.com","Password":"P@ssw0rd"}'
```

Use the returned JWT in `Authorization: Bearer <token>` for protected endpoints (cart, place order, admin endpoints).

---

## Where to look in the repo (important files)

- `docker-compose.yml` — full service composition & envs.  
- `OrderProcessingSystem.Api/Program.cs` — app startup, JWT, DI, Swagger.  
- `OrderProcessingSystem.Api/Controllers/*` — controllers for Accounts, Products, Orders, Cart, User.  
- `OrderProcessingSystem.OrderPlacedService/Worker.cs` — RabbitMQ consumer logic.  
- `OrderProcessingSystem.Api/Migrations/*` — EF Core migrations.

---

## Next steps / Help I can provide

- Add a **Development** section (run with Visual Studio / launch profiles).  
- Implement the TODOs: transactional order placement and stock decrement.  
- Add Postman collection or example requests.  
- Sanitize secrets and add example `.env` and `.env.example`.

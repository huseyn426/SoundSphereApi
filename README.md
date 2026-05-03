# SoundSphere

A web platform for music listening and catalog management: a public MVC UI, REST API, admin panel, player, playlists, social features, and payments. A single ASP.NET Core 8 application combines server-rendered pages and a JSON API.

---

## Architecture

- **ASP.NET Core 8.0** — hosting, middleware, routing.
- **Two controller styles in one app:**
  - **MVC** — `Controller` + Razor views (`Views/`) for the UI (home, tracks, playlists, account, admin).
  - **Web API** — `[ApiController]` controllers under `api/[controller]` for integrations and client-side JS (fetch + JWT).
- **Data layer:** EF Core + generic repository (`IRepository<T>`), migrations in `Migrations/`.
- **Application layer:** services (`Services/Interfaces`, `Services/Implementations`), DTOs for API and forms.
- **Errors:** custom `ExceptionMiddleware` — consistent JSON responses for unhandled exceptions on the pipeline.

---

## Backend — technologies and packages

| Area | Technology |
|------|------------|
| Platform | **.NET 8**, **C#** (nullable reference types, implicit usings) |
| Web | **ASP.NET Core** — MVC, **Razor**, **Tag Helpers** |
| ORM | **Entity Framework Core 8.0.26** |
| Database | **Microsoft SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`) |
| Migrations | **EF Core Tools** / **Design** (for `dotnet ef` CLI) |
| Validation | **FluentValidation** 11.x + **FluentValidation.AspNetCore** |
| DTO mapping | **AutoMapper** 16.x |
| API docs | **Swashbuckle.AspNetCore** (Swagger / OpenAPI UI in Development) |
| OpenAPI | **Microsoft.AspNetCore.OpenApi** |
| API authentication | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`) — issuer, audience, signing key validation |
| Payments | **Stripe.net** — subscriptions and payment flows at the service layer |

Additional server-side pieces:

- **Distributed memory cache** + **sessions** (`AddSession`) — cookie `.SoundSphere.Session`, stores `UserId`, JWT copy, display name, and role for MVC.
- **Cookie policy** — SameSite Lax, aligned with session usage.
- **IHttpContextAccessor** — HTTP context access for helpers/services where needed.

---

## Security and identity

- **Passwords:** ASP.NET Core **PasswordHasher** for users stored in the database.
- **JWT:** custom token generation (`JwtSecurityToken`, claims: `NameIdentifier`, `Name`, `Email`, `Role`), settings from `Jwt:*` in configuration.
- **MVC login:** form posts credentials into **server session**; optional **session restore from JWT** in `localStorage` when the session cookie is missing.
- **API:** protected endpoints with `[Authorize]` and the Bearer scheme.

---

## Data and domain model

- Entities include users and roles, tracks, artists, albums, genres, playlists, listening history, likes, comments, subscriptions/payments (per project structure).
- **Bootstrap data:** `DbInitializer`, role seed, admin user, music catalog seed.
- **Schema versioning:** EF Core migrations targeting SQL Server.

---

## Frontend (MVC + static assets)

- **Razor (.cshtml)** — page markup, shared **Layout**, localization helpers (`UiText`) and language switching.
- **CSS:** main design in `wwwroot/css/site.css` (dark theme, grids, player UI, modals, track tables).
- **Vanilla JavaScript:**
  - audio player (queue, shuffle, repeat, volume, listening history via fetch);
  - client-side i18n;
  - live search with JSON;
  - playlist modal and toasts;
  - per-row track action menu;
  - **Service Worker** (`sw.js`) + **web app manifest** — PWA-oriented (static asset caching; navigation/HTML not served from stale cache in the current setup).
- **Fonts:** Google Fonts (Manrope) in the layout.

---

## API (overview)

- REST controllers for auth, tracks, albums, artists, genres, playlists, social, history, payments, admin, etc.
- Consistent response shapes via **DTOs** and wrappers such as `ApiResponse<T>`.
- Swagger UI in Development with **Bearer** security scheme documented.

---

## Development tooling

- **SDK:** .NET 8  
- **IDE:** Visual Studio / VS Code / Rider  
- **Version control:** Git  
- **Database:** SQL Server instance (connection string in `appsettings.json` or secrets)

---

## Getting started

1. Clone the repository.  
2. Set **ConnectionStrings:DefaultConnection** in `appsettings.json` (or User Secrets / environment variables).  
3. Apply migrations: `dotnet ef database update --project SoundSphereApi` (adjust path from your solution root if needed).  
4. Run: `dotnet run --project SoundSphereApi`.  
5. Open the HTTPS URL from `launchSettings.json` (port may vary).  
6. In Development, **Swagger** and **Swagger UI** are available.

**Default admin account** (if seeded): document the email/password you ship in your own README or remove secrets from public repos — rotate credentials if they were ever committed.

---

## Repository layout (high level)

- `SoundSphereApi/` — main web project  
  - `Controllers/` — MVC + `Api/`  
  - `Views/` — Razor templates  
  - `wwwroot/` — CSS, JS, PWA assets  
  - `Data/` — DbContext, seeds  
  - `Services/`, `Repositories/`, `DTOs/`, `Models/`, `Validators/`, `Middlewares/`  
- `SoundSphereApi.sln` — solution file

---


# FitBody App

He thong gom 3 phan: Backend API dung chung (.NET Core), Admin Web (Angular), va Mobile App (Flutter).

## Cau truc thu muc

```
backend/    API .NET Core (ASP.NET Core Web API + EF Core + PostgreSQL + SignalR)
database/   Ghi chu/scripts lien quan database (schema chinh nam trong backend/FitBodyApp.Infrastructure/Migrations)
frontend/   Admin Web Portal (Angular + Angular Material)
mobile/     Mobile App (Flutter) - dang trien khai
docs/       Dac ta ky thuat (kien truc, database schema, api reference)
```

## Yeu cau moi truong

- .NET SDK 8.0
- PostgreSQL 15+ (dang chay tai `localhost:5432`)
- Node.js 20 LTS + npm
- Angular CLI (`npm i -g @angular/cli`)
- Flutter SDK stable (cho phan mobile)

## 1. Backend API

```bash
cd backend
dotnet restore
dotnet ef database update --project FitBodyApp.Infrastructure --startup-project FitBodyApp.Api
dotnet run --project FitBodyApp.Api --urls "http://localhost:5080"
```

- Swagger UI: http://localhost:5080/swagger
- Connection string va Azure Storage connection string thuc te nam trong
  `backend/FitBodyApp.Api/appsettings.Development.json` (khong commit, tu tao theo mau bien moi truong cua ban):
  ```json
  {
    "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=fitbody_dev;Username=postgres;Password=..." },
    "Jwt": { "Issuer": "FitBodyApp", "Audience": "FitBodyApp.Clients", "Key": "...", "AccessTokenMinutes": 60, "RefreshTokenDays": 30 },
    "AzureStorage": { "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net" }
  }
  ```
- Khi chay o moi truong Development, `DbSeeder` tu dong:
  - Tao 1 tai khoan `super_admin` (email + mat khau in ra console log lan dau tien seed).
  - Seed du lieu mau: exercises, routines, meal plans, articles, videos, faqs.

## 2. Admin Web (Angular)

```bash
cd frontend
npm install
ng serve
```

- Truy cap: http://localhost:4200
- `src/environments/environment.development.ts` tro ve `http://localhost:5080/api` (dung khi `ng serve`).
- Dang nhap bang tai khoan `super_admin` da seed o buoc 1 (chi tai khoan role `moderator/admin/super_admin` moi vao duoc).

## 3. Mobile App (Flutter)

```bash
cd mobile
flutter pub get
dart run build_runner build --delete-conflicting-outputs   # sinh code retrofit + json_serializable
flutter run -d chrome --dart-define=API_BASE_URL=http://localhost:5080/api   # hoac -d <device_id> cho Android/iOS that
```

- Kien truc feature-first: `lib/core/` (network/Dio+interceptor JWT tu refresh, secure storage, config),
  `lib/features/<module>/{data,domain,presentation}` cho tung module (auth, workout, nutrition, content,
  community, notification, support, profile, admin).
- Mot app dung chung cho ca User va Admin/Moderator: sau khi dang nhap, neu `role != user` se hien them
  tab **Quan tri** (dashboard rut gon, kiem duyet noi dung, ho tro khach hang realtime qua SignalR,
  khoa/mo khoa nhanh 1 tai khoan). CRUD noi dung day du (exercises/routines/meal-plans/articles/videos/
  faqs/challenges) va doi role CHI lam tren Admin Web, khong xay dung lai tren mobile.
- Can Android SDK (Android Studio) hoac Xcode de build/chay tren thiet bi that; neu chua cai, dung
  `-d chrome` de chay thu tren web trong luc phat trien.
- Giao dien hien la Material co ban, se cap nhat theo Figma UI Kit sau.

## Tai khoan test

| Vai tro | Email | Ghi chu |
| --- | --- | --- |
| super_admin | admin@fitbody.dev | Mat khau in ra console khi seed lan dau, doi ngay sau khi dang nhap |
| user | (tu dang ky qua `/api/auth/register`) | Role mac dinh la `user` |

## Tai lieu

Xem thu muc `docs/` de biet chi tiet kien truc he thong, database schema, va API reference.

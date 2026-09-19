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
- Connection string thuc te nam trong `backend/FitBodyApp.Api/appsettings.Development.json` (khong commit, tu tao theo mau bien moi truong cua ban).
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
flutter run
```

(Se cap nhat chi tiet sau khi hoan tat trien khai.)

## Tai khoan test

| Vai tro | Email | Ghi chu |
| --- | --- | --- |
| super_admin | admin@fitbody.dev | Mat khau in ra console khi seed lan dau, doi ngay sau khi dang nhap |
| user | (tu dang ky qua `/api/auth/register`) | Role mac dinh la `user` |

## Tai lieu

Xem thu muc `docs/` de biet chi tiet kien truc he thong, database schema, va API reference.

# Database

Schema thuc te (bang, cot, index, khoa ngoai) duoc quan ly bang EF Core Code-First trong
`backend/FitBodyApp.Infrastructure/Persistence/` (cac lop `IEntityTypeConfiguration<T>`) va
`backend/FitBodyApp.Infrastructure/Migrations/`. Day la nguon du lieu chuan (source of truth) cho schema.

## Ap dung schema vao PostgreSQL cuc bo

```bash
cd backend
dotnet ef database update --project FitBodyApp.Infrastructure --startup-project FitBodyApp.Api
```

## Seed du lieu

Du lieu mau (tai khoan `super_admin`, exercises/routines/meal-plans/articles/videos/faqs mau) duoc seed
tu dong khi chay API o moi truong Development, xem `backend/FitBodyApp.Infrastructure/Persistence/DbSeeder.cs`.

Tham khao chi tiet toan bo bang/cot trong `docs/02-database-schema.md`.

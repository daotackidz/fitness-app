# FitBody App — Hướng dẫn triển khai cho Claude Code

Đây là bộ tài liệu đặc tả để Claude Code (chạy trong VS Code) đọc và triển khai dự án FitBody App. Đặt cả 4 file này ở gốc repo (hoặc trong thư mục `docs/`) rồi mở Claude Code, yêu cầu: *"Đọc các file trong docs/ và thực hiện theo hướng dẫn ở 00-CLAUDE-CODE-INSTRUCTIONS.md"*.

## Tài liệu liên quan (đọc theo thứ tự)

1. `01-tong-quan-kien-truc.md` — stack công nghệ, chuẩn API chung, nguyên tắc RESTful.
2. `02-database-schema.md` — toàn bộ bảng, cột, kiểu dữ liệu, khoá chính/ngoại, chỉ mục (index).
3. `03-api-reference.md` — danh sách đầy đủ endpoint REST theo từng module.

## Stack bắt buộc

- **Backend:** .NET Core (ASP.NET Core Web API, C#), Entity Framework Core (Code First), PostgreSQL, Redis, SignalR cho chat realtime.
- **App:** Flutter (Dart), kiến trúc gợi ý: Clean Architecture hoặc Feature-first (`lib/features/<module>/{data,domain,presentation}`), state management: Riverpod hoặc Bloc, gọi API bằng `dio` + `retrofit`.

## Việc cần làm (thực hiện tuần tự, mỗi bước tạo commit riêng)

### A. Backend (.NET Core)

1. **Khởi tạo solution**
   - `dotnet new sln -n FitBodyApp`
   - Tạo các project: `FitBodyApp.Api` (Web API), `FitBodyApp.Domain` (entities), `FitBodyApp.Infrastructure` (EF Core, repository), `FitBodyApp.Application` (use case/service, DTO, validation).
2. **Entities & DbContext**
   - Dựa theo `02-database-schema.md`, tạo entity class C# cho từng bảng (namespace `FitBodyApp.Domain.Entities`), đúng tên cột, kiểu dữ liệu, quan hệ FK, ENUM → C# `enum`.
   - Tạo `FitBodyDbContext` (EF Core), cấu hình từng entity bằng `IEntityTypeConfiguration<T>` (Fluent API) — khai báo đầy đủ **index** đúng như liệt kê trong tài liệu (dùng `.HasIndex(...)`, `IsUnique()` cho các UNIQUE index, composite index dùng `.HasIndex(x => new { x.UserId, x.LogDate })`).
   - Tạo migration đầu tiên: `dotnet ef migrations add InitialCreate`.
3. **Cấu trúc theo module**, mỗi module gồm Controller + Service + DTO + Validator (FluentValidation), theo đúng nhóm trong `03-api-reference.md`:
   - `Auth` (JWT issuing, refresh token, social login, biometric login, forgot/reset password)
   - `Users` (profile, settings, avatar upload, password)
   - `Workout` (exercises, routines, routine_exercises, workout_logs, progress_tracking, recommendations)
   - `Nutrition` (meal_plans, meals, food_items)
   - `Content` (articles, videos, favorites)
   - `Community` (forum_posts, post_likes, comments, challenges, challenge_participants)
   - `Notification` (notifications, FCM integration)
   - `Support` (faqs, support_tickets, support_messages, SignalR hub `/hubs/support/{ticketId}`)
4. **Chuẩn hoá theo `01-tong-quan-kien-truc.md`:**
   - Response wrapper `{ success, data, meta, error }` dùng chung 1 middleware/filter.
   - Global exception handler map lỗi sang bảng mã lỗi chuẩn (`VALIDATION_ERROR`, `UNAUTHORIZED`...).
   - JWT middleware xác thực Bearer token; refresh token lưu hash trong bảng `refresh_tokens`.
   - Rate limiting qua Redis (`AspNetCoreRateLimit` hoặc middleware tự viết).
   - Áp dụng đúng verb/path RESTful đã liệt kê — **không tự đổi tên path**.
5. **Swagger/OpenAPI:** bật `Swashbuckle.AspNetCore`, sinh tài liệu API tự động khớp với `03-api-reference.md` để đối chiếu.
6. **Seed data** tối thiểu: vài `exercises`, `routines` mẫu theo 3 cấp độ, vài `articles`/`videos`, `faqs` để test app.

### B. App (Flutter)

1. `flutter create fitbody_app`, cấu hình `pubspec.yaml`: `dio`, `retrofit`, `riverpod` (hoặc `flutter_bloc`), `flutter_secure_storage` (lưu token), `firebase_messaging`, `signalr_netcore` (hoặc `signalr_core`).
2. Tạo cấu trúc `lib/features/<module>` khớp với các module backend ở trên (auth, onboarding/profile-setup, home, workout, nutrition, content, community, notification, support).
3. Repository/API client gọi đúng endpoint trong `03-api-reference.md`, model class map đúng field trong `02-database-schema.md`.
4. Màn hình UI dựng theo thiết kế Figma UI Kit đã phân tích trước đó (flow: Onboarding → Auth → Set Up profile 4.1–4.7 → Home → các module).
5. Interceptor `dio` tự động gắn `Authorization: Bearer`, tự refresh token khi 401, xử lý lỗi theo `error.code` chuẩn.

### C. Kiểm thử & DevOps

- Unit test cho Service layer (xUnit), integration test cho Controller quan trọng (Auth, Workout logs).
- `docker-compose.yml`: PostgreSQL, Redis, API service.
- CI: build + test + `dotnet ef database update` khi merge vào `main`.

## Ưu tiên triển khai (nếu làm theo giai đoạn)

1. Auth + User profile (bắt buộc trước, mọi API khác cần JWT).
2. Workout module (exercises, routines, logs) — nhóm tính năng lõi.
3. Nutrition + Content & Favorites.
4. Community + Notification.
5. Support (chat realtime) — làm sau cùng vì cần SignalR riêng.

## Lưu ý cho Claude Code khi thực hiện

- Luôn đối chiếu tên bảng/cột/endpoint đúng 100% với 2 file `02-database-schema.md` và `03-api-reference.md`, không tự ý đổi tên.
- Sau khi tạo entity/migration, chạy `dotnet build` và `dotnet ef migrations add ... && dotnet ef database update` (nếu có DB kết nối) để xác nhận không lỗi trước khi qua bước tiếp theo.
- Mỗi module hoàn thành nên có Swagger hoạt động được (test thử qua `dotnet run` + `/swagger`).

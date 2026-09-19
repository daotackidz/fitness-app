# FitBody App - Tổng quan Kiến trúc

Tài liệu đặc tả kỹ thuật cho backend + app FitBody App: kiến trúc hệ thống, chuẩn API RESTful, schema database (kèm chỉ mục) và danh sách API endpoint, xây dựng dựa trên phân tích hơn 80 màn hình thiết kế Figma UI Kit.

**Stack:** Backend **.NET Core (ASP.NET Core Web API, C#)**, App **Flutter (Dart)**.

## Kiến trúc hệ thống & công nghệ

**Mô hình tổng thể:** Mobile App (Flutter) ⇄ REST API (.NET Core - ASP.NET Core Web API) ⇄ PostgreSQL (dữ liệu quan hệ) + Redis (cache, session, rate-limit) + S3/Cloud Storage (ảnh, video, avatar) + Firebase Cloud Messaging (push notification) + SignalR (chat hỗ trợ realtime).

| Thành phần | Đề xuất | Vai trò |
| --- | --- | --- |
| Mobile app | Flutter (Dart) | Ứng dụng di động iOS/Android theo thiết kế Figma UI Kit |
| Backend framework | .NET Core (ASP.NET Core Web API, C#) | REST API, validation, DI (Entity Framework Core ORM) |
| Database chính | PostgreSQL 15+ | Dữ liệu quan hệ (user, workout, meal, community...) |
| Cache/Queue | Redis | Cache, session, hàng đợi job (nhắc lịch, thông báo) |
| Object storage | S3-compatible | Ảnh bài viết, video, avatar |
| Auth | JWT (access + refresh token) + OAuth2 (Google, Facebook) + biometric token (fingerprint) | Xác thực đa phương thức theo màn hình Login/Sign Up |
| Push notification | Firebase Cloud Messaging | Workout Reminders, System notification |
| Realtime chat | SignalR (ASP.NET Core) | Customer Service, Online Support |
| Search | PostgreSQL full-text search hoặc Elasticsearch (khi dữ liệu lớn) | All Search, Workout Search, Nutrition Search |

## Chuẩn API chung

- **Base URL:** `https://api.fitbodyapp.com/v1`
- **Versioning:** đưa vào path (`/v1`), tăng version khi có breaking change.
- **Format:** `Content-Type: application/json`; request/response đều là JSON.
- **Xác thực:** header `Authorization: Bearer <access_token>` (JWT). Access token hết hạn 15–60 phút, refresh token 30 ngày, lưu refresh token dạng hash trong bảng `refresh_token`.
- **Phân trang:** query `?page=1&limit=20`, response kèm `meta: { total, page, limit, totalPages }`.
- **Format response chuẩn:**

```json
{
  "success": true,
  "data": {},
  "meta": {},
  "error": null
}
```

- **Mã lỗi chuẩn (error.code):**

| HTTP Status | code | Ý nghĩa |
| --- | --- | --- |
| 400 | VALIDATION_ERROR | Dữ liệu đầu vào không hợp lệ |
| 401 | UNAUTHORIZED | Thiếu/hết hạn access token |
| 403 | FORBIDDEN | Không có quyền truy cập tài nguyên |
| 404 | NOT_FOUND | Không tìm thấy tài nguyên |
| 409 | CONFLICT | Trùng dữ liệu (vd: email đã tồn tại) |
| 422 | UNPROCESSABLE_ENTITY | Không xử lý được nghiệp vụ |
| 429 | RATE_LIMITED | Vượt giới hạn số request |
| 500 | INTERNAL_ERROR | Lỗi hệ thống |

- **Rate limiting:** áp dụng theo `user_id`/IP qua Redis, mặc định 100 req/phút cho API thường, 5 req/phút cho các API nhạy cảm (login, forgot password, OTP).

### Nguyên tắc thiết kế RESTful

- **Danh từ số nhiều cho resource:** `/users`, `/routines`, `/meal-plans`... không dùng động từ trong path (trừ nhóm `/auth/*` theo quy ước chung của ngành).
- **HTTP verb đúng ngữ nghĩa:** `GET` (đọc, không đổi state), `POST` (tạo mới), `PUT/PATCH` (cập nhật toàn bộ/một phần), `DELETE` (xoá).
- **Hành động → resource phụ, không dùng verb trong path:** ví dụ "like" là `POST /forum/posts/:id/likes` (tạo bản ghi like) thay vì `POST /forum/posts/:id/like`; "tham gia thử thách" là `POST /challenges/:id/participants` thay vì `/join`.
- **Lọc/tìm kiếm qua query param trên chính collection**, không tạo sub-resource `/search` riêng: `GET /exercises?q=` thay vì `GET /exercises/search?q=`.
- **Đánh dấu trạng thái = PATCH resource đó** với field cần đổi trong body, ví dụ `PATCH /notifications/:id { "is_read": true }` thay vì endpoint `/read` riêng.
- **Mã trạng thái HTTP chuẩn:** `200` (thành công), `201` (tạo mới thành công, kèm header `Location`), `204` (xoá/không có nội dung trả về), `4xx/5xx` theo bảng mã lỗi ở trên.
- **Idempotency:** `GET/PUT/DELETE` phải idempotent; `POST` không đảm bảo idempotent (dùng `Idempotency-Key` header cho các API thanh toán/nhạy cảm nếu có sau này).
- **Không lồng quá 2 cấp resource:** `/forum/posts/:id/comments` là hợp lý; tránh lồng sâu hơn.

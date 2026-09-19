# API Reference (RESTful)

## Auth

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| POST | /auth/register | Đăng ký (full_name, email/phone, password) | Không |
| POST | /auth/login | Đăng nhập email/username + password | Không |
| POST | /auth/social-login | Đăng nhập Google/Facebook (provider, id_token) | Không |
| POST | /auth/biometric-login | Đăng nhập vân tay (device_id, biometric_token) | Không |
| POST | /auth/refresh-token | Cấp lại access token từ refresh token | Refresh token |
| POST | /auth/forgot-password | Gửi OTP/link đặt lại mật khẩu | Không |
| POST | /auth/reset-password | Đặt lại mật khẩu (token, new_password) | Không |
| POST | /auth/logout | Thu hồi refresh token | Có |

## User & Profile

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /users/me | Lấy thông tin profile hiện tại | Có |
| PATCH | /users/me | Cập nhật profile (gender, dob, height, weight, goal, activity_level...) | Có |
| POST | /users/me/avatar | Upload avatar | Có |
| GET | /users/me/settings | Lấy cài đặt (notification, ngôn ngữ) | Có |
| PATCH | /users/me/settings | Cập nhật cài đặt (bao gồm giờ nhắc tập) | Có |
| PATCH | /users/me/password | Đổi mật khẩu | Có |
| DELETE | /users/me | Xoá tài khoản | Có |
| GET | /search?q=&type= | All Search (type=all\|workout\|nutrition) | Có |

## Workout

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /exercises?difficulty=&muscle_group=&q= | Danh sách/tìm kiếm bài tập, lọc theo cấp độ (Workout Search) | Có |
| GET | /exercises/:id | Chi tiết bài tập | Có |
| GET | /routines?level= | Danh sách routine theo cấp độ (Beginner/Intermediate/Advanced) | Có |
| GET | /routines/:id | Chi tiết routine (kèm danh sách exercise) | Có |
| POST | /routines | Tạo routine tuỳ chỉnh (Create your own routine) | Có |
| PATCH | /routines/:id | Sửa routine của tôi | Có |
| DELETE | /routines/:id | Xoá routine của tôi | Có |
| POST | /workout-logs | Ghi log buổi tập | Có |
| GET | /workout-logs?from=&to= | Lịch sử tập luyện | Có |
| POST | /progress | Ghi nhận chỉ số tiến độ (cân nặng, ảnh...) | Có |
| GET | /progress?from=&to= | Xem tiến độ (Progress Tracking) | Có |
| GET | /recommendations | Gợi ý bài tập cá nhân hoá | Có |

## Nutrition

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /meal-plans?goal= | Danh sách meal plan theo mục tiêu | Có |
| GET | /meal-plans/:id | Chi tiết meal plan (các bữa ăn) | Có |
| POST | /meal-plans | Tạo meal plan tuỳ chỉnh | Có |
| GET | /foods?q= | Tìm kiếm thực phẩm (Nutrition Search) | Có |
| GET | /foods/:id | Chi tiết thực phẩm (calories, macro) | Có |

## Content & Favorites

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /articles?category= | Danh sách bài viết (Articles & Tips) | Có |
| GET | /articles/:id | Chi tiết bài viết | Có |
| GET | /videos?category= | Danh sách video (Workout Videos) | Có |
| GET | /videos/:id | Chi tiết video | Có |
| POST | /favorites | Thêm yêu thích (favoritable_type, favoritable_id) | Có |
| DELETE | /favorites/:id | Bỏ yêu thích | Có |
| GET | /favorites?type=article\|video | Danh sách yêu thích | Có |

## Community

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /forum/posts | Danh sách bài đăng (Discussion Forum) | Có |
| POST | /forum/posts | Tạo bài đăng | Có |
| GET | /forum/posts/:id | Chi tiết bài đăng | Có |
| POST | /forum/posts/:id/likes | Thích bài đăng | Có |
| DELETE | /forum/posts/:id/likes | Bỏ thích bài đăng | Có |
| POST | /forum/posts/:id/comments | Bình luận | Có |
| GET | /forum/posts/:id/comments | Danh sách bình luận | Có |
| GET | /challenges?type=weekly\|competition | Danh sách thử thách (Weekly Challenge, Challenge & Competitions) | Có |
| GET | /challenges/:id | Chi tiết thử thách + bảng xếp hạng | Có |
| POST | /challenges/:id/participants | Tham gia thử thách (tạo participant) | Có |
| PATCH | /challenges/:id/participants/me | Cập nhật tiến độ tham gia | Có |

## Notification & Support

| Method | Path | Mô tả | Auth |
| --- | --- | --- | --- |
| GET | /notifications | Danh sách thông báo | Có |
| PATCH | /notifications/:id | Cập nhật trạng thái đã đọc (PATCH body: `{ "is_read": true }`) | Có |
| GET | /faqs?category= | Danh sách FAQ (Help Center, Help & FAQs) | Có |
| POST | /support/tickets | Tạo yêu cầu hỗ trợ (Customer Service) | Có |
| GET | /support/tickets | Danh sách ticket của tôi | Có |
| GET | /support/tickets/:id/messages | Lịch sử tin nhắn hỗ trợ | Có |
| POST | /support/tickets/:id/messages | Gửi tin nhắn (Online Support) | Có |
| WS (SignalR Hub) | /hubs/support/:ticket_id | Kênh realtime chat hỗ trợ | Có |

### Ghi chú triển khai

- Tất cả endpoint `GET` danh sách hỗ trợ phân trang `page`, `limit` theo chuẩn ở file `01-tong-quan-kien-truc.md`.
- `favoritable_type`/`favoritable_id` và các quan hệ polymorphic cần validate tồn tại bản ghi đích trước khi insert.
- Các bảng có cột lọc/tra cứu thường xuyên theo cặp (user_id + ngày, user_id + trạng thái) nên dùng **composite index** thay vì 2 index đơn: `workout_logs`, `progress_tracking`, `notifications`, `support_tickets`, `challenge_participants`.

## Admin API (Phụ lục — role-based, ghi admin_audit_logs cho mọi hành động ghi)

### Quản lý người dùng (role: admin)

| Method | Path | Mô tả | Role tối thiểu |
| --- | --- | --- | --- |
| GET | /admin/users?q=&status=&role= | Danh sách + tìm kiếm người dùng | admin |
| GET | /admin/users/:id | Chi tiết người dùng | admin |
| PATCH | /admin/users/:id/status | Khoá/mở khoá tài khoản (`{ "status": "locked" }`) | admin |
| PATCH | /admin/users/:id/role | Đổi role | super_admin |

### Quản lý nội dung (role: moderator/admin)

| Method | Path |
| --- | --- |
| GET, POST | /admin/exercises |
| PATCH, DELETE | /admin/exercises/:id |
| GET, POST | /admin/routines |
| PATCH, DELETE | /admin/routines/:id |
| GET, POST | /admin/meal-plans |
| PATCH, DELETE | /admin/meal-plans/:id |
| GET, POST | /admin/articles |
| PATCH, DELETE | /admin/articles/:id |
| GET, POST | /admin/videos |
| PATCH, DELETE | /admin/videos/:id |
| GET, POST | /admin/faqs |
| PATCH, DELETE | /admin/faqs/:id |
| GET, POST | /admin/challenges |
| PATCH, DELETE | /admin/challenges/:id |

### Kiểm duyệt cộng đồng (role: moderator/admin)

| Method | Path | Mô tả |
| --- | --- | --- |
| GET | /admin/reported-contents?status= | Danh sách báo cáo vi phạm |
| PATCH | /admin/reported-contents/:id | Cập nhật trạng thái xử lý (`{ "status": "reviewed" }`) |
| DELETE | /admin/forum-posts/:id | Gỡ bài vi phạm |
| DELETE | /admin/comments/:id | Gỡ bình luận vi phạm |

### Hỗ trợ khách hàng (role: moderator/admin)

| Method | Path | Mô tả |
| --- | --- | --- |
| GET | /admin/support-tickets?status= | Danh sách toàn bộ ticket |
| PATCH | /admin/support-tickets/:id | Đổi trạng thái ticket |
| POST | /admin/support-tickets/:id/messages | Admin trả lời (đẩy qua SignalR hub `/hubs/support/:ticket_id`) |

### Dashboard thống kê (role: admin)

| Method | Path | Mô tả |
| --- | --- | --- |
| GET | /admin/dashboard/summary | Tổng số user, user mới theo ngày/tuần, số ticket mở, số report chờ xử lý, top routine/article yêu thích nhiều nhất |

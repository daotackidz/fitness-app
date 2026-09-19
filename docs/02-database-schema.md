# Database Schema

## 1. Nhóm User & Auth

**users**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| full_name | VARCHAR(100) | |
| email | VARCHAR(150) | UNIQUE, NOT NULL |
| phone | VARCHAR(20) | UNIQUE, NULLABLE |
| password_hash | VARCHAR(255) | NULL nếu chỉ đăng nhập social |
| gender | ENUM('male','female','other') | |
| date_of_birth | DATE | |
| height_cm | DECIMAL(5,1) | |
| weight_kg | DECIMAL(5,1) | |
| fitness_goal | ENUM('lose_weight','build_muscle','maintain','improve_endurance') | |
| activity_level | ENUM('sedentary','light','moderate','active','very_active') | |
| avatar_url | VARCHAR(255) | |
| status | ENUM('active','locked','deleted') | DEFAULT 'active' |
| created_at / updated_at | TIMESTAMP | |

*Index:* `UNIQUE idx_users_email (email)`, `UNIQUE idx_users_phone (phone)`, `idx_users_status (status)`.

**auth_providers** — đăng nhập Google/Facebook/Fingerprint

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK → users.id) | |
| provider | ENUM('email','google','facebook','fingerprint') | |
| provider_uid | VARCHAR(255) | |
| created_at | TIMESTAMP | |

*Index:* `UNIQUE idx_auth_provider_user (provider, provider_uid)`, `idx_auth_providers_user_id (user_id)`.

**refresh_tokens**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| token_hash | VARCHAR(255) | |
| expires_at | TIMESTAMP | |
| revoked | BOOLEAN | DEFAULT false |

*Index:* `idx_refresh_tokens_user_id (user_id)`, `idx_refresh_tokens_expires_at (expires_at)`.

**user_settings** (1-1 với users)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| user_id | UUID (PK, FK → users.id) | |
| notification_enabled | BOOLEAN | DEFAULT true |
| workout_reminder_time | TIME | |
| language | VARCHAR(10) | DEFAULT 'vi' |

## 2. Nhóm Workout

**exercises**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| name | VARCHAR(150) | |
| description | TEXT | |
| muscle_group | VARCHAR(50) | |
| equipment | VARCHAR(50) | |
| difficulty_level | ENUM('beginner','intermediate','advanced') | |
| video_url | VARCHAR(255) | |
| image_url | VARCHAR(255) | |
| calories_estimate | INT | kcal/phút ước tính |

*Index:* `idx_exercises_muscle_group (muscle_group)`, `idx_exercises_difficulty (difficulty_level)`, full-text search trên `name`.

**routines** (chương trình tập, gồm cả routine hệ thống và "Create your own routine")

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| name | VARCHAR(150) | |
| level | ENUM('beginner','intermediate','advanced') | |
| description | TEXT | |
| duration_weeks | INT | |
| is_custom | BOOLEAN | DEFAULT false |
| created_by_user_id | UUID (FK → users.id, NULLABLE) | NULL nếu là routine hệ thống |

*Index:* `idx_routines_level (level)`, `idx_routines_created_by (created_by_user_id)`.

**routine_exercises** (bảng nối N-N)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| routine_id | UUID (FK → routines.id) | |
| exercise_id | UUID (FK → exercises.id) | |
| sets | INT | |
| reps | INT | |
| rest_seconds | INT | |
| order_index | INT | |

*Index:* `UNIQUE idx_routine_exercise (routine_id, exercise_id, order_index)`, `idx_routine_exercises_routine_id (routine_id)`.

**workout_logs**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| routine_id | UUID (FK, NULLABLE) | |
| exercise_id | UUID (FK, NULLABLE) | |
| log_date | DATE | |
| duration_minutes | INT | |
| sets_completed | INT | |
| reps_completed | INT | |
| weight_used_kg | DECIMAL(5,1) | |
| calories_burned | INT | |

*Index:* `idx_workout_logs_user_date (user_id, log_date)` — phục vụ truy vấn lịch sử theo user và khoảng ngày, `idx_workout_logs_routine_id (routine_id)`.

**progress_tracking**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| record_date | DATE | |
| weight_kg | DECIMAL(5,1) | |
| body_fat_percent | DECIMAL(4,1) | |
| measurements | JSONB | vòng eo/ngực/tay... |
| photo_url | VARCHAR(255) | |

*Index:* `idx_progress_user_date (user_id, record_date)`.

**recommendations**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| exercise_id | UUID (FK) | |
| reason | VARCHAR(255) | |
| created_at | TIMESTAMP | |

*Index:* `idx_recommendations_user_id (user_id)`.

## 3. Nhóm Dinh dưỡng

**meal_plans**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| name | VARCHAR(150) | |
| goal | VARCHAR(50) | |
| description | TEXT | |
| total_calories | INT | |
| is_custom | BOOLEAN | DEFAULT false |
| created_by_user_id | UUID (FK, NULLABLE) | |

*Index:* `idx_meal_plans_goal (goal)`, `idx_meal_plans_created_by (created_by_user_id)`.

**meals**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| meal_plan_id | UUID (FK → meal_plans.id) | |
| meal_type | ENUM('breakfast','lunch','dinner','snack') | |
| name | VARCHAR(150) | |
| calories | INT | |
| protein_g | DECIMAL(5,1) | |
| carbs_g | DECIMAL(5,1) | |
| fat_g | DECIMAL(5,1) | |
| image_url | VARCHAR(255) | |

*Index:* `idx_meals_plan_type (meal_plan_id, meal_type)`.

**food_items** (dùng cho Nutrition Search)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| name | VARCHAR(150) | |
| calories | INT | |
| protein_g / carbs_g / fat_g | DECIMAL(5,1) | |
| image_url | VARCHAR(255) | |
| category | VARCHAR(50) | |

*Index:* full-text search `idx_food_items_name_fts (name)`, `idx_food_items_category (category)`.

## 4. Nhóm Nội dung & Yêu thích

**articles**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| title | VARCHAR(200) | |
| content | TEXT | |
| category | VARCHAR(50) | |
| cover_image | VARCHAR(255) | |
| author | VARCHAR(100) | |
| published_at | TIMESTAMP | |

*Index:* `idx_articles_category (category)`, `idx_articles_published_at (published_at)`.

**videos**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| title | VARCHAR(200) | |
| description | TEXT | |
| video_url | VARCHAR(255) | |
| thumbnail_url | VARCHAR(255) | |
| duration_seconds | INT | |
| category | VARCHAR(50) | |

*Index:* `idx_videos_category (category)`.

**favorites** (polymorphic: article hoặc video)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| favoritable_type | ENUM('article','video') | |
| favoritable_id | UUID | |
| created_at | TIMESTAMP | |

*Index:* `UNIQUE idx_favorites_unique (user_id, favoritable_type, favoritable_id)`, `idx_favorites_user_id (user_id)`.

## 5. Nhóm Cộng đồng

**forum_posts**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| title | VARCHAR(200) | |
| content | TEXT | |
| image_url | VARCHAR(255) | |
| likes_count | INT | DEFAULT 0 |
| created_at | TIMESTAMP | |

*Index:* `idx_forum_posts_user_id (user_id)`, `idx_forum_posts_created_at (created_at)`.

**post_likes** (bảng nối cho tính năng like/unlike)

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| post_id | UUID (FK → forum_posts.id) | |
| user_id | UUID (FK) | |
| created_at | TIMESTAMP | |

*Index:* `UNIQUE idx_post_likes_unique (post_id, user_id)`.

**comments**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| post_id | UUID (FK → forum_posts.id) | |
| user_id | UUID (FK) | |
| content | TEXT | |
| created_at | TIMESTAMP | |

*Index:* `idx_comments_post_id (post_id)`.

**challenges**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| name | VARCHAR(150) | |
| description | TEXT | |
| type | ENUM('weekly','competition') | |
| start_date / end_date | DATE | |
| goal_metric | VARCHAR(100) | |
| reward | VARCHAR(150) | |

*Index:* `idx_challenges_type_dates (type, start_date, end_date)`.

**challenge_participants**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| challenge_id | UUID (FK) | |
| user_id | UUID (FK) | |
| progress | DECIMAL(5,2) | |
| rank | INT | |
| joined_at | TIMESTAMP | |

*Index:* `UNIQUE idx_challenge_participant (challenge_id, user_id)`, `idx_challenge_participants_challenge_id (challenge_id)`.

## 6. Nhóm Thông báo & Hỗ trợ

**notifications**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| type | ENUM('workout_reminder','system') | |
| title | VARCHAR(150) | |
| message | TEXT | |
| is_read | BOOLEAN | DEFAULT false |
| scheduled_at | TIMESTAMP | |
| created_at | TIMESTAMP | |

*Index:* `idx_notifications_user_read (user_id, is_read)`, `idx_notifications_scheduled_at (scheduled_at)`.

**faqs**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| question | VARCHAR(255) | |
| answer | TEXT | |
| category | VARCHAR(50) | |

*Index:* `idx_faqs_category (category)`.

**support_tickets**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| user_id | UUID (FK) | |
| subject | VARCHAR(200) | |
| status | ENUM('open','pending','closed') | DEFAULT 'open' |
| created_at | TIMESTAMP | |

*Index:* `idx_support_tickets_user_status (user_id, status)`.

**support_messages**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| ticket_id | UUID (FK → support_tickets.id) | |
| sender_type | ENUM('user','agent','bot') | |
| message | TEXT | |
| sent_at | TIMESTAMP | |

*Index:* `idx_support_messages_ticket_id (ticket_id)`.

## 7. Phụ lục: Phân quyền & Quản trị (bổ sung cho Admin Web + Mobile Admin)

**users** — bổ sung cột:

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| role | ENUM('user','moderator','admin','super_admin') | DEFAULT 'user' |

*Index bổ sung:* `idx_users_role (role)`.

**admin_audit_logs**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| admin_user_id | UUID (FK → users.id) | |
| action | VARCHAR(100) | |
| target_type | VARCHAR(50) | |
| target_id | UUID (NULLABLE) | |
| metadata | JSONB (NULLABLE) | |
| created_at | TIMESTAMP | |

*Index:* `idx_admin_audit_logs_admin_user_id (admin_user_id)`, `idx_admin_audit_logs_created_at (created_at)`.

**reported_contents**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| reporter_user_id | UUID (FK → users.id) | |
| reportable_type | ENUM('forum_post','comment') | |
| reportable_id | UUID | |
| reason | VARCHAR(255) | |
| status | ENUM('pending','reviewed','dismissed') | DEFAULT 'pending' |
| created_at | TIMESTAMP | |

*Index:* `idx_reported_contents_status (status)`.

## 8. Phụ lục: File media (video_files, image_files)

Toàn bộ file video/ảnh do người dùng hoặc admin upload (qua Azure Blob Storage) được lưu metadata đầy đủ
trong 2 bảng riêng, thay vì chỉ lưu URL dạng chuỗi rải rác trong các bảng nội dung. Các cột `*_url` gốc
trong đặc tả ban đầu (ở mục 1-6) được thay bằng khoá ngoại trỏ tới 2 bảng này; response API vẫn trả về
URL đã resolve sẵn (không đổi hợp đồng API phía client).

**video_files**

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| id | UUID (PK) | |
| url | VARCHAR(500) | SAS URL (read, hạn dài) vì storage account chặn public access |
| blob_name | VARCHAR(255) | Tên blob duy nhất trong container |
| container | VARCHAR(100) | Container Azure chứa file (vd: `videos`, `exercises`) |
| file_name | VARCHAR(255) | Tên file gốc lúc upload |
| content_type | VARCHAR(100) (NULLABLE) | MIME type |
| size_bytes | BIGINT | |
| uploaded_by_user_id | UUID (FK → users.id, NULLABLE) | |
| created_at | TIMESTAMP | |

*Index:* `idx_video_files_uploaded_by (uploaded_by_user_id)`, `idx_video_files_created_at (created_at)`.

**image_files** — cấu trúc giống hệt `video_files` (cùng các cột trên), tách bảng riêng để phân biệt loại
media và tránh nhầm lẫn với bảng `videos` (nội dung "Workout Videos" đã có ở mục 4).

*Index:* `idx_image_files_uploaded_by (uploaded_by_user_id)`, `idx_image_files_created_at (created_at)`.

**Các cột đã đổi từ URL string sang khoá ngoại:**

| Bảng | Cột cũ | Cột mới |
| --- | --- | --- |
| users | avatar_url | avatar_image_id (FK → image_files.id, NULLABLE) |
| exercises | video_url, image_url | video_file_id, image_file_id (FK, NULLABLE) |
| articles | cover_image | cover_image_id (FK → image_files.id, NULLABLE) |
| videos | video_url (required), thumbnail_url | video_file_id (FK → video_files.id, NOT NULL), thumbnail_image_id (FK, NULLABLE) |
| progress_tracking | photo_url | photo_image_id (FK → image_files.id, NULLABLE) |
| forum_posts | image_url | image_file_id (FK → image_files.id, NULLABLE) |
| meals | image_url | image_file_id (FK → image_files.id, NULLABLE) |
| food_items | image_url | image_file_id (FK → image_files.id, NULLABLE) |

**Upload API:** `POST /uploads?container=&type=` (user, container giới hạn `forum-posts`/`progress`) và
`POST /admin/uploads?container=&type=` (admin, container giới hạn `exercises`/`articles`/`videos`/`meal-plans`),
`type` là `video` hoặc `image`. Response trả `{ id, url }` — `id` dùng để gửi lại trong request tạo/sửa
nội dung (vd. `imageFileId`), `url` dùng để hiển thị preview ngay trên client.

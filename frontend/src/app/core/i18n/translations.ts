import { Lang } from '../services/language.service';

/**
 * Lightweight runtime dictionary for the admin shell + the pages built for
 * it (login/register/dashboard/categories/users). Deeper CRUD pages
 * (exercises, routines, articles, ...) still use their original Vietnamese
 * labels directly in resource-configs.ts, not through this dictionary -
 * wiring every field label there into the EN/VI switch is a larger
 * follow-up, not covered by this dictionary yet.
 */
export const TRANSLATIONS: Record<string, Record<Lang, string>> = {
  'nav.dashboard': { vi: 'Dashboard', en: 'Dashboard' },
  'nav.users': { vi: 'Người dùng', en: 'Users' },
  'nav.exercises': { vi: 'Bài tập', en: 'Exercises' },
  'nav.routines': { vi: 'Routine', en: 'Routines' },
  'nav.mealPlans': { vi: 'Meal Plans', en: 'Meal Plans' },
  'nav.articles': { vi: 'Articles', en: 'Articles' },
  'nav.videos': { vi: 'Videos', en: 'Videos' },
  'nav.faqs': { vi: 'FAQs', en: 'FAQs' },
  'nav.challenges': { vi: 'Challenges', en: 'Challenges' },
  'nav.categories': { vi: 'Danh mục', en: 'Categories' },
  'nav.reportedContents': { vi: 'Kiểm duyệt', en: 'Moderation' },
  'nav.support': { vi: 'Hỗ trợ', en: 'Support' },
  'nav.logout': { vi: 'Đăng xuất', en: 'Log out' },

  'login.title': { vi: 'Đăng nhập', en: 'Log In' },
  'login.subtitle': { vi: 'Đăng nhập để quản trị nội dung và người dùng FitBody', en: 'Sign in to manage FitBody content and users' },
  'login.email': { vi: 'Email', en: 'Email' },
  'login.password': { vi: 'Mật khẩu', en: 'Password' },
  'login.remember': { vi: 'Ghi nhớ tài khoản', en: 'Remember me' },
  'login.submit': { vi: 'Đăng nhập', en: 'Log In' },

  'register.title': { vi: 'Tạo tài khoản', en: 'Create Account' },
  'register.subtitle': {
    vi: 'Tạo tài khoản nhân viên quản trị (moderator / admin)',
    en: 'Create a staff account (moderator / admin)'
  },
  'register.fullName': { vi: 'Họ và tên', en: 'Full name' },
  'register.email': { vi: 'Email', en: 'Email' },
  'register.phone': { vi: 'Số điện thoại (tùy chọn)', en: 'Phone (optional)' },
  'register.password': { vi: 'Mật khẩu', en: 'Password' },
  'register.confirmPassword': { vi: 'Xác nhận mật khẩu', en: 'Confirm password' },
  'register.role': { vi: 'Vai trò', en: 'Role' },
  'register.submit': { vi: 'Đăng ký', en: 'Sign Up' },
  'register.mismatch': { vi: 'Mật khẩu xác nhận không khớp', en: 'Passwords do not match' },
  'register.fullNamePlaceholder': { vi: 'Nguyễn Văn A', en: 'Jane Doe' },
  'register.passwordPlaceholder': { vi: 'Ít nhất 8 ký tự', en: 'At least 8 characters' },
  'register.confirmPasswordPlaceholder': { vi: 'Nhập lại mật khẩu', en: 'Re-enter password' },

  'dashboard.title': { vi: 'Dashboard', en: 'Dashboard' },
  'dashboard.totalUsers': { vi: 'Tổng số người dùng', en: 'Total users' },
  'dashboard.newUsersToday': { vi: 'User mới hôm nay', en: 'New users today' },
  'dashboard.newUsersThisWeek': { vi: 'User mới tuần này', en: 'New users this week' },
  'dashboard.openTickets': { vi: 'Ticket đang mở', en: 'Open tickets' },
  'dashboard.pendingReports': { vi: 'Report chờ xử lý', en: 'Pending reports' },
  'dashboard.userGrowth': { vi: 'Người dùng mới (30 ngày)', en: 'New users (30 days)' },
  'dashboard.workoutActivity': { vi: 'Hoạt động tập luyện (30 ngày)', en: 'Workout activity (30 days)' },
  'dashboard.contentDistribution': { vi: 'Phân bố nội dung', en: 'Content distribution' },
  'dashboard.ticketReportStatus': { vi: 'Trạng thái ticket / report', en: 'Ticket / report status' },
  'dashboard.supportTicket': { vi: 'Support ticket', en: 'Support tickets' },
  'dashboard.reportedContent': { vi: 'Nội dung bị báo cáo', en: 'Reported content' },
  'dashboard.topRoutines': { vi: 'Top Routine (theo lượt tập)', en: 'Top routines (by workouts)' },
  'dashboard.topArticles': { vi: 'Top Article (theo yêu thích)', en: 'Top articles (by favorites)' },

  'categories.title': { vi: 'Danh mục', en: 'Categories' },
  'categories.add': { vi: 'Thêm category', en: 'Add category' },
  'categories.name': { vi: 'Tên', en: 'Name' },
  'categories.usage': { vi: 'Đang được dùng', en: 'In use' },
  'categories.createdAt': { vi: 'Ngày tạo', en: 'Created at' },
  'categories.actions': { vi: 'Hành động', en: 'Actions' },
  'categories.empty': { vi: 'Chưa có category nào cho loại này', en: 'No categories for this type yet' },

  'users.title': { vi: 'Người dùng', en: 'Users' },
  'users.addStaff': { vi: 'Thêm tài khoản quản trị', en: 'Add staff account' }
};

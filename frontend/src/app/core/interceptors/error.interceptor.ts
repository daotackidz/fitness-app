import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

const ERROR_MESSAGES: Record<string, string> = {
  VALIDATION_ERROR: 'Dữ liệu không hợp lệ',
  UNAUTHORIZED: 'Bạn cần đăng nhập lại',
  FORBIDDEN: 'Bạn không có quyền thực hiện hành động này',
  NOT_FOUND: 'Không tìm thấy dữ liệu',
  CONFLICT: 'Dữ liệu bị trùng',
  UNPROCESSABLE_ENTITY: 'Không thể xử lý yêu cầu',
  RATE_LIMITED: 'Bạn thao tác quá nhanh, vui lòng thử lại sau',
  INTERNAL_ERROR: 'Đã có lỗi hệ thống, vui lòng thử lại sau'
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const code = error.error?.error?.code as string | undefined;
      const message = error.error?.error?.message || (code && ERROR_MESSAGES[code]) || 'Đã có lỗi xảy ra';
      snackBar.open(message, 'Đóng', { duration: 4000 });
      return throwError(() => error);
    })
  );
};

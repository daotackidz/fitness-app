import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

const ERROR_MESSAGES: Record<string, string> = {
  VALIDATION_ERROR: 'Du lieu khong hop le',
  UNAUTHORIZED: 'Ban can dang nhap lai',
  FORBIDDEN: 'Ban khong co quyen thuc hien hanh dong nay',
  NOT_FOUND: 'Khong tim thay du lieu',
  CONFLICT: 'Du lieu bi trung',
  UNPROCESSABLE_ENTITY: 'Khong the xu ly yeu cau',
  RATE_LIMITED: 'Ban thao tac qua nhanh, vui long thu lai sau',
  INTERNAL_ERROR: 'Da co loi he thong, vui long thu lai sau'
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const code = error.error?.error?.code as string | undefined;
      const message = (code && ERROR_MESSAGES[code]) || error.error?.error?.message || 'Da co loi xay ra';
      snackBar.open(message, 'Dong', { duration: 4000 });
      return throwError(() => error);
    })
  );
};

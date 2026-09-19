export interface ApiError {
  code: string;
  message: string;
}

export interface PageMeta {
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  meta: PageMeta | null;
  error: ApiError | null;
}

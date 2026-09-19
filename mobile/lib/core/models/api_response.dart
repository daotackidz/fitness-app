class ApiError {
  final String code;
  final String message;

  ApiError({required this.code, required this.message});

  factory ApiError.fromJson(Map<String, dynamic> json) =>
      ApiError(code: json['code'] as String, message: json['message'] as String);
}

class PageMeta {
  final int total;
  final int page;
  final int limit;
  final int totalPages;

  PageMeta({required this.total, required this.page, required this.limit, required this.totalPages});

  factory PageMeta.fromJson(Map<String, dynamic> json) => PageMeta(
        total: json['total'] as int,
        page: json['page'] as int,
        limit: json['limit'] as int,
        totalPages: json['totalPages'] as int,
      );
}

class ApiResponse<T> {
  final bool success;
  final T? data;
  final PageMeta? meta;
  final ApiError? error;

  ApiResponse({required this.success, this.data, this.meta, this.error});

  factory ApiResponse.fromJson(Map<String, dynamic> json, T Function(dynamic) fromJsonT) {
    return ApiResponse(
      success: json['success'] as bool,
      data: json['data'] == null ? null : fromJsonT(json['data']),
      meta: json['meta'] == null ? null : PageMeta.fromJson(json['meta'] as Map<String, dynamic>),
      error: json['error'] == null ? null : ApiError.fromJson(json['error'] as Map<String, dynamic>),
    );
  }
}

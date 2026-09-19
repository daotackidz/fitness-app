import 'package:dio/dio.dart';

String extractErrorMessage(DioException e, {String fallback = 'Da co loi xay ra'}) {
  final data = e.response?.data;
  if (data is Map) {
    final error = data['error'];
    if (error is Map && error['message'] is String) {
      return error['message'] as String;
    }
  }
  return fallback;
}

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../app_config.dart';
import '../storage/secure_storage_service.dart';
import 'auth_token_holder.dart';

final secureStorageProvider = Provider<SecureStorageService>((ref) => SecureStorageService());

final dioProvider = Provider<Dio>((ref) {
  final tokenHolder = ref.watch(authTokenHolderProvider);
  final storage = ref.watch(secureStorageProvider);

  final dio = Dio(BaseOptions(baseUrl: AppConfig.apiBaseUrl, connectTimeout: const Duration(seconds: 15)));

  dio.interceptors.add(InterceptorsWrapper(
    onRequest: (options, handler) {
      if (tokenHolder.accessToken != null) {
        options.headers['Authorization'] = 'Bearer ${tokenHolder.accessToken}';
      }
      handler.next(options);
    },
    onError: (error, handler) async {
      final isAuthPath = error.requestOptions.path.contains('/auth/');
      if (error.response?.statusCode != 401 || isAuthPath) {
        handler.next(error);
        return;
      }

      final refreshToken = await storage.readRefreshToken();
      if (refreshToken == null) {
        handler.next(error);
        return;
      }

      try {
        final refreshDio = Dio(BaseOptions(baseUrl: AppConfig.apiBaseUrl));
        final response = await refreshDio.post('/auth/refresh-token', data: {'refreshToken': refreshToken});
        final data = response.data['data'] as Map<String, dynamic>;
        tokenHolder.accessToken = data['accessToken'] as String;
        await storage.saveRefreshToken(data['refreshToken'] as String);

        final retryOptions = error.requestOptions;
        retryOptions.headers['Authorization'] = 'Bearer ${tokenHolder.accessToken}';
        final retryResponse = await dio.fetch(retryOptions);
        handler.resolve(retryResponse);
      } catch (_) {
        tokenHolder.accessToken = null;
        await storage.clear();
        handler.next(error);
      }
    },
  ));

  return dio;
});

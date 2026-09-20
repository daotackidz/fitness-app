import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

import '../../../core/models/api_response.dart';
import '../domain/auth_models.dart';

part 'auth_api.g.dart';

@RestApi()
abstract class AuthApi {
  factory AuthApi(Dio dio, {String baseUrl}) = _AuthApi;

  @POST('/auth/register')
  Future<HttpResponse<dynamic>> register(@Body() Map<String, dynamic> body);

  @POST('/auth/login')
  Future<HttpResponse<dynamic>> login(@Body() Map<String, dynamic> body);

  @POST('/auth/logout')
  Future<HttpResponse<dynamic>> logout(@Body() Map<String, dynamic> body);

  @POST('/auth/forgot-password')
  Future<HttpResponse<dynamic>> forgotPassword(@Body() Map<String, dynamic> body);

  @POST('/auth/verify-reset-code')
  Future<HttpResponse<dynamic>> verifyResetCode(@Body() Map<String, dynamic> body);

  @POST('/auth/reset-password')
  Future<HttpResponse<dynamic>> resetPassword(@Body() Map<String, dynamic> body);
}

class AuthRepository {
  final AuthApi _api;

  AuthRepository(this._api);

  Future<AuthResult> register({required String fullName, required String email, String? phone, required String password}) async {
    final response = await _api.register({
      'fullName': fullName,
      'email': email,
      'phone': phone,
      'password': password,
    });
    return _parseAuthResult(response);
  }

  Future<AuthResult> login({required String email, required String password}) async {
    final response = await _api.login({'email': email, 'password': password});
    return _parseAuthResult(response);
  }

  Future<void> logout(String refreshToken) async {
    await _api.logout({'refreshToken': refreshToken});
  }

  Future<void> forgotPassword(String email) async {
    await _api.forgotPassword({'email': email});
  }

  Future<void> verifyResetCode({required String email, required String code}) async {
    await _api.verifyResetCode({'email': email, 'code': code});
  }

  Future<void> resetPassword({required String email, required String code, required String newPassword}) async {
    await _api.resetPassword({'email': email, 'code': code, 'newPassword': newPassword});
  }

  AuthResult _parseAuthResult(HttpResponse<dynamic> response) {
    final wrapper = ApiResponse<AuthResult>.fromJson(
      response.data,
      (json) => AuthResult.fromJson(json as Map<String, dynamic>),
    );
    return wrapper.data!;
  }
}

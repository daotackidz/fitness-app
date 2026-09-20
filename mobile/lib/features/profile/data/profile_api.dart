import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'profile_api.g.dart';

@RestApi()
abstract class ProfileApi {
  factory ProfileApi(Dio dio, {String baseUrl}) = _ProfileApi;

  @GET('/users/me')
  Future<HttpResponse<dynamic>> getMe();

  @PATCH('/users/me')
  Future<HttpResponse<dynamic>> updateMe(@Body() Map<String, dynamic> body);

  @GET('/users/me/settings')
  Future<HttpResponse<dynamic>> getSettings();

  @PATCH('/users/me/settings')
  Future<HttpResponse<dynamic>> updateSettings(@Body() Map<String, dynamic> body);

  @PATCH('/users/me/password')
  Future<HttpResponse<dynamic>> changePassword(@Body() Map<String, dynamic> body);

  @DELETE('/users/me')
  Future<HttpResponse<dynamic>> deleteAccount();

  @GET('/favorites')
  Future<HttpResponse<dynamic>> getFavorites(@Queries() Map<String, dynamic> query);

  @POST('/favorites')
  Future<HttpResponse<dynamic>> addFavorite(@Body() Map<String, dynamic> body);

  @DELETE('/favorites/{id}')
  Future<HttpResponse<dynamic>> deleteFavorite(@Path('id') String id);

  @GET('/faqs')
  Future<HttpResponse<dynamic>> getFaqs(@Queries() Map<String, dynamic> query);
}

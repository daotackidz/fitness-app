import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'admin_api.g.dart';

@RestApi()
abstract class AdminApi {
  factory AdminApi(Dio dio, {String baseUrl}) = _AdminApi;

  @GET('/admin/dashboard/summary')
  Future<HttpResponse<dynamic>> getDashboardSummary();

  @GET('/admin/reported-contents')
  Future<HttpResponse<dynamic>> getReportedContents(@Queries() Map<String, dynamic> query);

  @PATCH('/admin/reported-contents/{id}')
  Future<HttpResponse<dynamic>> updateReportedContentStatus(@Path('id') String id, @Body() Map<String, dynamic> body);

  @DELETE('/admin/forum-posts/{id}')
  Future<HttpResponse<dynamic>> deleteForumPost(@Path('id') String id);

  @DELETE('/admin/comments/{id}')
  Future<HttpResponse<dynamic>> deleteComment(@Path('id') String id);

  @GET('/admin/support-tickets')
  Future<HttpResponse<dynamic>> getSupportTickets(@Queries() Map<String, dynamic> query);

  @GET('/admin/support-tickets/{id}/messages')
  Future<HttpResponse<dynamic>> getMessages(@Path('id') String id, @Queries() Map<String, dynamic> query);

  @POST('/admin/support-tickets/{id}/messages')
  Future<HttpResponse<dynamic>> replySupportTicket(@Path('id') String id, @Body() Map<String, dynamic> body);

  @GET('/admin/users')
  Future<HttpResponse<dynamic>> searchUsers(@Queries() Map<String, dynamic> query);

  @PATCH('/admin/users/{id}/status')
  Future<HttpResponse<dynamic>> updateUserStatus(@Path('id') String id, @Body() Map<String, dynamic> body);
}

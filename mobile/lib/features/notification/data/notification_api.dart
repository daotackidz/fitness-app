import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'notification_api.g.dart';

@RestApi()
abstract class NotificationApi {
  factory NotificationApi(Dio dio, {String baseUrl}) = _NotificationApi;

  @GET('/notifications')
  Future<HttpResponse<dynamic>> getNotifications(@Queries() Map<String, dynamic> query);

  @PATCH('/notifications/{id}')
  Future<HttpResponse<dynamic>> markRead(@Path('id') String id, @Body() Map<String, dynamic> body);
}

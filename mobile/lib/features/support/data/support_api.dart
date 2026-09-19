import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'support_api.g.dart';

@RestApi()
abstract class SupportApi {
  factory SupportApi(Dio dio, {String baseUrl}) = _SupportApi;

  @GET('/support/tickets')
  Future<HttpResponse<dynamic>> getTickets(@Queries() Map<String, dynamic> query);

  @POST('/support/tickets')
  Future<HttpResponse<dynamic>> createTicket(@Body() Map<String, dynamic> body);

  @GET('/support/tickets/{id}/messages')
  Future<HttpResponse<dynamic>> getMessages(@Path('id') String id, @Queries() Map<String, dynamic> query);

  @POST('/support/tickets/{id}/messages')
  Future<HttpResponse<dynamic>> sendMessage(@Path('id') String id, @Body() Map<String, dynamic> body);
}

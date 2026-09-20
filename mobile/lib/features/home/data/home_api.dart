import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'home_api.g.dart';

@RestApi()
abstract class HomeApi {
  factory HomeApi(Dio dio, {String baseUrl}) = _HomeApi;

  @GET('/home')
  Future<HttpResponse<dynamic>> getHome();

  @POST('/favorites')
  Future<HttpResponse<dynamic>> addFavorite(@Body() Map<String, dynamic> body);
}

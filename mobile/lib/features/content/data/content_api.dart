import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'content_api.g.dart';

@RestApi()
abstract class ContentApi {
  factory ContentApi(Dio dio, {String baseUrl}) = _ContentApi;

  @GET('/articles')
  Future<HttpResponse<dynamic>> getArticles(@Queries() Map<String, dynamic> query);

  @GET('/videos')
  Future<HttpResponse<dynamic>> getVideos(@Queries() Map<String, dynamic> query);

  @POST('/favorites')
  Future<HttpResponse<dynamic>> addFavorite(@Body() Map<String, dynamic> body);
}

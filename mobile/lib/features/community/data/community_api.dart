import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'community_api.g.dart';

@RestApi()
abstract class CommunityApi {
  factory CommunityApi(Dio dio, {String baseUrl}) = _CommunityApi;

  @GET('/forum/posts')
  Future<HttpResponse<dynamic>> getPosts(@Queries() Map<String, dynamic> query);

  @POST('/forum/posts')
  Future<HttpResponse<dynamic>> createPost(@Body() Map<String, dynamic> body);

  @POST('/forum/posts/{id}/likes')
  Future<HttpResponse<dynamic>> like(@Path('id') String id);

  @DELETE('/forum/posts/{id}/likes')
  Future<HttpResponse<dynamic>> unlike(@Path('id') String id);

  @GET('/challenges')
  Future<HttpResponse<dynamic>> getChallenges(@Queries() Map<String, dynamic> query);
}

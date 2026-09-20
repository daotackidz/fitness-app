import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'search_api.g.dart';

@RestApi()
abstract class SearchApi {
  factory SearchApi(Dio dio, {String baseUrl}) = _SearchApi;

  @GET('/search')
  Future<HttpResponse<dynamic>> search(@Queries() Map<String, dynamic> query);
}

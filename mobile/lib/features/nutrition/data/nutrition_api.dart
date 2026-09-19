import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'nutrition_api.g.dart';

@RestApi()
abstract class NutritionApi {
  factory NutritionApi(Dio dio, {String baseUrl}) = _NutritionApi;

  @GET('/meal-plans')
  Future<HttpResponse<dynamic>> getMealPlans(@Queries() Map<String, dynamic> query);

  @GET('/foods')
  Future<HttpResponse<dynamic>> getFoods(@Queries() Map<String, dynamic> query);
}

import 'package:dio/dio.dart' hide Headers;
import 'package:retrofit/retrofit.dart';

part 'workout_api.g.dart';

@RestApi()
abstract class WorkoutApi {
  factory WorkoutApi(Dio dio, {String baseUrl}) = _WorkoutApi;

  @GET('/exercises')
  Future<HttpResponse<dynamic>> getExercises(@Queries() Map<String, dynamic> query);

  @GET('/routines')
  Future<HttpResponse<dynamic>> getRoutines(@Queries() Map<String, dynamic> query);

  @POST('/workout-logs')
  Future<HttpResponse<dynamic>> createWorkoutLog(@Body() Map<String, dynamic> body);
}

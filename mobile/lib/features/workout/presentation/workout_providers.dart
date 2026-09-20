import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/workout_api.dart';

final workoutApiProvider = Provider<WorkoutApi>((ref) => WorkoutApi(ref.watch(dioProvider)));

final exercisesProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(workoutApiProvider).getExercises({'page': 1, 'limit': 50});
  final data = response.data['data'] as List<dynamic>;
  return data.cast<Map<String, dynamic>>();
});

final routinesProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(workoutApiProvider).getRoutines({'page': 1, 'limit': 50});
  final data = response.data['data'] as List<dynamic>;
  return data.cast<Map<String, dynamic>>();
});

final filteredRoutinesProvider = FutureProvider.autoDispose.family<List<Map<String, dynamic>>, String>((ref, levelParam) async {
  final query = <String, dynamic>{'page': 1, 'limit': 50};
  if (levelParam.isNotEmpty) query['level'] = levelParam;
  final response = await ref.watch(workoutApiProvider).getRoutines(query);
  final data = response.data['data'] as List<dynamic>;
  return data.cast<Map<String, dynamic>>();
});

final routineDetailProvider = FutureProvider.autoDispose.family<Map<String, dynamic>, String>((ref, routineId) async {
  final response = await ref.watch(workoutApiProvider).getRoutineDetail(routineId);
  return response.data['data'] as Map<String, dynamic>;
});

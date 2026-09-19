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

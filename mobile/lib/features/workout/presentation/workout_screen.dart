import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'workout_providers.dart';

class WorkoutScreen extends ConsumerWidget {
  const WorkoutScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Workout'),
          bottom: const TabBar(tabs: [Tab(text: 'Bai tap'), Tab(text: 'Routine')]),
        ),
        body: TabBarView(
          children: [
            _ExerciseList(),
            _RoutineList(),
          ],
        ),
      ),
    );
  }
}

class _ExerciseList extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final exercisesAsync = ref.watch(exercisesProvider);

    return exercisesAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
      data: (exercises) => ListView.separated(
        itemCount: exercises.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final e = exercises[index];
          return ListTile(
            title: Text(e['name'] as String),
            subtitle: Text('${e['muscleGroup'] ?? ''} - ${e['difficultyLevel']}'),
            trailing: e['caloriesEstimate'] != null ? Text('${e['caloriesEstimate']} kcal/p') : null,
          );
        },
      ),
    );
  }
}

class _RoutineList extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final routinesAsync = ref.watch(routinesProvider);

    return routinesAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
      data: (routines) => ListView.separated(
        itemCount: routines.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final r = routines[index];
          return ListTile(
            title: Text(r['name'] as String),
            subtitle: Text('${r['level']} - ${r['durationWeeks'] ?? '?'} tuan'),
          );
        },
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/nutrition_api.dart';

final nutritionApiProvider = Provider<NutritionApi>((ref) => NutritionApi(ref.watch(dioProvider)));

final mealPlansProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(nutritionApiProvider).getMealPlans({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

class NutritionScreen extends ConsumerWidget {
  const NutritionScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final mealPlansAsync = ref.watch(mealPlansProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Dinh duong')),
      body: mealPlansAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
        data: (plans) => ListView.separated(
          itemCount: plans.length,
          separatorBuilder: (_, _) => const Divider(height: 1),
          itemBuilder: (context, index) {
            final p = plans[index];
            return ListTile(
              title: Text(p['name'] as String),
              subtitle: Text('Muc tieu: ${p['goal'] ?? '-'}'),
              trailing: p['totalCalories'] != null ? Text('${p['totalCalories']} kcal') : null,
            );
          },
        ),
      ),
    );
  }
}

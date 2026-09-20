import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/home_api.dart';

final homeApiProvider = Provider<HomeApi>((ref) => HomeApi(ref.watch(dioProvider)));

final homeProvider = FutureProvider.autoDispose<Map<String, dynamic>>((ref) async {
  final response = await ref.watch(homeApiProvider).getHome();
  return response.data['data'] as Map<String, dynamic>;
});

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/profile_api.dart';

final profileApiProvider = Provider<ProfileApi>((ref) => ProfileApi(ref.watch(dioProvider)));

final myProfileProvider = FutureProvider.autoDispose<Map<String, dynamic>>((ref) async {
  final response = await ref.watch(profileApiProvider).getMe();
  return response.data['data'] as Map<String, dynamic>;
});

final profileSettingsProvider = FutureProvider.autoDispose<Map<String, dynamic>>((ref) async {
  final response = await ref.watch(profileApiProvider).getSettings();
  return response.data['data'] as Map<String, dynamic>;
});

final favoritesProvider = FutureProvider.autoDispose.family<List<Map<String, dynamic>>, String?>((ref, type) async {
  final response = await ref.watch(profileApiProvider).getFavorites({'type': ?type});
  final data = response.data['data'] as List<dynamic>;
  return data.cast<Map<String, dynamic>>();
});

typedef FaqQuery = ({String? category, String? q});

final faqsProvider = FutureProvider.autoDispose.family<List<Map<String, dynamic>>, FaqQuery>((ref, params) async {
  final response = await ref.watch(profileApiProvider).getFaqs({
    if (params.category != null) 'category': params.category,
    if (params.q != null && params.q!.isNotEmpty) 'q': params.q,
  });
  final data = response.data['data'] as List<dynamic>;
  return data.cast<Map<String, dynamic>>();
});

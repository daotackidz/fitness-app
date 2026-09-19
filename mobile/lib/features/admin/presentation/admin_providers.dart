import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/admin_api.dart';

final adminApiProvider = Provider<AdminApi>((ref) => AdminApi(ref.watch(dioProvider)));

final dashboardSummaryProvider = FutureProvider.autoDispose<Map<String, dynamic>>((ref) async {
  final response = await ref.watch(adminApiProvider).getDashboardSummary();
  return response.data['data'] as Map<String, dynamic>;
});

final reportedContentsProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(adminApiProvider).getReportedContents({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

final adminTicketsProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(adminApiProvider).getSupportTickets({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

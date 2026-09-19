import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/notification_api.dart';

final notificationApiProvider = Provider<NotificationApi>((ref) => NotificationApi(ref.watch(dioProvider)));

final notificationsProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(notificationApiProvider).getNotifications({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

class NotificationScreen extends ConsumerWidget {
  const NotificationScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final notificationsAsync = ref.watch(notificationsProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Thong bao')),
      body: notificationsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
        data: (items) => ListView.separated(
          itemCount: items.length,
          separatorBuilder: (_, _) => const Divider(height: 1),
          itemBuilder: (context, index) {
            final n = items[index];
            final isRead = n['isRead'] as bool;
            return ListTile(
              leading: Icon(isRead ? Icons.notifications_none : Icons.notifications_active, color: isRead ? Colors.grey : Colors.blue),
              title: Text(n['title'] as String),
              subtitle: Text(n['message'] ?? ''),
              onTap: () async {
                if (!isRead) {
                  await ref.read(notificationApiProvider).markRead(n['id'] as String, {'isRead': true});
                  ref.invalidate(notificationsProvider);
                }
              },
            );
          },
        ),
      ),
    );
  }
}

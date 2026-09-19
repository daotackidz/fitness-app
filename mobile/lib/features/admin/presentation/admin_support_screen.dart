import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'admin_providers.dart';
import 'admin_ticket_chat_screen.dart';

class AdminSupportScreen extends ConsumerWidget {
  const AdminSupportScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ticketsAsync = ref.watch(adminTicketsProvider);

    return ticketsAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
      data: (tickets) => ListView.separated(
        itemCount: tickets.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final t = tickets[index];
          return ListTile(
            title: Text(t['subject'] as String),
            subtitle: Text('${t['userFullName']} - ${t['status']}'),
            onTap: () => Navigator.of(context).push(
              MaterialPageRoute(builder: (_) => AdminTicketChatScreen(ticketId: t['id'] as String, subject: t['subject'] as String)),
            ),
          );
        },
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import 'admin_providers.dart';
import 'admin_ticket_chat_screen.dart';

class AdminSupportScreen extends ConsumerWidget {
  const AdminSupportScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ticketsAsync = ref.watch(adminTicketsProvider);
    final l10n = context.l10n;

    return ticketsAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
      data: (tickets) => ListView.separated(
        itemCount: tickets.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final t = tickets[index];
          return ListTile(
            title: Text(t['subject'] as String),
            subtitle: Text(l10n.t('admin.support.ticketSubtitle', {
              'userFullName': '${t['userFullName']}',
              'status': '${t['status']}',
            })),
            onTap: () => Navigator.of(context).push(
              MaterialPageRoute(builder: (_) => AdminTicketChatScreen(ticketId: t['id'] as String, subject: t['subject'] as String)),
            ),
          );
        },
      ),
    );
  }
}

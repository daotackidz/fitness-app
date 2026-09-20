import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import 'support_providers.dart';
import 'ticket_chat_screen.dart';

class SupportScreen extends ConsumerWidget {
  const SupportScreen({super.key});

  Future<void> _createTicket(BuildContext context, WidgetRef ref) async {
    final l10n = context.l10n;
    final controller = TextEditingController();
    final subject = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(l10n.t('support.createTicket.title')),
        content: TextField(controller: controller, decoration: InputDecoration(labelText: l10n.t('support.createTicket.topicLabel'))),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context), child: Text(l10n.t('support.createTicket.cancel'))),
          FilledButton(onPressed: () => Navigator.pop(context, controller.text), child: Text(l10n.t('support.createTicket.create'))),
        ],
      ),
    );

    if (subject == null || subject.trim().isEmpty) return;

    await ref.read(supportApiProvider).createTicket({'subject': subject.trim()});
    ref.invalidate(myTicketsProvider);
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final ticketsAsync = ref.watch(myTicketsProvider);
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.t('support.title'))),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _createTicket(context, ref),
        child: const Icon(Icons.add),
      ),
      body: ticketsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
        data: (tickets) => ListView.separated(
          itemCount: tickets.length,
          separatorBuilder: (_, _) => const Divider(height: 1),
          itemBuilder: (context, index) {
            final t = tickets[index];
            return ListTile(
              title: Text(t['subject'] as String),
              subtitle: Text(l10n.t('support.ticket.statusLabel', {'status': '${t['status']}'})),
              onTap: () => Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => TicketChatScreen(ticketId: t['id'] as String, subject: t['subject'] as String)),
              ),
            );
          },
        ),
      ),
    );
  }
}

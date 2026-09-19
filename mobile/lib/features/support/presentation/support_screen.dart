import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'support_providers.dart';
import 'ticket_chat_screen.dart';

class SupportScreen extends ConsumerWidget {
  const SupportScreen({super.key});

  Future<void> _createTicket(BuildContext context, WidgetRef ref) async {
    final controller = TextEditingController();
    final subject = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Tao yeu cau ho tro'),
        content: TextField(controller: controller, decoration: const InputDecoration(labelText: 'Chu de')),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context), child: const Text('Huy')),
          FilledButton(onPressed: () => Navigator.pop(context, controller.text), child: const Text('Tao')),
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

    return Scaffold(
      appBar: AppBar(title: const Text('Ho tro')),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _createTicket(context, ref),
        child: const Icon(Icons.add),
      ),
      body: ticketsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
        data: (tickets) => ListView.separated(
          itemCount: tickets.length,
          separatorBuilder: (_, _) => const Divider(height: 1),
          itemBuilder: (context, index) {
            final t = tickets[index];
            return ListTile(
              title: Text(t['subject'] as String),
              subtitle: Text('Trang thai: ${t['status']}'),
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

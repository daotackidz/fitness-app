import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/auth_token_holder.dart';
import '../../../core/network/support_hub_connection.dart';
import 'admin_providers.dart';

class AdminTicketChatScreen extends ConsumerStatefulWidget {
  final String ticketId;
  final String subject;

  const AdminTicketChatScreen({super.key, required this.ticketId, required this.subject});

  @override
  ConsumerState<AdminTicketChatScreen> createState() => _AdminTicketChatScreenState();
}

class _AdminTicketChatScreenState extends ConsumerState<AdminTicketChatScreen> {
  final _messages = <Map<String, dynamic>>[];
  final _textController = TextEditingController();
  final _hub = SupportHubConnection();
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final response = await ref.read(adminApiProvider).getMessages(widget.ticketId, {'page': 1, 'limit': 100});
    final items = (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();

    final token = ref.read(authTokenHolderProvider).accessToken;
    if (token != null) {
      await _hub.connect(widget.ticketId, token, (message) {
        setState(() => _messages.add(message));
      });
    }

    setState(() {
      _messages.addAll(items);
      _loading = false;
    });
  }

  Future<void> _send() async {
    final text = _textController.text.trim();
    if (text.isEmpty) return;

    _textController.clear();
    await ref.read(adminApiProvider).replySupportTicket(widget.ticketId, {'message': text});
  }

  @override
  void dispose() {
    _hub.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(widget.subject)),
      body: Column(
        children: [
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : ListView.builder(
                    padding: const EdgeInsets.all(12),
                    itemCount: _messages.length,
                    itemBuilder: (context, index) {
                      final m = _messages[index];
                      final isAgent = (m['senderType'] as String).toLowerCase() == 'agent';
                      return Align(
                        alignment: isAgent ? Alignment.centerRight : Alignment.centerLeft,
                        child: Container(
                          margin: const EdgeInsets.symmetric(vertical: 4),
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color: isAgent ? Colors.green.shade100 : Colors.grey.shade200,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Text(m['message'] as String),
                        ),
                      );
                    },
                  ),
          ),
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(8),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _textController,
                      decoration: InputDecoration(hintText: l10n.t('admin.ticketChat.replyHint'), border: const OutlineInputBorder()),
                      onSubmitted: (_) => _send(),
                    ),
                  ),
                  IconButton(onPressed: _send, icon: const Icon(Icons.send)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

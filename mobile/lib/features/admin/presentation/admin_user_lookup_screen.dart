import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'admin_providers.dart';

class AdminUserLookupScreen extends ConsumerStatefulWidget {
  const AdminUserLookupScreen({super.key});

  @override
  ConsumerState<AdminUserLookupScreen> createState() => _AdminUserLookupScreenState();
}

class _AdminUserLookupScreenState extends ConsumerState<AdminUserLookupScreen> {
  final _searchController = TextEditingController();
  List<Map<String, dynamic>> _results = [];
  bool _loading = false;

  Future<void> _search() async {
    setState(() => _loading = true);
    try {
      final response = await ref.read(adminApiProvider).searchUsers({'q': _searchController.text.trim(), 'page': 1, 'limit': 20});
      setState(() => _results = (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>());
    } finally {
      setState(() => _loading = false);
    }
  }

  Future<void> _toggleLock(Map<String, dynamic> user) async {
    final isLocked = (user['status'] as String).toLowerCase() == 'locked';
    await ref.read(adminApiProvider).updateUserStatus(user['id'] as String, {'status': isLocked ? 'active' : 'locked'});
    await _search();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _searchController,
                  decoration: const InputDecoration(hintText: 'Tim theo ten hoac email', border: OutlineInputBorder()),
                  onSubmitted: (_) => _search(),
                ),
              ),
              const SizedBox(width: 8),
              FilledButton(onPressed: _search, child: const Text('Tim')),
            ],
          ),
        ),
        if (_loading) const LinearProgressIndicator(),
        Expanded(
          child: ListView.separated(
            itemCount: _results.length,
            separatorBuilder: (_, _) => const Divider(height: 1),
            itemBuilder: (context, index) {
              final u = _results[index];
              final isLocked = (u['status'] as String).toLowerCase() == 'locked';
              return ListTile(
                title: Text(u['fullName'] as String),
                subtitle: Text('${u['email']} - ${u['role']}'),
                trailing: TextButton(
                  onPressed: () => _toggleLock(u),
                  child: Text(isLocked ? 'Mo khoa' : 'Khoa'),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

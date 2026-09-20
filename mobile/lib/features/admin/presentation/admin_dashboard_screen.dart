import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import 'admin_providers.dart';

class AdminDashboardScreen extends ConsumerWidget {
  const AdminDashboardScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final summaryAsync = ref.watch(dashboardSummaryProvider);
    final l10n = context.l10n;

    return summaryAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
      data: (summary) => GridView.count(
        padding: const EdgeInsets.all(16),
        crossAxisCount: 2,
        mainAxisSpacing: 12,
        crossAxisSpacing: 12,
        childAspectRatio: 1.4,
        children: [
          _StatCard(label: l10n.t('admin.dashboard.stat.totalUsers'), value: '${summary['totalUsers']}'),
          _StatCard(label: l10n.t('admin.dashboard.stat.newUsersToday'), value: '${summary['newUsersToday']}'),
          _StatCard(label: l10n.t('admin.dashboard.stat.newUsersThisWeek'), value: '${summary['newUsersThisWeek']}'),
          _StatCard(label: l10n.t('admin.dashboard.stat.openTickets'), value: '${summary['openTickets']}'),
          _StatCard(label: l10n.t('admin.dashboard.stat.pendingReports'), value: '${summary['pendingReports']}'),
        ],
      ),
    );
  }
}

class _StatCard extends StatelessWidget {
  final String label;
  final String value;

  const _StatCard({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(value, style: const TextStyle(fontSize: 28, fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Text(label, textAlign: TextAlign.center, style: const TextStyle(color: Colors.grey)),
          ],
        ),
      ),
    );
  }
}

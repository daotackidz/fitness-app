import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import 'admin_providers.dart';

class AdminReportedContentsScreen extends ConsumerWidget {
  const AdminReportedContentsScreen({super.key});

  Future<void> _removeContent(WidgetRef ref, Map<String, dynamic> report) async {
    final api = ref.read(adminApiProvider);
    final type = (report['reportableType'] as String).toLowerCase();
    final targetId = report['reportableId'] as String;

    if (type == 'forumpost') {
      await api.deleteForumPost(targetId);
    } else {
      await api.deleteComment(targetId);
    }
    await api.updateReportedContentStatus(report['id'] as String, {'status': 'reviewed'});
    ref.invalidate(reportedContentsProvider);
  }

  Future<void> _dismiss(WidgetRef ref, Map<String, dynamic> report) async {
    await ref.read(adminApiProvider).updateReportedContentStatus(report['id'] as String, {'status': 'dismissed'});
    ref.invalidate(reportedContentsProvider);
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final reportsAsync = ref.watch(reportedContentsProvider);
    final l10n = context.l10n;

    return reportsAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
      data: (reports) => ListView.separated(
        itemCount: reports.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final r = reports[index];
          return ListTile(
            title: Text(l10n.t('admin.moderation.reportTitle', {
              'type': '${r['reportableType']}',
              'status': '${r['status']}',
            })),
            subtitle: Text(r['reason'] ?? l10n.t('admin.moderation.noReason')),
            trailing: r['status'] == 'Pending' || r['status'] == 'pending'
                ? Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(icon: const Icon(Icons.close), tooltip: l10n.t('admin.moderation.dismiss'), onPressed: () => _dismiss(ref, r)),
                      IconButton(icon: const Icon(Icons.delete, color: Colors.red), tooltip: l10n.t('admin.moderation.removeContent'), onPressed: () => _removeContent(ref, r)),
                    ],
                  )
                : null,
          );
        },
      ),
    );
  }
}

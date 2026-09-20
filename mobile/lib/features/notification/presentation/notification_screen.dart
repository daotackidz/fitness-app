import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:intl/intl.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/dio_client.dart';
import '../../../core/theme/app_colors.dart';
import '../../profile/presentation/profile_screen.dart';
import '../../search/presentation/search_screen.dart';
import '../data/notification_api.dart';

final notificationApiProvider = Provider<NotificationApi>((ref) => NotificationApi(ref.watch(dioProvider)));

final notificationsProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(notificationApiProvider).getNotifications({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

(IconData, Color) _iconForTitle(String title) {
  final t = title.toLowerCase();
  if (t.contains('workout') || t.contains('exercise') || t.contains('completed')) {
    return t.contains('completed') ? (Icons.emoji_events, AppColors.brandLime) : (Icons.star, AppColors.brandLavender);
  }
  if (t.contains('drink') || t.contains('remind') || t.contains('remember')) {
    return (Icons.lightbulb, AppColors.brandLavender);
  }
  if (t.contains('article') || t.contains('privacy') || t.contains('terms') || t.contains('maintenance')) {
    return (Icons.description, AppColors.brandLavender);
  }
  if (t.contains('login') || t.contains('device')) {
    return (Icons.notifications, AppColors.brandLavender);
  }
  if (t.contains('message')) {
    return (Icons.star, AppColors.brandLime);
  }
  return (Icons.notifications, AppColors.brandLavender);
}

String _sectionLabel(DateTime date, AppLocalizations l10n) {
  final now = DateTime.now();
  final today = DateTime(now.year, now.month, now.day);
  final that = DateTime(date.year, date.month, date.day);
  final diff = today.difference(that).inDays;
  if (diff == 0) return l10n.t('notification.section.today');
  if (diff == 1) return l10n.t('notification.section.yesterday');
  return DateFormat('MMMM d - yyyy').format(date);
}

class NotificationScreen extends ConsumerStatefulWidget {
  const NotificationScreen({super.key});

  @override
  ConsumerState<NotificationScreen> createState() => _NotificationScreenState();
}

class _NotificationScreenState extends ConsumerState<NotificationScreen> {
  String _type = 'WorkoutReminder';

  Future<void> _markRead(String id) async {
    await ref.read(notificationApiProvider).markRead(id, {'isRead': true});
    ref.invalidate(notificationsProvider);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final notificationsAsync = ref.watch(notificationsProvider);

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                children: [
                  IconButton(
                    padding: EdgeInsets.zero,
                    constraints: const BoxConstraints(),
                    onPressed: () => Navigator.of(context).maybePop(),
                    icon: const Icon(Icons.arrow_back_ios_new, color: AppColors.brandLime, size: 18),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      l10n.t('notification.title'),
                      style: GoogleFonts.poppins(color: Colors.white, fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                  ),
                  _HeaderIconButton(
                    icon: Icons.search,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const SearchScreen())),
                  ),
                  const SizedBox(width: 8),
                  _HeaderIconButton(
                    icon: Icons.person_outline,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ProfileScreen())),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  _FilterPill(
                    label: l10n.t('notification.filter.reminders'),
                    selected: _type == 'WorkoutReminder',
                    onTap: () => setState(() => _type = 'WorkoutReminder'),
                  ),
                  const SizedBox(width: 10),
                  _FilterPill(
                    label: l10n.t('notification.filter.system'),
                    selected: _type == 'System',
                    onTap: () => setState(() => _type = 'System'),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Expanded(
                child: notificationsAsync.when(
                  loading: () => const Center(child: CircularProgressIndicator()),
                  error: (err, _) => Center(
                    child: Text(
                      l10n.t('common.error.loadFailed', {'error': err.toString()}),
                      style: const TextStyle(color: Colors.white),
                    ),
                  ),
                  data: (items) {
                    final filtered = items.where((n) => n['type'] == _type).toList();
                    if (filtered.isEmpty) {
                      return Center(child: Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400)));
                    }
                    final grouped = <String, List<Map<String, dynamic>>>{};
                    for (final n in filtered) {
                      final date = DateTime.parse(n['createdAt'] as String);
                      grouped.putIfAbsent(_sectionLabel(date, l10n), () => []).add(n);
                    }
                    return ListView(
                      children: [
                        for (final entry in grouped.entries) ...[
                          Padding(
                            padding: const EdgeInsets.symmetric(vertical: 8),
                            child: Text(
                              entry.key,
                              style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold, fontSize: 13),
                            ),
                          ),
                          for (final n in entry.value) ...[
                            _NotificationRow(item: n, onTap: () => _markRead(n['id'] as String)),
                            const SizedBox(height: 12),
                          ],
                        ],
                      ],
                    );
                  },
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _NotificationRow extends StatelessWidget {
  const _NotificationRow({required this.item, required this.onTap});

  final Map<String, dynamic> item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final title = item['title'] as String? ?? '';
    final isRead = item['isRead'] as bool? ?? true;
    final createdAt = DateTime.parse(item['createdAt'] as String);
    final (icon, badgeColor) = _iconForTitle(title);

    return InkWell(
      onTap: isRead ? null : onTap,
      borderRadius: BorderRadius.circular(20),
      child: Container(
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(20)),
        padding: const EdgeInsets.all(14),
        child: Row(
          children: [
            Stack(
              clipBehavior: Clip.none,
              children: [
                Container(
                  width: 44,
                  height: 44,
                  decoration: BoxDecoration(color: badgeColor, shape: BoxShape.circle),
                  child: Icon(icon, color: AppColors.brandDark, size: 20),
                ),
                if (!isRead)
                  Positioned(
                    top: -2,
                    left: -2,
                    child: Container(
                      width: 12,
                      height: 12,
                      decoration: const BoxDecoration(color: AppColors.brandLavender, shape: BoxShape.circle),
                    ),
                  ),
              ],
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 14),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    DateFormat('MMMM d - h:mm a').format(createdAt),
                    style: const TextStyle(color: AppColors.brandLavender, fontSize: 12, fontWeight: FontWeight.w600),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _HeaderIconButton extends StatelessWidget {
  const _HeaderIconButton({required this.icon, this.onTap});

  final IconData icon;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 36,
        height: 36,
        decoration: BoxDecoration(color: AppColors.brandDark2, borderRadius: BorderRadius.circular(10)),
        child: Icon(icon, color: AppColors.brandLavender, size: 18),
      ),
    );
  }
}

class _FilterPill extends StatelessWidget {
  const _FilterPill({required this.label, required this.selected, required this.onTap});

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 8),
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.transparent,
          border: Border.all(color: selected ? AppColors.brandLime : Colors.grey.shade600),
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(
          label,
          style: TextStyle(color: selected ? AppColors.brandDark : Colors.white, fontWeight: FontWeight.w600, fontSize: 13),
        ),
      ),
    );
  }
}

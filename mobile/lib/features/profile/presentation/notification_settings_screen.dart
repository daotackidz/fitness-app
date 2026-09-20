import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import 'profile_providers.dart';
import 'widgets/profile_widgets.dart';

class NotificationSettingsScreen extends ConsumerStatefulWidget {
  const NotificationSettingsScreen({super.key});

  @override
  ConsumerState<NotificationSettingsScreen> createState() => _NotificationSettingsScreenState();
}

class _NotificationSettingsScreenState extends ConsumerState<NotificationSettingsScreen> {
  Map<String, dynamic>? _settings;

  Future<void> _toggle(String key, bool value) async {
    final previous = _settings;
    setState(() => _settings = {...?_settings, key: value});
    try {
      await ref.read(profileApiProvider).updateSettings({key: value});
    } on DioException catch (_) {
      if (mounted) setState(() => _settings = previous);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final settingsAsync = ref.watch(profileSettingsProvider);

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Column(
          children: [
            ProfilePushHeader(title: l10n.t('profile.settings.notification')),
            Expanded(
              child: settingsAsync.when(
                loading: () => const Center(child: CircularProgressIndicator()),
                error: (err, _) => Center(
                  child: Text(
                    l10n.t('common.error.loadFailed', {'error': err.toString()}),
                    style: const TextStyle(color: Colors.white),
                  ),
                ),
                data: (data) {
                  _settings ??= data;
                  final settings = _settings!;
                  return ListView(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    children: [
                      _ToggleRow(
                        label: l10n.t('profile.notification.general'),
                        value: settings['notificationEnabled'] == true,
                        onChanged: (v) => _toggle('notificationEnabled', v),
                      ),
                      _ToggleRow(
                        label: l10n.t('profile.notification.sound'),
                        value: settings['soundEnabled'] == true,
                        onChanged: (v) => _toggle('soundEnabled', v),
                      ),
                      _ToggleRow(
                        label: l10n.t('profile.notification.doNotDisturb'),
                        value: settings['doNotDisturbEnabled'] == true,
                        onChanged: (v) => _toggle('doNotDisturbEnabled', v),
                      ),
                      _ToggleRow(
                        label: l10n.t('profile.notification.vibrate'),
                        value: settings['vibrateEnabled'] == true,
                        onChanged: (v) => _toggle('vibrateEnabled', v),
                      ),
                      _ToggleRow(
                        label: l10n.t('profile.notification.lockScreen'),
                        value: settings['lockScreenEnabled'] == true,
                        onChanged: (v) => _toggle('lockScreenEnabled', v),
                      ),
                      _ToggleRow(
                        label: l10n.t('profile.notification.reminders'),
                        value: settings['remindersEnabled'] == true,
                        onChanged: (v) => _toggle('remindersEnabled', v),
                      ),
                    ],
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ToggleRow extends StatelessWidget {
  const _ToggleRow({required this.label, required this.value, required this.onChanged});

  final String label;
  final bool value;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.symmetric(horizontal: 16),
      decoration: BoxDecoration(color: AppColors.brandDark2, borderRadius: BorderRadius.circular(16)),
      child: SwitchListTile(
        contentPadding: EdgeInsets.zero,
        title: Text(label, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w600)),
        value: value,
        activeThumbColor: AppColors.brandLime,
        onChanged: onChanged,
      ),
    );
  }
}

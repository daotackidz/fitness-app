import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../auth/presentation/auth_controller.dart';
import 'notification_settings_screen.dart';
import 'password_settings_screen.dart';
import 'profile_providers.dart';
import 'widgets/profile_widgets.dart';

class SettingsScreen extends ConsumerWidget {
  const SettingsScreen({super.key});

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref) async {
    final l10n = context.l10n;
    final confirmed = await showConfirmSheet(
      context,
      message: l10n.t('profile.settings.deleteConfirm'),
      confirmLabel: l10n.t('profile.settings.deleteYes'),
      cancelLabel: l10n.t('common.cancel'),
    );
    if (!confirmed) return;
    try {
      await ref.read(profileApiProvider).deleteAccount();
    } catch (_) {
      // ke ca goi xoa tai khoan that bai, van tiep tuc dang xuat cuc bo
    }
    await ref.read(authControllerProvider.notifier).logout();
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Column(
          children: [
            ProfilePushHeader(title: l10n.t('profile.settings.title')),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                children: [
                  ProfileMenuRow(
                    icon: Icons.notifications_none,
                    label: l10n.t('profile.settings.notification'),
                    trailingIcon: Icons.keyboard_arrow_down,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NotificationSettingsScreen())),
                  ),
                  ProfileMenuRow(
                    icon: Icons.lock_outline,
                    label: l10n.t('profile.settings.password'),
                    trailingIcon: Icons.keyboard_arrow_down,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const PasswordSettingsScreen())),
                  ),
                  ProfileMenuRow(
                    icon: Icons.delete_outline,
                    label: l10n.t('profile.settings.deleteAccount'),
                    trailingIcon: Icons.keyboard_arrow_down,
                    onTap: () => _confirmDelete(context, ref),
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

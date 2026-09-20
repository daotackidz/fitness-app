import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../auth/presentation/auth_controller.dart';
import 'edit_profile_screen.dart';
import 'favorites_screen.dart';
import 'help_faq_screen.dart';
import 'privacy_policy_screen.dart';
import 'profile_providers.dart';
import 'settings_screen.dart';
import 'widgets/profile_format.dart';
import 'widgets/profile_widgets.dart';

class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final profileAsync = ref.watch(myProfileProvider);

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        bottom: false,
        child: RefreshIndicator(
          onRefresh: () async => ref.invalidate(myProfileProvider),
          child: profileAsync.when(
            loading: () => const Center(child: CircularProgressIndicator()),
            error: (err, _) => ListView(
              children: [
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 120),
                  child: Center(
                    child: Text(
                      l10n.t('common.error.loadFailed', {'error': err.toString()}),
                      style: const TextStyle(color: Colors.white),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ),
              ],
            ),
            data: (data) => _ProfileContent(data: data),
          ),
        ),
      ),
    );
  }
}

class _ProfileContent extends ConsumerWidget {
  const _ProfileContent({required this.data});

  final Map<String, dynamic> data;

  Future<void> _confirmLogout(BuildContext context, WidgetRef ref) async {
    final l10n = context.l10n;
    final confirmed = await showConfirmSheet(
      context,
      message: l10n.t('profile.logout.confirm'),
      confirmLabel: l10n.t('profile.logout.yes'),
      cancelLabel: l10n.t('common.cancel'),
    );
    if (confirmed) {
      await ref.read(authControllerProvider.notifier).logout();
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final fullName = data['fullName'] as String? ?? '';
    final email = data['email'] as String? ?? '';
    final avatarUrl = data['avatarUrl'] as String?;
    final dateOfBirth = data['dateOfBirth'] as String?;
    final weightKg = data['weightKg'];
    final heightCm = data['heightCm'];
    final age = computeAge(dateOfBirth);

    return ListView(
      padding: EdgeInsets.zero,
      children: [
        Container(
          width: double.infinity,
          padding: const EdgeInsets.fromLTRB(20, 4, 20, 28),
          decoration: const BoxDecoration(
            color: AppColors.brandLavender,
            borderRadius: BorderRadius.vertical(bottom: Radius.circular(32)),
          ),
          child: Column(
            children: [
              Row(
                children: [
                  IconButton(
                    onPressed: () => Navigator.of(context).maybePop(),
                    icon: const Icon(Icons.arrow_back_ios_new, color: AppColors.brandDark, size: 18),
                  ),
                  Expanded(
                    child: Text(
                      l10n.t('profile.myProfile'),
                      textAlign: TextAlign.center,
                      style: GoogleFonts.poppins(color: AppColors.brandDark, fontSize: 18, fontWeight: FontWeight.bold),
                    ),
                  ),
                  const SizedBox(width: 18),
                ],
              ),
              const SizedBox(height: 12),
              CircleAvatar(
                radius: 44,
                backgroundColor: AppColors.brandDark2,
                backgroundImage: avatarUrl != null ? NetworkImage(avatarUrl) : null,
                child: avatarUrl == null ? const Icon(Icons.person, size: 44, color: Colors.white70) : null,
              ),
              const SizedBox(height: 12),
              Text(fullName, style: GoogleFonts.poppins(color: AppColors.brandDark, fontSize: 20, fontWeight: FontWeight.bold)),
              const SizedBox(height: 4),
              Text(email, style: TextStyle(color: AppColors.brandDark.withValues(alpha: 0.7), fontSize: 13)),
              const SizedBox(height: 4),
              Text(
                l10n.t('profile.birthday', {'date': formatBirthday(dateOfBirth)}),
                style: TextStyle(color: AppColors.brandDark.withValues(alpha: 0.7), fontSize: 13),
              ),
              const SizedBox(height: 20),
              Row(
                children: [
                  Expanded(
                    child: _StatItem(
                      value: weightKg != null ? l10n.t('profile.stats.weightValue', {'value': '$weightKg'}) : '--',
                      label: l10n.t('profile.stats.weight'),
                    ),
                  ),
                  const _StatDivider(),
                  Expanded(
                    child: _StatItem(
                      value: age != null ? l10n.t('profile.stats.ageValue', {'value': '$age'}) : '--',
                      label: l10n.t('profile.stats.age'),
                    ),
                  ),
                  const _StatDivider(),
                  Expanded(
                    child: _StatItem(
                      value: heightCm != null ? l10n.t('profile.stats.heightValue', {'value': '$heightCm'}) : '--',
                      label: l10n.t('profile.stats.height'),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
        Padding(
          padding: const EdgeInsets.fromLTRB(20, 20, 20, 20),
          child: Column(
            children: [
              ProfileMenuRow(
                icon: Icons.person_outline,
                label: l10n.t('profile.menu.profile'),
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const EditProfileScreen())),
              ),
              ProfileMenuRow(
                icon: Icons.star_border,
                label: l10n.t('profile.menu.favorite'),
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const FavoritesScreen())),
              ),
              ProfileMenuRow(
                icon: Icons.lock_outline,
                label: l10n.t('profile.menu.privacyPolicy'),
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const PrivacyPolicyScreen())),
              ),
              ProfileMenuRow(
                icon: Icons.settings_outlined,
                label: l10n.t('profile.menu.settings'),
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const SettingsScreen())),
              ),
              ProfileMenuRow(
                icon: Icons.headset_mic_outlined,
                label: l10n.t('profile.menu.help'),
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const HelpFaqScreen())),
              ),
              ProfileMenuRow(
                icon: Icons.logout,
                label: l10n.t('profile.menu.logout'),
                onTap: () => _confirmLogout(context, ref),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _StatItem extends StatelessWidget {
  const _StatItem({required this.value, required this.label});

  final String value;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(value, style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 15)),
        const SizedBox(height: 4),
        Text(label, style: TextStyle(color: AppColors.brandDark.withValues(alpha: 0.6), fontSize: 12)),
      ],
    );
  }
}

class _StatDivider extends StatelessWidget {
  const _StatDivider();

  @override
  Widget build(BuildContext context) {
    return Container(width: 1, height: 36, color: AppColors.brandDark.withValues(alpha: 0.2));
  }
}

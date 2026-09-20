import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 2/8: chon gioi tinh (Male / Female), chon 1 trong 2, dang radio dang vong tron.
class GenderStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const GenderStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<GenderStep> createState() => _GenderStepState();
}

class _GenderStepState extends State<GenderStep> {
  late String? _selected = widget.draft.gender;

  void _select(String gender) {
    setState(() {
      _selected = gender;
      widget.draft.gender = gender;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.gender.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.gender.description'),
        ),
        Expanded(
          child: Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                _GenderOption(
                  icon: Icons.male,
                  label: l10n.t('profileSetup.gender.male'),
                  selected: _selected == 'Male',
                  onTap: () => _select('Male'),
                ),
                const SizedBox(height: 32),
                _GenderOption(
                  icon: Icons.female,
                  label: l10n.t('profileSetup.gender.female'),
                  selected: _selected == 'Female',
                  onTap: () => _select('Female'),
                ),
              ],
            ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(24),
          child: AuthPillButton(label: l10n.t('common.continue'), onPressed: _selected == null ? null : widget.onNext),
        ),
      ],
    );
  }
}

class _GenderOption extends StatelessWidget {
  final IconData icon;
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _GenderOption({required this.icon, required this.label, required this.selected, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      behavior: HitTestBehavior.opaque,
      child: Column(
        children: [
          AnimatedContainer(
            duration: const Duration(milliseconds: 150),
            width: 96,
            height: 96,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: selected ? AppColors.brandLime : AppColors.brandDark2,
              border: Border.all(color: selected ? AppColors.brandLime : Colors.white54, width: 2),
            ),
            child: Icon(icon, size: 44, color: selected ? AppColors.brandDark : Colors.white),
          ),
          const SizedBox(height: 10),
          Text(
            label,
            style: TextStyle(
              color: selected ? AppColors.brandLime : Colors.white70,
              fontWeight: FontWeight.bold,
              fontSize: 15,
            ),
          ),
        ],
      ),
    );
  }
}

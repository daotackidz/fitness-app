import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 7/8: chon muc do van dong (chon 1 trong 3), cac pill nam truc tiep
/// tren nen dark (khong boc trong AuthLavenderCard, theo dung mockup).
class ActivityLevelStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const ActivityLevelStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<ActivityLevelStep> createState() => _ActivityLevelStepState();
}

class _ActivityLevelStepState extends State<ActivityLevelStep> {
  // Nhan hien thi la "Advance" theo mockup nhung gia tri gui len API la "Advanced".
  static const _values = ['Beginner', 'Intermediate', 'Advanced'];

  static const _labelKeys = {
    'Beginner': 'profileSetup.activityLevel.beginner',
    'Intermediate': 'profileSetup.activityLevel.intermediate',
    'Advanced': 'profileSetup.activityLevel.advance',
  };

  late String? _selected = widget.draft.activityLevel;

  void _select(String value) {
    setState(() {
      _selected = value;
      widget.draft.activityLevel = value;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.activityLevel.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.activityLevel.description'),
        ),
        Expanded(
          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  for (final value in _values) ...[
                    _ActivityPill(
                      label: l10n.t(_labelKeys[value]!),
                      selected: _selected == value,
                      onTap: () => _select(value),
                    ),
                    const SizedBox(height: 16),
                  ],
                ],
              ),
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

class _ActivityPill extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _ActivityPill({required this.label, required this.selected, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 260,
        padding: const EdgeInsets.symmetric(vertical: 14),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.white,
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(
          label,
          style: TextStyle(
            color: selected ? AppColors.brandDark : AppColors.brandLavender,
            fontWeight: FontWeight.bold,
            fontSize: 16,
          ),
        ),
      ),
    );
  }
}

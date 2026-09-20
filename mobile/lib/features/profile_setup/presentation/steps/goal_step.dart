import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 6/8: chon muc tieu tap luyen (chon 1 trong 5), hien thi dang cac hang
/// pill trang tren nen AuthLavenderCard.
class GoalStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const GoalStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<GoalStep> createState() => _GoalStepState();
}

class _GoalStepState extends State<GoalStep> {
  static const _values = ['LoseWeight', 'GainWeight', 'MuscleMassGain', 'ShapeBody', 'Others'];

  static const _labelKeys = {
    'LoseWeight': 'profileSetup.goal.loseWeight',
    'GainWeight': 'profileSetup.goal.gainWeight',
    'MuscleMassGain': 'profileSetup.goal.muscleMassGain',
    'ShapeBody': 'profileSetup.goal.shapeBody',
    'Others': 'profileSetup.goal.others',
  };

  late String? _selected = widget.draft.fitnessGoal;

  void _select(String value) {
    setState(() {
      _selected = value;
      widget.draft.fitnessGoal = value;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.goal.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.goal.description'),
        ),
        Expanded(
          child: SingleChildScrollView(
            child: AuthLavenderCard(
              children: [
                for (final value in _values) ...[
                  _GoalOptionTile(
                    label: l10n.t(_labelKeys[value]!),
                    selected: _selected == value,
                    onTap: () => _select(value),
                  ),
                  const SizedBox(height: 14),
                ],
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

class _GoalOptionTile extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _GoalOptionTile({required this.label, required this.selected, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: selected ? Border.all(color: AppColors.brandDark, width: 2) : null,
        ),
        child: Row(
          children: [
            Expanded(
              child: Text(label, style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.w600, fontSize: 15)),
            ),
            Icon(
              selected ? Icons.radio_button_checked : Icons.radio_button_off,
              color: selected ? AppColors.brandDark : Colors.black38,
            ),
          ],
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 3/8: chon tuoi bang mot Slider don gian (thay the cho dai so ngang o mockup).
class AgeStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const AgeStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<AgeStep> createState() => _AgeStepState();
}

class _AgeStepState extends State<AgeStep> {
  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.age.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.age.description'),
        ),
        Expanded(
          child: Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  '${widget.draft.age}',
                  style: const TextStyle(color: Colors.white, fontSize: 72, fontWeight: FontWeight.w900),
                ),
                Text(l10n.t('profileSetup.age.unit'), style: const TextStyle(color: AppColors.brandLime, fontSize: 16, fontWeight: FontWeight.bold)),
                const SizedBox(height: 24),
                SizedBox(
                  width: 280,
                  child: Slider(
                    value: widget.draft.age.toDouble(),
                    min: 13,
                    max: 90,
                    activeColor: AppColors.brandLime,
                    inactiveColor: AppColors.brandDark2,
                    onChanged: (v) => setState(() => widget.draft.age = v.round()),
                  ),
                ),
              ],
            ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(24),
          child: AuthPillButton(label: l10n.t('common.continue'), onPressed: widget.onNext),
        ),
      ],
    );
  }
}

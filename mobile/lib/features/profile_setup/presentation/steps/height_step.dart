import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 5/8: chon chieu cao (cm) bang Slider.
class HeightStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const HeightStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<HeightStep> createState() => _HeightStepState();
}

class _HeightStepState extends State<HeightStep> {
  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final clampedValue = widget.draft.heightCm.clamp(120, 220).toDouble();

    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.height.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.height.description'),
        ),
        Expanded(
          child: Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  '${clampedValue.round()}',
                  style: const TextStyle(color: Colors.white, fontSize: 72, fontWeight: FontWeight.w900),
                ),
                Text(l10n.t('profileSetup.height.unit'), style: const TextStyle(color: AppColors.brandLime, fontSize: 16, fontWeight: FontWeight.bold)),
                const SizedBox(height: 24),
                SizedBox(
                  width: 280,
                  child: Slider(
                    value: clampedValue,
                    min: 120,
                    max: 220,
                    activeColor: AppColors.brandLime,
                    inactiveColor: AppColors.brandDark2,
                    onChanged: (v) => setState(() => widget.draft.heightCm = v),
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

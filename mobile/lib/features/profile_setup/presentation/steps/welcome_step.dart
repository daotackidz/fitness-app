import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';

/// Buoc 1/8: man hinh chao mung, khong co truong nhap lieu, khong co nut back.
class WelcomeStep extends StatelessWidget {
  final VoidCallback onNext;

  const WelcomeStep({super.key, required this.onNext});

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      children: [
        Expanded(
          flex: 3,
          child: Image.asset('assets/images/onboarding_hero.png', width: double.infinity, fit: BoxFit.cover),
        ),
        Expanded(
          flex: 4,
          child: Container(
            width: double.infinity,
            color: AppColors.brandDark,
            padding: const EdgeInsets.fromLTRB(24, 20, 24, 0),
            child: Column(
              children: [
                Text(
                  l10n.t('profileSetup.welcome.heading'),
                  textAlign: TextAlign.center,
                  style: const TextStyle(color: AppColors.brandLime, fontSize: 26, fontWeight: FontWeight.w900, height: 1.25),
                ),
                Expanded(
                  child: Container(
                    margin: const EdgeInsets.only(top: 20),
                    width: double.infinity,
                    decoration: const BoxDecoration(
                      color: AppColors.brandLavender,
                      borderRadius: BorderRadius.vertical(top: Radius.circular(32)),
                    ),
                    padding: const EdgeInsets.fromLTRB(24, 24, 24, 24),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          l10n.t('profileSetup.welcome.description'),
                          textAlign: TextAlign.center,
                          style: const TextStyle(color: AppColors.brandDark, fontSize: 14, height: 1.5),
                        ),
                        AuthPillButton(label: l10n.t('common.next'), onPressed: onNext),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}

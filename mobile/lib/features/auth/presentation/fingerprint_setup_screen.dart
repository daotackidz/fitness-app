import 'package:flutter/material.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import 'widgets/auth_widgets.dart';

class FingerprintSetupScreen extends StatelessWidget {
  const FingerprintSetupScreen({super.key});

  void _finish(BuildContext context) {
    // Khong co API dang ky thiet bi sinh trac hoc rieng; man hinh nay chi la
    // buoc UI tuy chon, dang nhap sinh trac se dung /auth/biometric-login sau
    // khi thiet bi da duoc lien ket (ngoai pham vi hien tai).
    Navigator.of(context).maybePop();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Column(
          children: [
            AuthTopSection(
              title: l10n.t('auth.fingerprint.title'),
              showBack: false,
              description: l10n.t('auth.fingerprint.description'),
            ),
            Expanded(
              child: Container(
                width: double.infinity,
                color: AppColors.brandLavender,
                child: const Center(
                  child: Icon(Icons.fingerprint, size: 160, color: Colors.white),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  AuthPillButton(label: l10n.t('common.skip'), onPressed: () => _finish(context)),
                  const SizedBox(height: 16),
                  AuthPillButton(label: l10n.t('common.continue'), onPressed: () => _finish(context)),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

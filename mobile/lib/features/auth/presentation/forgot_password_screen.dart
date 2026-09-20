import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import 'auth_controller.dart';
import 'verify_reset_code_screen.dart';
import 'widgets/auth_widgets.dart';

class ForgotPasswordScreen extends ConsumerStatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  ConsumerState<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends ConsumerState<ForgotPasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  bool _isLoading = false;

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final l10n = context.l10n;
    setState(() => _isLoading = true);
    try {
      final email = _emailController.text.trim();
      await ref.read(authRepositoryProvider).forgotPassword(email);
      if (mounted) {
        showAuthSnackBar(context, l10n.t('auth.forgotPassword.codeSent'));
        Navigator.of(context).push(MaterialPageRoute(builder: (_) => VerifyResetCodeScreen(email: email)));
      }
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.forgotPassword.failure'));
      if (mounted) showAuthSnackBar(context, message, isError: true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: SingleChildScrollView(
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                AuthTopSection(
                  title: l10n.t('auth.forgotPassword.title'),
                  heading: l10n.t('auth.forgotPassword.heading'),
                  description: l10n.t('auth.forgotPassword.description'),
                ),
                AuthLavenderCard(
                  children: [
                    AuthTextField(
                      label: l10n.t('auth.forgotPassword.emailLabel'),
                      controller: _emailController,
                      hint: 'example@example.com',
                      keyboardType: TextInputType.emailAddress,
                      validator: (v) => (v == null || v.trim().isEmpty) ? l10n.t('auth.login.emailRequired') : null,
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(24),
                  child: AuthPillButton(label: l10n.t('auth.forgotPassword.submit'), onPressed: _submit, isLoading: _isLoading),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import 'auth_controller.dart';
import 'widgets/auth_widgets.dart';

class SetPasswordScreen extends ConsumerStatefulWidget {
  final String email;
  final String code;

  const SetPasswordScreen({super.key, required this.email, required this.code});

  @override
  ConsumerState<SetPasswordScreen> createState() => _SetPasswordScreenState();
}

class _SetPasswordScreenState extends ConsumerState<SetPasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _isLoading = false;

  @override
  void dispose() {
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final l10n = context.l10n;
    setState(() => _isLoading = true);
    try {
      await ref.read(authRepositoryProvider).resetPassword(
            email: widget.email,
            code: widget.code,
            newPassword: _passwordController.text,
          );
      if (mounted) {
        showAuthSnackBar(context, l10n.t('auth.setPassword.success'));
        Navigator.of(context).popUntil((route) => route.isFirst);
      }
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.setPassword.failure'));
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
                  title: l10n.t('auth.setPassword.title'),
                  description: l10n.t('auth.setPassword.description'),
                ),
                AuthLavenderCard(
                  children: [
                    AuthTextField(
                      label: l10n.t('auth.register.passwordLabel'),
                      controller: _passwordController,
                      hint: '••••••••••••',
                      obscureText: true,
                      validator: (v) => (v == null || v.length < 8) ? l10n.t('auth.register.passwordTooShort') : null,
                    ),
                    const SizedBox(height: 18),
                    AuthTextField(
                      label: l10n.t('auth.register.confirmPasswordLabel'),
                      controller: _confirmController,
                      hint: '••••••••••••',
                      obscureText: true,
                      validator: (v) => (v != _passwordController.text) ? l10n.t('auth.register.confirmPasswordMismatch') : null,
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(24),
                  child: AuthPillButton(label: l10n.t('auth.setPassword.submit'), onPressed: _submit, isLoading: _isLoading),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

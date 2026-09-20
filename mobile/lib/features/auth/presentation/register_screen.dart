import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import 'auth_controller.dart';
import 'fingerprint_setup_screen.dart';
import 'widgets/auth_widgets.dart';

class RegisterScreen extends ConsumerStatefulWidget {
  const RegisterScreen({super.key});

  @override
  ConsumerState<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends ConsumerState<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _fullNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  @override
  void dispose() {
    _fullNameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final l10n = context.l10n;
    try {
      await ref.read(authControllerProvider.notifier).register(
            _fullNameController.text.trim(),
            _emailController.text.trim(),
            null,
            _passwordController.text,
          );
      if (mounted) {
        showAuthSnackBar(context, l10n.t('auth.register.success'));
        Navigator.of(context).pushReplacement(MaterialPageRoute(builder: (_) => const FingerprintSetupScreen()));
      }
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.register.failure'));
      if (mounted) showAuthSnackBar(context, message, isError: true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = ref.watch(authControllerProvider.select((s) => s.isLoading));
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
                AuthTopSection(title: l10n.t('auth.register.title'), heading: l10n.t('auth.register.heading')),
                AuthLavenderCard(
                  children: [
                    AuthTextField(
                      label: l10n.t('auth.register.fullNameLabel'),
                      controller: _fullNameController,
                      hint: l10n.t('auth.register.fullNameHint'),
                      validator: (v) => (v == null || v.trim().isEmpty) ? l10n.t('auth.register.fullNameRequired') : null,
                    ),
                    const SizedBox(height: 18),
                    AuthTextField(
                      label: l10n.t('auth.register.emailLabel'),
                      controller: _emailController,
                      hint: '+123 567 89000',
                      keyboardType: TextInputType.emailAddress,
                      validator: (v) => (v == null || v.trim().isEmpty) ? l10n.t('auth.register.emailRequired') : null,
                    ),
                    const SizedBox(height: 18),
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
                      controller: _confirmPasswordController,
                      hint: '••••••••••••',
                      obscureText: true,
                      validator: (v) => (v != _passwordController.text) ? l10n.t('auth.register.confirmPasswordMismatch') : null,
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.fromLTRB(24, 24, 24, 24),
                  child: Column(
                    children: [
                      Text.rich(
                        TextSpan(
                          style: TextStyle(color: Colors.white.withValues(alpha: 0.6), fontSize: 12),
                          children: [
                            TextSpan(text: l10n.t('auth.register.termsPrefix')),
                            TextSpan(
                                text: l10n.t('auth.register.termsOfUse'),
                                style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold)),
                            TextSpan(text: l10n.t('auth.register.and')),
                            TextSpan(
                                text: l10n.t('auth.register.privacyPolicy'),
                                style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold)),
                            TextSpan(text: l10n.t('auth.register.periodSuffix')),
                          ],
                        ),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 20),
                      AuthPillButton(label: l10n.t('auth.register.submit'), onPressed: _submit, isLoading: isLoading),
                      const SizedBox(height: 24),
                      const AuthSocialRow(),
                      const SizedBox(height: 24),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(l10n.t('auth.register.haveAccount'), style: TextStyle(color: Colors.white.withValues(alpha: 0.7))),
                          GestureDetector(
                            onTap: () => Navigator.of(context).maybePop(),
                            child: Text(l10n.t('auth.register.login'),
                                style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold)),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

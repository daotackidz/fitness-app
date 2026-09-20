import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import 'auth_controller.dart';
import 'forgot_password_screen.dart';
import 'register_screen.dart';
import 'widgets/auth_widgets.dart';

class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key});

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final l10n = context.l10n;
    try {
      await ref.read(authControllerProvider.notifier).login(_emailController.text.trim(), _passwordController.text);
      if (mounted) showAuthSnackBar(context, l10n.t('auth.login.success'));
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.login.failure'));
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
                AuthTopSection(
                  title: l10n.t('auth.login.title'),
                  showBack: false,
                  heading: l10n.t('auth.login.welcome'),
                  description: l10n.t('auth.login.description'),
                ),
                AuthLavenderCard(
                  children: [
                    AuthTextField(
                      label: l10n.t('auth.login.emailLabel'),
                      controller: _emailController,
                      hint: 'example@example.com',
                      keyboardType: TextInputType.emailAddress,
                      validator: (v) => (v == null || v.trim().isEmpty) ? l10n.t('auth.login.emailRequired') : null,
                    ),
                    const SizedBox(height: 20),
                    AuthTextField(
                      label: l10n.t('auth.login.passwordLabel'),
                      controller: _passwordController,
                      hint: '••••••••••••',
                      obscureText: true,
                      validator: (v) => (v == null || v.isEmpty) ? l10n.t('auth.login.passwordRequired') : null,
                    ),
                    const SizedBox(height: 8),
                    Align(
                      alignment: Alignment.centerRight,
                      child: TextButton(
                        onPressed: () => Navigator.of(context)
                            .push(MaterialPageRoute(builder: (_) => const ForgotPasswordScreen())),
                        child: Text(l10n.t('auth.login.forgotPassword'),
                            style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold)),
                      ),
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.fromLTRB(24, 24, 24, 24),
                  child: Column(
                    children: [
                      AuthPillButton(label: l10n.t('auth.login.submit'), onPressed: _submit, isLoading: isLoading),
                      const SizedBox(height: 24),
                      const AuthSocialRow(),
                      const SizedBox(height: 24),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(l10n.t('auth.login.noAccount'), style: TextStyle(color: Colors.white.withValues(alpha: 0.7))),
                          GestureDetector(
                            onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const RegisterScreen())),
                            child: Text(l10n.t('auth.login.signUp'),
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

import 'dart:async';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import 'auth_controller.dart';
import 'set_password_screen.dart';
import 'widgets/auth_widgets.dart';

const _resendCooldownSeconds = 60;

class VerifyResetCodeScreen extends ConsumerStatefulWidget {
  final String email;

  const VerifyResetCodeScreen({super.key, required this.email});

  @override
  ConsumerState<VerifyResetCodeScreen> createState() => _VerifyResetCodeScreenState();
}

class _VerifyResetCodeScreenState extends ConsumerState<VerifyResetCodeScreen> {
  final _formKey = GlobalKey<FormState>();
  final _codeController = TextEditingController();
  bool _isVerifying = false;
  bool _isResending = false;
  int _secondsLeft = _resendCooldownSeconds;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _startCountdown();
  }

  @override
  void dispose() {
    _timer?.cancel();
    _codeController.dispose();
    super.dispose();
  }

  void _startCountdown() {
    setState(() => _secondsLeft = _resendCooldownSeconds);
    _timer?.cancel();
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (!mounted) return;
      if (_secondsLeft <= 1) {
        timer.cancel();
        setState(() => _secondsLeft = 0);
      } else {
        setState(() => _secondsLeft -= 1);
      }
    });
  }

  Future<void> _resend() async {
    final l10n = context.l10n;
    setState(() => _isResending = true);
    try {
      await ref.read(authRepositoryProvider).forgotPassword(widget.email);
      if (mounted) {
        showAuthSnackBar(context, l10n.t('auth.verifyCode.resendSuccess'));
        _startCountdown();
      }
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.verifyCode.resendFailure'));
      if (mounted) showAuthSnackBar(context, message, isError: true);
    } finally {
      if (mounted) setState(() => _isResending = false);
    }
  }

  Future<void> _verify() async {
    if (!_formKey.currentState!.validate()) return;

    final l10n = context.l10n;
    setState(() => _isVerifying = true);
    try {
      final code = _codeController.text.trim();
      await ref.read(authRepositoryProvider).verifyResetCode(email: widget.email, code: code);
      if (mounted) {
        showAuthSnackBar(context, l10n.t('auth.verifyCode.verifySuccess'));
        Navigator.of(context)
            .push(MaterialPageRoute(builder: (_) => SetPasswordScreen(email: widget.email, code: code)));
      }
    } on DioException catch (e) {
      final message = extractErrorMessage(e, fallback: l10n.t('auth.verifyCode.verifyFailure'));
      if (mounted) showAuthSnackBar(context, message, isError: true);
    } finally {
      if (mounted) setState(() => _isVerifying = false);
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
                  title: l10n.t('auth.verifyCode.title'),
                  heading: l10n.t('auth.verifyCode.heading'),
                  description: l10n.t('auth.verifyCode.description', {'email': widget.email}),
                ),
                AuthLavenderCard(
                  children: [
                    AuthTextField(
                      label: l10n.t('auth.verifyCode.codeLabel'),
                      controller: _codeController,
                      hint: '------',
                      keyboardType: TextInputType.number,
                      maxLength: 6,
                      validator: (v) => (v == null || v.trim().length != 6) ? l10n.t('auth.verifyCode.codeRequired') : null,
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    children: [
                      AuthPillButton(label: l10n.t('auth.verifyCode.submit'), onPressed: _verify, isLoading: _isVerifying),
                      const SizedBox(height: 20),
                      if (_secondsLeft > 0)
                        Text(
                          l10n.t('auth.verifyCode.resendIn', {'seconds': _secondsLeft.toString().padLeft(2, '0')}),
                          style: TextStyle(color: Colors.white.withValues(alpha: 0.6)),
                        )
                      else
                        TextButton(
                          onPressed: _isResending ? null : _resend,
                          child: _isResending
                              ? const SizedBox(
                                  width: 16,
                                  height: 16,
                                  child: CircularProgressIndicator(strokeWidth: 2, color: AppColors.brandLime),
                                )
                              : Text(l10n.t('auth.verifyCode.resend'),
                                  style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold)),
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

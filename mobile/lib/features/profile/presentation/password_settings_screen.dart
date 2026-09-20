import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import '../../auth/presentation/widgets/auth_widgets.dart';
import 'profile_providers.dart';
import 'widgets/profile_widgets.dart';

class PasswordSettingsScreen extends ConsumerStatefulWidget {
  const PasswordSettingsScreen({super.key});

  @override
  ConsumerState<PasswordSettingsScreen> createState() => _PasswordSettingsScreenState();
}

class _PasswordSettingsScreenState extends ConsumerState<PasswordSettingsScreen> {
  final _formKey = GlobalKey<FormState>();
  final _currentController = TextEditingController();
  final _newController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _obscureCurrent = true;
  bool _obscureNew = true;
  bool _obscureConfirm = true;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _currentController.dispose();
    _newController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final l10n = context.l10n;
    if (!_formKey.currentState!.validate()) return;
    if (_newController.text != _confirmController.text) {
      showAuthSnackBar(context, l10n.t('profile.password.mismatch'), isError: true);
      return;
    }
    setState(() => _isSubmitting = true);
    try {
      await ref.read(profileApiProvider).changePassword({
        'currentPassword': _currentController.text,
        'newPassword': _newController.text,
      });
      if (!mounted) return;
      showAuthSnackBar(context, l10n.t('profile.password.success'));
      Navigator.of(context).pop();
    } on DioException catch (e) {
      if (!mounted) return;
      showAuthSnackBar(context, extractErrorMessage(e, fallback: l10n.t('profile.password.failure')), isError: true);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Column(
          children: [
            ProfilePushHeader(title: l10n.t('profile.settings.password')),
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      _PasswordField(
                        label: l10n.t('profile.password.current'),
                        controller: _currentController,
                        obscure: _obscureCurrent,
                        onToggle: () => setState(() => _obscureCurrent = !_obscureCurrent),
                        validator: (v) => (v == null || v.isEmpty) ? l10n.t('profile.password.required') : null,
                      ),
                      const SizedBox(height: 18),
                      _PasswordField(
                        label: l10n.t('profile.password.new'),
                        controller: _newController,
                        obscure: _obscureNew,
                        onToggle: () => setState(() => _obscureNew = !_obscureNew),
                        validator: (v) => (v == null || v.length < 6) ? l10n.t('profile.password.tooShort') : null,
                      ),
                      const SizedBox(height: 18),
                      _PasswordField(
                        label: l10n.t('profile.password.confirm'),
                        controller: _confirmController,
                        obscure: _obscureConfirm,
                        onToggle: () => setState(() => _obscureConfirm = !_obscureConfirm),
                        validator: (v) => (v == null || v.isEmpty) ? l10n.t('profile.password.required') : null,
                      ),
                      const SizedBox(height: 12),
                      Align(
                        alignment: Alignment.centerRight,
                        child: Text(l10n.t('profile.password.forgot'), style: const TextStyle(color: AppColors.brandLavender, fontSize: 13)),
                      ),
                      const SizedBox(height: 24),
                      AuthPillButton(
                        label: l10n.t('profile.password.change'),
                        onPressed: _submit,
                        isLoading: _isSubmitting,
                        backgroundColor: AppColors.brandLime,
                        foregroundColor: AppColors.brandDark,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _PasswordField extends StatelessWidget {
  const _PasswordField({
    required this.label,
    required this.controller,
    required this.obscure,
    required this.onToggle,
    this.validator,
  });

  final String label;
  final TextEditingController controller;
  final bool obscure;
  final VoidCallback onToggle;
  final String? Function(String?)? validator;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: const TextStyle(color: AppColors.brandLavender, fontWeight: FontWeight.bold, fontSize: 13)),
        const SizedBox(height: 8),
        TextFormField(
          controller: controller,
          obscureText: obscure,
          validator: validator,
          style: const TextStyle(color: AppColors.brandDark),
          decoration: InputDecoration(
            filled: true,
            fillColor: Colors.white,
            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
            suffixIcon: IconButton(
              onPressed: onToggle,
              icon: Icon(obscure ? Icons.visibility_off : Icons.visibility, color: AppColors.brandDark.withValues(alpha: 0.6)),
            ),
          ),
        ),
      ],
    );
  }
}

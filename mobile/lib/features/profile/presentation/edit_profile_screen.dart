import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/dio_client.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import '../../auth/presentation/widgets/auth_widgets.dart';
import 'profile_providers.dart';
import 'widgets/profile_format.dart';
import 'widgets/profile_widgets.dart';

class EditProfileScreen extends ConsumerStatefulWidget {
  const EditProfileScreen({super.key});

  @override
  ConsumerState<EditProfileScreen> createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends ConsumerState<EditProfileScreen> {
  final _fullNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _phoneController = TextEditingController();
  final _weightController = TextEditingController();
  final _heightController = TextEditingController();
  DateTime? _dateOfBirth;
  XFile? _avatarFile;
  String? _avatarUrl;
  bool _hydrated = false;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _fullNameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    _weightController.dispose();
    _heightController.dispose();
    super.dispose();
  }

  void _hydrate(Map<String, dynamic> data) {
    if (_hydrated) return;
    _hydrated = true;
    _fullNameController.text = data['fullName'] as String? ?? '';
    _emailController.text = data['email'] as String? ?? '';
    _phoneController.text = data['phone'] as String? ?? '';
    _weightController.text = data['weightKg']?.toString() ?? '';
    _heightController.text = data['heightCm']?.toString() ?? '';
    _avatarUrl = data['avatarUrl'] as String?;
    final dob = data['dateOfBirth'] as String?;
    _dateOfBirth = dob != null ? DateTime.tryParse(dob) : null;
  }

  Future<void> _pickAvatar() async {
    final picked = await ImagePicker().pickImage(source: ImageSource.gallery, imageQuality: 85);
    if (picked != null) setState(() => _avatarFile = picked);
  }

  Future<void> _pickDate() async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _dateOfBirth ?? DateTime(now.year - 20, now.month, now.day),
      firstDate: DateTime(1900),
      lastDate: now,
    );
    if (picked != null) setState(() => _dateOfBirth = picked);
  }

  Future<void> _submit() async {
    final l10n = context.l10n;
    setState(() => _isSubmitting = true);
    final dio = ref.read(dioProvider);
    try {
      if (_avatarFile != null) {
        final form = FormData.fromMap({
          'file': await MultipartFile.fromFile(_avatarFile!.path, filename: _avatarFile!.name),
        });
        await dio.post('/users/me/avatar', data: form);
      }

      final body = <String, dynamic>{
        'fullName': _fullNameController.text.trim(),
        if (_dateOfBirth != null) 'dateOfBirth': formatIsoDate(_dateOfBirth!),
        if (double.tryParse(_heightController.text.trim()) != null)
          'heightCm': double.tryParse(_heightController.text.trim()),
        if (double.tryParse(_weightController.text.trim()) != null)
          'weightKg': double.tryParse(_weightController.text.trim()),
      };
      await ref.read(profileApiProvider).updateMe(body);

      ref.invalidate(myProfileProvider);
      if (!mounted) return;
      showAuthSnackBar(context, l10n.t('profile.edit.success'));
      Navigator.of(context).pop();
    } on DioException catch (e) {
      if (!mounted) return;
      showAuthSnackBar(context, extractErrorMessage(e, fallback: l10n.t('profile.edit.failure')), isError: true);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final profileAsync = ref.watch(myProfileProvider);

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: profileAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (err, _) => Center(
            child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}), style: const TextStyle(color: Colors.white)),
          ),
          data: (data) {
            _hydrate(data);
            return ListView(
              padding: EdgeInsets.zero,
              children: [
                ProfilePushHeader(title: l10n.t('profile.myProfile')),
                Center(
                  child: GestureDetector(
                    onTap: _pickAvatar,
                    child: Stack(
                      children: [
                        CircleAvatar(
                          radius: 48,
                          backgroundColor: AppColors.brandDark2,
                          backgroundImage: _avatarFile != null
                              ? FileImage(File(_avatarFile!.path)) as ImageProvider
                              : (_avatarUrl != null ? NetworkImage(_avatarUrl!) : null),
                          child: (_avatarFile == null && _avatarUrl == null)
                              ? const Icon(Icons.person, size: 48, color: Colors.white70)
                              : null,
                        ),
                        Positioned(
                          right: 0,
                          bottom: 0,
                          child: Container(
                            padding: const EdgeInsets.all(6),
                            decoration: const BoxDecoration(color: AppColors.brandDark, shape: BoxShape.circle),
                            child: const Icon(Icons.attach_file, size: 16, color: AppColors.brandLime),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 24),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      _FieldLabel(l10n.t('profile.edit.fullName')),
                      _EditField(controller: _fullNameController),
                      const SizedBox(height: 16),
                      _FieldLabel(l10n.t('profile.edit.email')),
                      _EditField(controller: _emailController, enabled: false),
                      const SizedBox(height: 16),
                      _FieldLabel(l10n.t('profile.edit.mobile')),
                      _EditField(controller: _phoneController, enabled: false),
                      const SizedBox(height: 16),
                      _FieldLabel(l10n.t('profile.edit.dateOfBirth')),
                      _DateField(
                        text: _dateOfBirth != null ? formatDdMmYyyy(_dateOfBirth!) : '',
                        onTap: _pickDate,
                      ),
                      const SizedBox(height: 16),
                      _FieldLabel(l10n.t('profile.edit.weight')),
                      _EditField(controller: _weightController, keyboardType: TextInputType.number),
                      const SizedBox(height: 16),
                      _FieldLabel(l10n.t('profile.edit.height')),
                      _EditField(controller: _heightController, keyboardType: TextInputType.number),
                      const SizedBox(height: 28),
                      AuthPillButton(
                        label: l10n.t('profile.edit.submit'),
                        onPressed: _submit,
                        isLoading: _isSubmitting,
                        backgroundColor: AppColors.brandLime,
                        foregroundColor: AppColors.brandDark,
                      ),
                      const SizedBox(height: 20),
                    ],
                  ),
                ),
              ],
            );
          },
        ),
      ),
    );
  }
}

class _FieldLabel extends StatelessWidget {
  const _FieldLabel(this.text);

  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Text(text, style: const TextStyle(color: AppColors.brandLavender, fontWeight: FontWeight.bold, fontSize: 13)),
    );
  }
}

class _EditField extends StatelessWidget {
  const _EditField({required this.controller, this.enabled = true, this.keyboardType});

  final TextEditingController controller;
  final bool enabled;
  final TextInputType? keyboardType;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      enabled: enabled,
      keyboardType: keyboardType,
      style: TextStyle(color: enabled ? AppColors.brandDark : AppColors.brandDark.withValues(alpha: 0.5)),
      decoration: InputDecoration(
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
        disabledBorder: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
      ),
    );
  }
}

class _DateField extends StatelessWidget {
  const _DateField({required this.text, required this.onTap});

  final String text;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(14)),
        child: Row(
          children: [
            Expanded(child: Text(text.isEmpty ? '--' : text, style: const TextStyle(color: AppColors.brandDark))),
            const Icon(Icons.calendar_today, size: 16, color: AppColors.brandDark),
          ],
        ),
      ),
    );
  }
}

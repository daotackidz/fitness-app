import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 8/8 (cuoi cung): dien thong tin ho so + chon avatar, roi bam Start de
/// submit toan bo du lieu da thu thap qua 8 buoc. Viec goi API (upload avatar
/// + /users/me/onboarding) duoc thuc hien o ProfileSetupFlow (noi co `ref`),
/// widget nay chi thu thap input va bao qua callback onSubmit.
class FillProfileStep extends StatefulWidget {
  final String initialFullName;
  final String initialEmail;
  final String initialPhone;
  final VoidCallback onBack;
  final bool isSubmitting;
  final void Function({
    required String fullName,
    required String nickname,
    required String phone,
    required XFile? avatarFile,
  }) onSubmit;

  const FillProfileStep({
    super.key,
    required this.initialFullName,
    required this.initialEmail,
    required this.initialPhone,
    required this.onBack,
    required this.isSubmitting,
    required this.onSubmit,
  });

  @override
  State<FillProfileStep> createState() => _FillProfileStepState();
}

class _FillProfileStepState extends State<FillProfileStep> {
  final _formKey = GlobalKey<FormState>();
  late final _fullNameController = TextEditingController(text: widget.initialFullName);
  final _nicknameController = TextEditingController();
  late final _emailController = TextEditingController(text: widget.initialEmail);
  late final _phoneController = TextEditingController(text: widget.initialPhone);
  XFile? _avatarFile;

  @override
  void dispose() {
    _fullNameController.dispose();
    _nicknameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  Future<void> _pickAvatar() async {
    final picked = await ImagePicker().pickImage(source: ImageSource.gallery, imageQuality: 85);
    if (picked != null) {
      setState(() => _avatarFile = picked);
    }
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    widget.onSubmit(
      fullName: _fullNameController.text.trim(),
      nickname: _nicknameController.text.trim(),
      phone: _phoneController.text.trim(),
      avatarFile: _avatarFile,
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return SingleChildScrollView(
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            ProfileSetupHeader(
              title: l10n.t('profileSetup.fillProfile.title'),
              onBack: widget.onBack,
              description: l10n.t('profileSetup.fillProfile.description'),
            ),
            AuthLavenderCard(
              children: [
                Center(
                  child: GestureDetector(
                    onTap: _pickAvatar,
                    child: Stack(
                      children: [
                        CircleAvatar(
                          radius: 48,
                          backgroundColor: AppColors.brandDark2,
                          backgroundImage: _avatarFile != null ? FileImage(File(_avatarFile!.path)) : null,
                          child: _avatarFile == null ? const Icon(Icons.person, size: 48, color: Colors.white70) : null,
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
                AuthTextField(
                  label: l10n.t('profileSetup.fillProfile.fullNameLabel'),
                  controller: _fullNameController,
                  hint: l10n.t('profileSetup.fillProfile.fullNameHint'),
                  validator: (v) => (v == null || v.trim().isEmpty) ? l10n.t('profileSetup.fillProfile.nameRequired') : null,
                ),
                const SizedBox(height: 18),
                AuthTextField(
                  label: l10n.t('profileSetup.fillProfile.nicknameLabel'),
                  controller: _nicknameController,
                  hint: l10n.t('profileSetup.fillProfile.nicknameHint'),
                ),
                const SizedBox(height: 18),
                AuthTextField(
                  label: l10n.t('profileSetup.fillProfile.emailLabel'),
                  controller: _emailController,
                  hint: l10n.t('profileSetup.fillProfile.emailHint'),
                  keyboardType: TextInputType.emailAddress,
                ),
                const SizedBox(height: 18),
                AuthTextField(
                  label: l10n.t('profileSetup.fillProfile.phoneLabel'),
                  controller: _phoneController,
                  hint: l10n.t('profileSetup.fillProfile.phoneHint'),
                  keyboardType: TextInputType.phone,
                ),
                const SizedBox(height: 28),
                AuthPillButton(label: l10n.t('profileSetup.fillProfile.submit'), onPressed: _submit, isLoading: widget.isSubmitting),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

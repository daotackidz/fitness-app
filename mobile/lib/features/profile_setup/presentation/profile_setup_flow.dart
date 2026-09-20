import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/dio_client.dart';
import '../../../core/network/error_message.dart';
import '../../../core/theme/app_colors.dart';
import '../../auth/domain/auth_models.dart';
import '../../auth/presentation/auth_controller.dart';
import '../../auth/presentation/widgets/auth_widgets.dart';
import '../domain/onboarding_draft.dart';
import 'steps/activity_level_step.dart';
import 'steps/age_step.dart';
import 'steps/fill_profile_step.dart';
import 'steps/gender_step.dart';
import 'steps/goal_step.dart';
import 'steps/height_step.dart';
import 'steps/weight_step.dart';
import 'steps/welcome_step.dart';

/// Luong onboarding "profile setup" gom 8 buoc, chay mot lan duy nhat ngay
/// sau khi dang ky (isProfileComplete == false). Dieu huong giua cac buoc
/// hoan toan noi bo (khong dung Navigator/named routes) qua bien `_step`,
/// chi tien ve khi nguoi dung bam nut (khong vuot).
class ProfileSetupFlow extends ConsumerStatefulWidget {
  const ProfileSetupFlow({super.key, required this.onCompleted});

  final void Function(String? avatarUrl) onCompleted;

  @override
  ConsumerState<ProfileSetupFlow> createState() => _ProfileSetupFlowState();
}

class _ProfileSetupFlowState extends ConsumerState<ProfileSetupFlow> {
  static const _lastStep = 7;

  final _draft = OnboardingDraft();
  int _step = 0;
  bool _isSubmitting = false;

  void _goNext() {
    if (_step < _lastStep) setState(() => _step += 1);
  }

  void _goBack() {
    if (_step > 0) setState(() => _step -= 1);
  }

  String _formatDate(DateTime date) {
    final month = date.month.toString().padLeft(2, '0');
    final day = date.day.toString().padLeft(2, '0');
    return '${date.year}-$month-$day';
  }

  Future<void> _submit({
    required String fullName,
    required String nickname,
    required String phone,
    required XFile? avatarFile,
  }) async {
    setState(() => _isSubmitting = true);
    final dio = ref.read(dioProvider);
    final l10n = context.l10n;
    try {
      String? avatarUrl;
      if (avatarFile != null) {
        final form = FormData.fromMap({
          'file': await MultipartFile.fromFile(avatarFile.path, filename: avatarFile.name),
        });
        final avatarRes = await dio.post('/users/me/avatar', data: form);
        avatarUrl = (avatarRes.data['data'] as Map<String, dynamic>)['avatarUrl'] as String?;
      }

      final now = DateTime.now();
      final dateOfBirth = _formatDate(DateTime(now.year - _draft.age, now.month, now.day));

      await dio.post('/users/me/onboarding', data: {
        'fullName': fullName,
        'nickname': nickname.isEmpty ? null : nickname,
        'phone': phone.isEmpty ? null : phone,
        'gender': _draft.gender,
        'dateOfBirth': dateOfBirth,
        'heightCm': _draft.heightCm,
        'weightKg': _draft.weightKg,
        'fitnessGoal': _draft.fitnessGoal,
        'activityLevel': _draft.activityLevel,
      });

      if (!mounted) return;
      widget.onCompleted(avatarUrl);
    } on DioException catch (e) {
      if (!mounted) return;
      showAuthSnackBar(context, extractErrorMessage(e, fallback: l10n.t('profileSetup.error.saveFailed')), isError: true);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = ref.read(authControllerProvider).user;

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(child: _buildStep(user)),
    );
  }

  Widget _buildStep(AuthUser? user) {
    switch (_step) {
      case 0:
        return WelcomeStep(onNext: _goNext);
      case 1:
        return GenderStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case 2:
        return AgeStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case 3:
        return WeightStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case 4:
        return HeightStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case 5:
        return GoalStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case 6:
        return ActivityLevelStep(draft: _draft, onNext: _goNext, onBack: _goBack);
      case _lastStep:
        return FillProfileStep(
          initialFullName: user?.fullName ?? '',
          initialEmail: user?.email ?? '',
          initialPhone: user?.phone ?? '',
          onBack: _goBack,
          isSubmitting: _isSubmitting,
          onSubmit: ({required fullName, required nickname, required phone, required avatarFile}) => _submit(
            fullName: fullName,
            nickname: nickname,
            phone: phone,
            avatarFile: avatarFile,
          ),
        );
      default:
        return const SizedBox.shrink();
    }
  }
}

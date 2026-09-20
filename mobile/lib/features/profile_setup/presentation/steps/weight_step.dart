import 'package:flutter/material.dart';

import '../../../../core/localization/app_localizations.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../auth/presentation/widgets/auth_widgets.dart';
import '../../domain/onboarding_draft.dart';
import '../widgets/profile_setup_header.dart';

/// Buoc 4/8: chon can nang. Co toggle KG/LB chi anh huong hien thi; gia tri
/// goc luon duoc quy doi va luu theo kg (draft.weightKg) de gui len API.
class WeightStep extends StatefulWidget {
  final OnboardingDraft draft;
  final VoidCallback onNext;
  final VoidCallback onBack;

  const WeightStep({super.key, required this.draft, required this.onNext, required this.onBack});

  @override
  State<WeightStep> createState() => _WeightStepState();
}

class _WeightStepState extends State<WeightStep> {
  static const _kgToLb = 2.20462;
  static const _lbToKgFactor = 0.453592;

  late bool _isLb = widget.draft.weightUnitIsLb;

  double get _displayValue => _isLb ? widget.draft.weightKg * _kgToLb : widget.draft.weightKg;

  void _setUnit(bool isLb) {
    setState(() {
      _isLb = isLb;
      widget.draft.weightUnitIsLb = isLb;
    });
  }

  void _setValue(double displayValue) {
    setState(() {
      widget.draft.weightKg = _isLb ? displayValue * _lbToKgFactor : displayValue;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final minValue = _isLb ? 30 * _kgToLb : 30.0;
    final maxValue = _isLb ? 200 * _kgToLb : 200.0;
    final clampedValue = _displayValue.clamp(minValue, maxValue).toDouble();

    return Column(
      children: [
        ProfileSetupHeader(
          title: l10n.t('profileSetup.weight.title'),
          onBack: widget.onBack,
          description: l10n.t('profileSetup.weight.description'),
        ),
        Expanded(
          child: Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                _UnitToggle(isLb: _isLb, onChanged: _setUnit),
                const SizedBox(height: 24),
                Text(
                  '${clampedValue.round()}',
                  style: const TextStyle(color: Colors.white, fontSize: 64, fontWeight: FontWeight.w900),
                ),
                Text(
                  _isLb ? l10n.t('profileSetup.weight.unitLb') : l10n.t('profileSetup.weight.unitKg'),
                  style: const TextStyle(color: AppColors.brandLime, fontSize: 16, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: 280,
                  child: Slider(
                    value: clampedValue,
                    min: minValue,
                    max: maxValue,
                    activeColor: AppColors.brandLime,
                    inactiveColor: AppColors.brandDark2,
                    onChanged: _setValue,
                  ),
                ),
              ],
            ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(24),
          child: AuthPillButton(label: l10n.t('common.continue'), onPressed: widget.onNext),
        ),
      ],
    );
  }
}

class _UnitToggle extends StatelessWidget {
  final bool isLb;
  final ValueChanged<bool> onChanged;

  const _UnitToggle({required this.isLb, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Container(
      padding: const EdgeInsets.all(4),
      decoration: BoxDecoration(color: AppColors.brandDark2, borderRadius: BorderRadius.circular(999)),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          _UnitChip(label: l10n.t('profileSetup.weight.unitKg'), selected: !isLb, onTap: () => onChanged(false)),
          _UnitChip(label: l10n.t('profileSetup.weight.unitLb'), selected: isLb, onTap: () => onChanged(true)),
        ],
      ),
    );
  }
}

class _UnitChip extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _UnitChip({required this.label, required this.selected, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 150),
        padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 8),
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.transparent,
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(
          label,
          style: TextStyle(color: selected ? AppColors.brandDark : Colors.white70, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}

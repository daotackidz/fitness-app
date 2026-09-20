import 'package:flutter/material.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';

// TODO: thay bang anh that theo Figma UI Kit khi co asset, hien dung anh placeholder.
const _splashImageUrl = 'https://picsum.photos/id/1080/800/1400';

class SplashScreen extends StatefulWidget {
  final VoidCallback onSplashDone;

  const SplashScreen({super.key, required this.onSplashDone});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();
    Future.delayed(const Duration(seconds: 2), () {
      if (!mounted) return;
      widget.onSplashDone();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: Stack(
        fit: StackFit.expand,
        children: [
          Image.network(
            _splashImageUrl,
            fit: BoxFit.cover,
            errorBuilder: (context, error, stackTrace) => Container(color: AppColors.brandDark),
          ),
          DecoratedBox(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  AppColors.brandDark.withValues(alpha: 0.35),
                  AppColors.brandDark.withValues(alpha: 0.55),
                  AppColors.brandDark.withValues(alpha: 0.9),
                ],
              ),
            ),
          ),
          Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  context.l10n.t('onboarding.splash.welcomeTo'),
                  style: TextStyle(color: AppColors.brandLime, fontSize: 18, fontWeight: FontWeight.w600),
                ),
                const SizedBox(height: 8),
                RichText(
                  text: TextSpan(
                    style: const TextStyle(fontSize: 52, fontWeight: FontWeight.w900, height: 1),
                    children: [
                      TextSpan(text: 'FB', style: TextStyle(color: AppColors.brandLavender)),
                    ],
                  ),
                ),
                const SizedBox(height: 4),
                RichText(
                  text: TextSpan(
                    style: const TextStyle(fontSize: 30, fontWeight: FontWeight.w900, letterSpacing: 1),
                    children: [
                      TextSpan(text: 'FIT', style: TextStyle(color: AppColors.brandLavender)),
                      TextSpan(text: 'BODY', style: TextStyle(color: AppColors.brandLime)),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

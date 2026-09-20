import 'package:flutter/material.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';

class _OnboardingSlide {
  final String imageUrl;
  final IconData icon;
  final String titleKey;
  final String buttonLabelKey;

  const _OnboardingSlide({required this.imageUrl, required this.icon, required this.titleKey, required this.buttonLabelKey});
}

// TODO: thay bang anh that theo Figma UI Kit khi co asset, hien dung anh placeholder.
const _slides = [
  _OnboardingSlide(
    imageUrl: 'https://picsum.photos/id/1074/800/1400',
    icon: Icons.directions_run,
    titleKey: 'onboarding.slide1.title',
    buttonLabelKey: 'onboarding.next',
  ),
  _OnboardingSlide(
    imageUrl: 'https://picsum.photos/id/292/800/1400',
    icon: Icons.eco,
    titleKey: 'onboarding.slide2.title',
    buttonLabelKey: 'onboarding.next',
  ),
  _OnboardingSlide(
    imageUrl: 'https://picsum.photos/id/1082/800/1400',
    icon: Icons.groups,
    titleKey: 'onboarding.slide3.title',
    buttonLabelKey: 'onboarding.getStarted',
  ),
];

class OnboardingScreen extends StatefulWidget {
  final VoidCallback onFinished;

  const OnboardingScreen({super.key, required this.onFinished});

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {
  final _pageController = PageController();
  int _index = 0;

  void _next() {
    if (_index == _slides.length - 1) {
      widget.onFinished();
      return;
    }
    _pageController.nextPage(duration: const Duration(milliseconds: 300), curve: Curves.easeInOut);
  }

  void _skip() => widget.onFinished();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: PageView.builder(
        controller: _pageController,
        itemCount: _slides.length,
        onPageChanged: (i) => setState(() => _index = i),
        itemBuilder: (context, i) => _SlideView(
          slide: _slides[i],
          index: i,
          total: _slides.length,
          showSkip: i != _slides.length - 1,
          onSkip: _skip,
          onNext: _next,
        ),
      ),
    );
  }
}

class _SlideView extends StatelessWidget {
  final _OnboardingSlide slide;
  final int index;
  final int total;
  final bool showSkip;
  final VoidCallback onSkip;
  final VoidCallback onNext;

  const _SlideView({
    required this.slide,
    required this.index,
    required this.total,
    required this.showSkip,
    required this.onSkip,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final height = MediaQuery.of(context).size.height;
    final cardTop = height * 0.42;
    final cardHeight = height * 0.24;

    return Stack(
      fit: StackFit.expand,
      children: [
        Image.network(
          slide.imageUrl,
          fit: BoxFit.cover,
          errorBuilder: (context, error, stackTrace) => Container(color: AppColors.brandDark2),
        ),
        if (showSkip)
          SafeArea(
            child: Align(
              alignment: Alignment.topRight,
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 8),
                child: TextButton(
                  onPressed: onSkip,
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(l10n.t('onboarding.skip'), style: TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold)),
                      Icon(Icons.chevron_right, color: AppColors.brandLime),
                    ],
                  ),
                ),
              ),
            ),
          ),
        Positioned(
          top: cardTop,
          left: 0,
          right: 0,
          height: cardHeight,
          child: Container(
            decoration: const BoxDecoration(
              color: AppColors.brandLavender,
              borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
            ),
            padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 20),
            child: SingleChildScrollView(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(slide.icon, color: AppColors.brandLime, size: 32),
                  const SizedBox(height: 12),
                  Text(
                    l10n.t(slide.titleKey),
                    textAlign: TextAlign.center,
                    style: const TextStyle(color: Colors.white, fontSize: 20, fontWeight: FontWeight.w800, height: 1.3),
                  ),
                  const SizedBox(height: 14),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: List.generate(total, (i) {
                      final active = i == index;
                      return AnimatedContainer(
                        duration: const Duration(milliseconds: 200),
                        margin: const EdgeInsets.symmetric(horizontal: 3),
                        width: active ? 20 : 6,
                        height: 6,
                        decoration: BoxDecoration(
                          color: active ? AppColors.brandLime : Colors.white.withValues(alpha: 0.5),
                          borderRadius: BorderRadius.circular(3),
                        ),
                      );
                    }),
                  ),
                ],
              ),
            ),
          ),
        ),
        Positioned(
          top: cardTop + cardHeight - 26,
          left: 0,
          right: 0,
          child: Center(
            child: SizedBox(
              width: 200,
              height: 52,
              child: ElevatedButton(
                onPressed: onNext,
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.brandDark,
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(999),
                    side: const BorderSide(color: Colors.white24),
                  ),
                ),
                child: Text(l10n.t(slide.buttonLabelKey), style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

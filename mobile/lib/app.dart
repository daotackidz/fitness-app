import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import 'core/localization/app_localizations.dart';
import 'core/localization/locale_controller.dart';
import 'core/network/dio_client.dart';
import 'core/theme/app_colors.dart';
import 'features/auth/presentation/auth_controller.dart';
import 'features/auth/presentation/login_screen.dart';
import 'features/onboarding/presentation/onboarding_screen.dart';
import 'features/onboarding/presentation/splash_screen.dart';
import 'features/profile_setup/presentation/profile_setup_flow.dart';
import 'features/shell/main_shell.dart';

final onboardingSeenProvider = FutureProvider<bool>((ref) {
  return ref.watch(secureStorageProvider).isOnboardingSeen();
});

class FitBodyApp extends ConsumerWidget {
  const FitBodyApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final locale = ref.watch(localeControllerProvider);

    return MaterialApp(
      title: 'FitBody',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorSchemeSeed: AppColors.brandPurple,
        useMaterial3: true,
        scaffoldBackgroundColor: AppColors.brandDark,
        fontFamily: GoogleFonts.leagueSpartan().fontFamily,
        textTheme: GoogleFonts.leagueSpartanTextTheme().copyWith(
          displayLarge: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          displayMedium: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          displaySmall: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          headlineLarge: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          headlineMedium: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          headlineSmall: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.bold)),
          titleLarge: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.w600)),
          titleMedium: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.w600)),
          titleSmall: GoogleFonts.poppins(textStyle: const TextStyle(fontWeight: FontWeight.w600)),
        ),
      ),
      locale: locale,
      supportedLocales: AppLocalizations.supportedLocales,
      localizationsDelegates: const [
        AppLocalizations.delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      home: const _RootGate(),
    );
  }
}

class _RootGate extends ConsumerWidget {
  const _RootGate();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final onboardingSeenAsync = ref.watch(onboardingSeenProvider);

    return onboardingSeenAsync.when(
      loading: () => const Scaffold(body: Center(child: CircularProgressIndicator())),
      error: (_, _) => const _AuthGate(),
      data: (seen) {
        if (seen) return const _AuthGate();

        return _OnboardingFlow(
          onFinished: () async {
            await ref.read(secureStorageProvider).markOnboardingSeen();
            ref.invalidate(onboardingSeenProvider);
          },
        );
      },
    );
  }
}

// Keeps Splash + Onboarding under one Element (no Navigator route swap), so
// the onFinished closure's `ref` (owned by the ancestor _RootGate) stays alive.
class _OnboardingFlow extends StatefulWidget {
  final VoidCallback onFinished;

  const _OnboardingFlow({required this.onFinished});

  @override
  State<_OnboardingFlow> createState() => _OnboardingFlowState();
}

class _OnboardingFlowState extends State<_OnboardingFlow> {
  bool _showOnboarding = false;

  @override
  Widget build(BuildContext context) {
    if (_showOnboarding) {
      return OnboardingScreen(onFinished: widget.onFinished);
    }
    return SplashScreen(onSplashDone: () => setState(() => _showOnboarding = true));
  }
}

class _AuthGate extends ConsumerWidget {
  const _AuthGate();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final authState = ref.watch(authControllerProvider);

    if (authState.isInitializing) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    if (!authState.isAuthenticated) {
      return const LoginScreen();
    }

    if (authState.user!.isProfileComplete) {
      return const MainShell();
    }

    return ProfileSetupFlow(
      onCompleted: (avatarUrl) =>
          ref.read(authControllerProvider.notifier).markProfileComplete(avatarUrl: avatarUrl),
    );
  }
}

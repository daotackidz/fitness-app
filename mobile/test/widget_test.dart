import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:fitbody_app/app.dart';

void main() {
  testWidgets('First launch shows splash/onboarding welcome screen', (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [onboardingSeenProvider.overrideWith((ref) async => false)],
        child: const FitBodyApp(),
      ),
    );
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 100));

    expect(find.text('Chào mừng đến với'), findsOneWidget);

    // Consume the splash screen's auto-advance timer so no pending Timer remains at teardown.
    await tester.pump(const Duration(seconds: 3));
  });

  testWidgets('Returning user (onboarding already seen) goes to login screen', (WidgetTester tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [onboardingSeenProvider.overrideWith((ref) async => true)],
        child: const FitBodyApp(),
      ),
    );
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 100));

    expect(find.text('Chào mừng'), findsOneWidget);
    expect(find.text('Tên đăng nhập hoặc email'), findsOneWidget);
  });
}

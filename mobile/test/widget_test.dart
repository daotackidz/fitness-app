import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:fitbody_app/app.dart';

void main() {
  testWidgets('App shows login screen when not authenticated', (WidgetTester tester) async {
    await tester.pumpWidget(const ProviderScope(child: FitBodyApp()));
    await tester.pump();

    expect(find.text('FitBody'), findsOneWidget);
  });
}

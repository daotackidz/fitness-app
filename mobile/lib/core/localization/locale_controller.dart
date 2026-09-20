import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../network/dio_client.dart';

class LocaleController extends StateNotifier<Locale> {
  final Ref _ref;

  LocaleController(this._ref) : super(const Locale('vi')) {
    _restore();
  }

  Future<void> _restore() async {
    try {
      final saved = await _ref.read(secureStorageProvider).readLanguageCode();
      if (saved != null) {
        state = Locale(saved);
      }
    } catch (_) {
      // Ignore (e.g. secure storage unavailable in tests); keep the default locale.
    }
  }

  Future<void> setLocale(Locale locale) async {
    state = locale;
    await _ref.read(secureStorageProvider).saveLanguageCode(locale.languageCode);
  }
}

final localeControllerProvider = StateNotifierProvider<LocaleController, Locale>((ref) {
  return LocaleController(ref);
});

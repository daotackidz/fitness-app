import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import 'strings/admin_strings.dart';
import 'strings/auth_strings.dart';
import 'strings/common_strings.dart';
import 'strings/community_strings.dart';
import 'strings/content_strings.dart';
import 'strings/home_strings.dart';
import 'strings/notification_strings.dart';
import 'strings/nutrition_strings.dart';
import 'strings/onboarding_strings.dart';
import 'strings/profile_setup_strings.dart';
import 'strings/profile_strings.dart';
import 'strings/search_strings.dart';
import 'strings/shell_strings.dart';
import 'strings/support_strings.dart';
import 'strings/workout_strings.dart';

class AppLocalizations {
  final Locale locale;
  late final Map<String, String> _strings;

  AppLocalizations(this.locale) {
    final isVi = locale.languageCode == 'vi';
    _strings = {
      ...(isVi ? commonStringsVi : commonStringsEn),
      ...(isVi ? authStringsVi : authStringsEn),
      ...(isVi ? onboardingStringsVi : onboardingStringsEn),
      ...(isVi ? profileStringsVi : profileStringsEn),
      ...(isVi ? profileSetupStringsVi : profileSetupStringsEn),
      ...(isVi ? shellStringsVi : shellStringsEn),
      ...(isVi ? adminStringsVi : adminStringsEn),
      ...(isVi ? workoutStringsVi : workoutStringsEn),
      ...(isVi ? nutritionStringsVi : nutritionStringsEn),
      ...(isVi ? contentStringsVi : contentStringsEn),
      ...(isVi ? homeStringsVi : homeStringsEn),
      ...(isVi ? communityStringsVi : communityStringsEn),
      ...(isVi ? notificationStringsVi : notificationStringsEn),
      ...(isVi ? supportStringsVi : supportStringsEn),
      ...(isVi ? searchStringsVi : searchStringsEn),
    };
  }

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  /// Looks up [key]; falls back to the key itself if missing so a gap is
  /// visible instead of crashing. Supports `{placeholder}` substitution.
  String t(String key, [Map<String, String>? params]) {
    var value = _strings[key] ?? key;
    if (params != null) {
      for (final entry in params.entries) {
        value = value.replaceAll('{${entry.key}}', entry.value);
      }
    }
    return value;
  }

  static const List<Locale> supportedLocales = [Locale('en'), Locale('vi')];

  static const LocalizationsDelegate<AppLocalizations> delegate = _AppLocalizationsDelegate();
}

class _AppLocalizationsDelegate extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  bool isSupported(Locale locale) => AppLocalizations.supportedLocales.any((l) => l.languageCode == locale.languageCode);

  @override
  Future<AppLocalizations> load(Locale locale) => SynchronousFuture(AppLocalizations(locale));

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

extension AppLocalizationsX on BuildContext {
  AppLocalizations get l10n => AppLocalizations.of(this);
}

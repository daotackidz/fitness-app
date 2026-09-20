import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class SecureStorageService {
  static const _refreshTokenKey = 'refresh_token';
  static const _onboardingSeenKey = 'onboarding_seen';
  static const _languageCodeKey = 'language_code';

  final _storage = const FlutterSecureStorage();

  Future<void> saveRefreshToken(String token) => _storage.write(key: _refreshTokenKey, value: token);

  Future<String?> readRefreshToken() => _storage.read(key: _refreshTokenKey);

  Future<void> clear() => _storage.delete(key: _refreshTokenKey);

  Future<bool> isOnboardingSeen() async => (await _storage.read(key: _onboardingSeenKey)) == 'true';

  Future<void> markOnboardingSeen() => _storage.write(key: _onboardingSeenKey, value: 'true');

  Future<String?> readLanguageCode() => _storage.read(key: _languageCodeKey);

  Future<void> saveLanguageCode(String languageCode) => _storage.write(key: _languageCodeKey, value: languageCode);
}

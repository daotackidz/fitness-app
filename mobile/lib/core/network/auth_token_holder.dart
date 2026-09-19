import 'package:flutter_riverpod/flutter_riverpod.dart';

class AuthTokenHolder {
  String? accessToken;
}

final authTokenHolderProvider = Provider<AuthTokenHolder>((ref) => AuthTokenHolder());

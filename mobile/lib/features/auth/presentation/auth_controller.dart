import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/auth_token_holder.dart';
import '../../../core/network/dio_client.dart';
import '../data/auth_api.dart';
import '../domain/auth_models.dart';

class AuthState {
  final AuthUser? user;
  final bool isLoading;
  final bool isInitializing;

  const AuthState({this.user, this.isLoading = false, this.isInitializing = true});

  bool get isAuthenticated => user != null;

  AuthState copyWith({AuthUser? user, bool? isLoading, bool? isInitializing, bool clearUser = false}) => AuthState(
        user: clearUser ? null : (user ?? this.user),
        isLoading: isLoading ?? this.isLoading,
        isInitializing: isInitializing ?? this.isInitializing,
      );
}

final authApiProvider = Provider<AuthApi>((ref) => AuthApi(ref.watch(dioProvider)));
final authRepositoryProvider = Provider<AuthRepository>((ref) => AuthRepository(ref.watch(authApiProvider)));

class AuthController extends StateNotifier<AuthState> {
  final AuthRepository _repository;
  final AuthTokenHolder _tokenHolder;
  final Ref _ref;

  AuthController(this._repository, this._tokenHolder, this._ref) : super(const AuthState()) {
    _tryRestoreSession();
  }

  Future<void> _tryRestoreSession() async {
    // TODO: khi co refresh token da luu, goi /auth/refresh-token de tu dang nhap lai.
    // Hien tai chua co endpoint "whoami" bang refresh token don le nen bo qua, yeu cau dang nhap lai.
    state = state.copyWith(isInitializing: false);
  }

  Future<void> login(String email, String password) async {
    state = state.copyWith(isLoading: true);
    try {
      final result = await _repository.login(email: email, password: password);
      await _applySession(result);
    } finally {
      state = state.copyWith(isLoading: false);
    }
  }

  Future<void> register(String fullName, String email, String? phone, String password) async {
    state = state.copyWith(isLoading: true);
    try {
      final result = await _repository.register(fullName: fullName, email: email, phone: phone, password: password);
      await _applySession(result);
    } finally {
      state = state.copyWith(isLoading: false);
    }
  }

  Future<void> logout() async {
    final storage = _ref.read(secureStorageProvider);
    final refreshToken = await storage.readRefreshToken();
    if (refreshToken != null) {
      try {
        await _repository.logout(refreshToken);
      } catch (_) {
        // bo qua loi logout phia server, van xoa session cuc bo
      }
    }
    _tokenHolder.accessToken = null;
    await storage.clear();
    state = state.copyWith(clearUser: true);
  }

  Future<void> _applySession(AuthResult result) async {
    _tokenHolder.accessToken = result.accessToken;
    await _ref.read(secureStorageProvider).saveRefreshToken(result.refreshToken);
    state = state.copyWith(user: result.user);
  }
}

final authControllerProvider = StateNotifierProvider<AuthController, AuthState>((ref) {
  return AuthController(
    ref.watch(authRepositoryProvider),
    ref.watch(authTokenHolderProvider),
    ref,
  );
});

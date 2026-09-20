import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/theme/app_colors.dart';
import '../admin/presentation/admin_home_screen.dart';
import '../auth/presentation/auth_controller.dart';
import '../content/presentation/content_screen.dart';
import '../home/presentation/home_screen.dart';
import '../profile/presentation/favorites_screen.dart';
import '../support/presentation/support_screen.dart';

class MainShell extends ConsumerStatefulWidget {
  const MainShell({super.key});

  @override
  ConsumerState<MainShell> createState() => _MainShellState();
}

class _MainShellState extends ConsumerState<MainShell> {
  int _index = 0;

  @override
  Widget build(BuildContext context) {
    final isStaff = ref.watch(authControllerProvider.select((s) => s.user?.isStaff ?? false));

    final pages = [
      const HomeScreen(),
      const ContentScreen(),
      const FavoritesScreen(showBackButton: false),
      const SupportScreen(),
      if (isStaff) const AdminHomeScreen(),
    ];
    final icons = [
      Icons.home,
      Icons.description,
      Icons.star,
      Icons.headset_mic,
      if (isStaff) Icons.admin_panel_settings,
    ];
    final safeIndex = _index >= pages.length ? 0 : _index;

    final bottomInset = MediaQuery.of(context).padding.bottom;

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: IndexedStack(index: safeIndex, children: pages),
      bottomNavigationBar: Container(
        decoration: const BoxDecoration(
          color: AppColors.brandLavender,
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        padding: EdgeInsets.fromLTRB(0, 10, 0, 10 + bottomInset),
        child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              for (var i = 0; i < icons.length; i++)
                GestureDetector(
                  onTap: () => setState(() => _index = i),
                  behavior: HitTestBehavior.opaque,
                  child: Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: safeIndex == i ? Colors.white.withValues(alpha: 0.25) : Colors.transparent,
                      shape: BoxShape.circle,
                    ),
                    child: Icon(icons[i], color: Colors.white, size: 24),
                  ),
                ),
            ],
          ),
      ),
    );
  }
}

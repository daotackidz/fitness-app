import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../auth/presentation/auth_controller.dart';
import '../admin/presentation/admin_home_screen.dart';
import '../community/presentation/community_screen.dart';
import '../content/presentation/content_screen.dart';
import '../nutrition/presentation/nutrition_screen.dart';
import '../profile/presentation/profile_screen.dart';
import '../support/presentation/support_screen.dart';
import '../workout/presentation/workout_screen.dart';

class MainShell extends ConsumerStatefulWidget {
  const MainShell({super.key});

  @override
  ConsumerState<MainShell> createState() => _MainShellState();
}

class _MainShellState extends ConsumerState<MainShell> {
  int _index = 0;

  @override
  Widget build(BuildContext context) {
    final user = ref.watch(authControllerProvider.select((s) => s.user));
    final isStaff = user?.isStaff ?? false;

    final pages = [
      const WorkoutScreen(),
      const NutritionScreen(),
      const ContentScreen(),
      const CommunityScreen(),
      const SupportScreen(),
      const ProfileScreen(),
      if (isStaff) const AdminHomeScreen(),
    ];

    final destinations = [
      const NavigationDestination(icon: Icon(Icons.fitness_center), label: 'Workout'),
      const NavigationDestination(icon: Icon(Icons.restaurant), label: 'Dinh duong'),
      const NavigationDestination(icon: Icon(Icons.article), label: 'Noi dung'),
      const NavigationDestination(icon: Icon(Icons.people), label: 'Cong dong'),
      const NavigationDestination(icon: Icon(Icons.support_agent), label: 'Ho tro'),
      const NavigationDestination(icon: Icon(Icons.person), label: 'Ca nhan'),
      if (isStaff) const NavigationDestination(icon: Icon(Icons.admin_panel_settings), label: 'Quan tri'),
    ];

    final safeIndex = _index >= pages.length ? 0 : _index;

    return Scaffold(
      body: pages[safeIndex],
      bottomNavigationBar: NavigationBar(
        selectedIndex: safeIndex,
        onDestinationSelected: (i) => setState(() => _index = i),
        destinations: destinations,
      ),
    );
  }
}

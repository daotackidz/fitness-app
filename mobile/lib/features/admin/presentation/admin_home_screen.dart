import 'package:flutter/material.dart';

import 'admin_dashboard_screen.dart';
import 'admin_reported_contents_screen.dart';
import 'admin_support_screen.dart';
import 'admin_user_lookup_screen.dart';

// Quan tri rut gon tren mobile: dashboard, kiem duyet, ho tro, khoa/mo khoa nhanh 1 tai khoan.
// CRUD noi dung day du (exercises/routines/meal-plans/articles/videos/faqs/challenges) va doi role
// CHI lam tren Admin Web, khong xay dung lai tren mobile.
class AdminHomeScreen extends StatelessWidget {
  const AdminHomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 4,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Quan tri'),
          bottom: const TabBar(
            isScrollable: true,
            tabs: [
              Tab(text: 'Dashboard'),
              Tab(text: 'Kiem duyet'),
              Tab(text: 'Ho tro'),
              Tab(text: 'Nguoi dung'),
            ],
          ),
        ),
        body: const TabBarView(
          children: [
            AdminDashboardScreen(),
            AdminReportedContentsScreen(),
            AdminSupportScreen(),
            AdminUserLookupScreen(),
          ],
        ),
      ),
    );
  }
}

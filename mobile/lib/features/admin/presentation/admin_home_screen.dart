import 'package:flutter/material.dart';

import '../../../core/localization/app_localizations.dart';
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
    final l10n = context.l10n;

    return DefaultTabController(
      length: 4,
      child: Scaffold(
        appBar: AppBar(
          title: Text(l10n.t('admin.home.title')),
          bottom: TabBar(
            isScrollable: true,
            tabs: [
              Tab(text: l10n.t('admin.home.tab.dashboard')),
              Tab(text: l10n.t('admin.home.tab.moderation')),
              Tab(text: l10n.t('admin.home.tab.support')),
              Tab(text: l10n.t('admin.home.tab.users')),
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

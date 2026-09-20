import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import 'profile_providers.dart';
import 'widgets/profile_widgets.dart';

class HelpFaqScreen extends ConsumerStatefulWidget {
  const HelpFaqScreen({super.key});

  @override
  ConsumerState<HelpFaqScreen> createState() => _HelpFaqScreenState();
}

class _HelpFaqScreenState extends ConsumerState<HelpFaqScreen> {
  int _tab = 0;
  String? _category;
  final _searchController = TextEditingController();
  String _query = '';

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Column(
          children: [
            ProfilePushHeader(title: l10n.t('profile.help.title')),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: Row(
                children: [
                  Expanded(child: _TopTab(label: l10n.t('profile.help.faqTab'), selected: _tab == 0, onTap: () => setState(() => _tab = 0))),
                  const SizedBox(width: 12),
                  Expanded(child: _TopTab(label: l10n.t('profile.help.contactTab'), selected: _tab == 1, onTap: () => setState(() => _tab = 1))),
                ],
              ),
            ),
            const SizedBox(height: 16),
            Expanded(child: _tab == 0 ? _buildFaqTab(context) : const _ContactUsTab()),
          ],
        ),
      ),
    );
  }

  Widget _buildFaqTab(BuildContext context) {
    final l10n = context.l10n;
    final faqsAsync = ref.watch(faqsProvider((category: _category, q: _query.isEmpty ? null : _query)));

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextField(
            controller: _searchController,
            style: const TextStyle(color: Colors.white),
            decoration: InputDecoration(
              hintText: l10n.t('common.search'),
              hintStyle: TextStyle(color: Colors.grey.shade500),
              filled: true,
              fillColor: AppColors.brandDark2,
              prefixIcon: const Icon(Icons.search, color: Colors.grey),
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
            ),
            onSubmitted: (v) => setState(() => _query = v.trim()),
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              _CategoryPill(
                label: l10n.t('profile.help.general'),
                selected: _category == 'general',
                onTap: () => setState(() => _category = _category == 'general' ? null : 'general'),
              ),
              const SizedBox(width: 10),
              _CategoryPill(
                label: l10n.t('profile.help.account'),
                selected: _category == 'account',
                onTap: () => setState(() => _category = _category == 'account' ? null : 'account'),
              ),
              const SizedBox(width: 10),
              _CategoryPill(
                label: l10n.t('profile.help.services'),
                selected: _category == 'services',
                onTap: () => setState(() => _category = _category == 'services' ? null : 'services'),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: faqsAsync.when(
              loading: () => const Center(child: CircularProgressIndicator()),
              error: (err, _) => Center(
                child: Text(
                  l10n.t('common.error.loadFailed', {'error': err.toString()}),
                  style: const TextStyle(color: Colors.white),
                ),
              ),
              data: (items) {
                if (items.isEmpty) {
                  return Center(child: Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400)));
                }
                return ListView.separated(
                  itemCount: items.length,
                  separatorBuilder: (_, _) => const SizedBox(height: 10),
                  itemBuilder: (context, index) => _FaqTile(item: items[index]),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}

class _TopTab extends StatelessWidget {
  const _TopTab({required this.label, required this.selected, required this.onTap});

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 12),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : AppColors.brandDark2,
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(label, style: TextStyle(color: selected ? AppColors.brandDark : Colors.white, fontWeight: FontWeight.bold)),
      ),
    );
  }
}

class _CategoryPill extends StatelessWidget {
  const _CategoryPill({required this.label, required this.selected, required this.onTap});

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.transparent,
          border: Border.all(color: selected ? AppColors.brandLime : Colors.grey.shade600),
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(label, style: TextStyle(color: selected ? AppColors.brandDark : Colors.white, fontSize: 12, fontWeight: FontWeight.w600)),
      ),
    );
  }
}

class _FaqTile extends StatelessWidget {
  const _FaqTile({required this.item});

  final Map<String, dynamic> item;

  @override
  Widget build(BuildContext context) {
    final question = item['question'] as String? ?? '';
    final answer = item['answer'] as String? ?? '';
    return Container(
      decoration: BoxDecoration(color: AppColors.brandDark2, borderRadius: BorderRadius.circular(16)),
      child: Theme(
        data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
        child: ExpansionTile(
          title: Text(question, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 14)),
          iconColor: AppColors.brandLime,
          collapsedIconColor: AppColors.brandLavender,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
              child: Align(
                alignment: Alignment.centerLeft,
                child: Text(answer, style: TextStyle(color: Colors.grey.shade300, fontSize: 13, height: 1.4)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ContactUsTab extends StatelessWidget {
  const _ContactUsTab();

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final items = [
      (Icons.support_agent, l10n.t('profile.help.customerService')),
      (Icons.language, l10n.t('profile.help.website')),
      (Icons.chat, l10n.t('profile.help.whatsapp')),
      (Icons.facebook, l10n.t('profile.help.facebook')),
      (Icons.camera_alt_outlined, l10n.t('profile.help.instagram')),
    ];
    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      children: [
        for (final entry in items) ProfileMenuRow(icon: entry.$1, label: entry.$2, onTap: () {}),
      ],
    );
  }
}

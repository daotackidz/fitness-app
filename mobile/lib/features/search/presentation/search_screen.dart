import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../notification/presentation/notification_screen.dart';
import '../../profile/presentation/profile_screen.dart';
import 'search_providers.dart';

const _workoutSuggestions = ['Circuit', 'Split', 'Challenge', 'Legs', 'Cardio'];
const _nutritionSuggestions = ['Breakfast', 'Yogurt', 'Vegetarian', 'Smoothie', 'Chicken'];

class SearchScreen extends ConsumerStatefulWidget {
  const SearchScreen({super.key});

  @override
  ConsumerState<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends ConsumerState<SearchScreen> {
  final _controller = TextEditingController();
  String _type = 'all';
  bool _loading = false;
  Map<String, dynamic>? _results;
  int _requestId = 0;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _search(String query) async {
    final trimmed = query.trim();
    if (trimmed.isEmpty) {
      setState(() {
        _results = null;
        _loading = false;
      });
      return;
    }
    final requestId = ++_requestId;
    setState(() => _loading = true);
    try {
      final response = await ref.read(searchApiProvider).search({'q': trimmed, 'type': _type});
      if (requestId != _requestId) return;
      final data = response.data['data'] as Map<String, dynamic>;
      setState(() {
        _results = data;
        _loading = false;
      });
    } catch (_) {
      if (requestId != _requestId) return;
      setState(() {
        _results = {};
        _loading = false;
      });
    }
  }

  void _selectType(String type) {
    if (_type == type) return;
    setState(() => _type = type);
    if (_controller.text.trim().isNotEmpty) _search(_controller.text);
  }

  void _selectSuggestion(String term) {
    _controller.text = term;
    setState(() {});
    _search(term);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final hasQuery = _controller.text.trim().isNotEmpty;
    final suggestions = _type == 'nutrition' ? _nutritionSuggestions : _workoutSuggestions;

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                children: [
                  IconButton(
                    padding: EdgeInsets.zero,
                    constraints: const BoxConstraints(),
                    onPressed: () => Navigator.of(context).maybePop(),
                    icon: const Icon(Icons.arrow_back_ios_new, color: AppColors.brandLime, size: 18),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      l10n.t('search.title'),
                      style: GoogleFonts.poppins(color: Colors.white, fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                  ),
                  _HeaderIconButton(
                    icon: Icons.notifications_none,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NotificationScreen())),
                  ),
                  const SizedBox(width: 8),
                  _HeaderIconButton(
                    icon: Icons.person_outline,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ProfileScreen())),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Container(
                decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(999)),
                child: TextField(
                  controller: _controller,
                  onChanged: (value) {
                    setState(() {});
                    _search(value);
                  },
                  style: const TextStyle(color: AppColors.brandDark),
                  decoration: InputDecoration(
                    hintText: l10n.t('search.hint'),
                    hintStyle: TextStyle(color: Colors.grey.shade500),
                    prefixIcon: Icon(Icons.search, color: Colors.grey.shade500),
                    border: InputBorder.none,
                    contentPadding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  _FilterPill(label: l10n.t('search.filter.all'), selected: _type == 'all', onTap: () => _selectType('all')),
                  const SizedBox(width: 10),
                  _FilterPill(label: l10n.t('search.filter.workout'), selected: _type == 'workout', onTap: () => _selectType('workout')),
                  const SizedBox(width: 10),
                  _FilterPill(label: l10n.t('search.filter.nutrition'), selected: _type == 'nutrition', onTap: () => _selectType('nutrition')),
                ],
              ),
              const SizedBox(height: 20),
              Expanded(
                child: !hasQuery
                    ? _TopSearches(suggestions: suggestions, onTap: _selectSuggestion)
                    : _loading
                        ? const Center(child: CircularProgressIndicator())
                        : _SearchResults(results: _results ?? const {}),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _TopSearches extends StatelessWidget {
  const _TopSearches({required this.suggestions, required this.onTap});

  final List<String> suggestions;
  final ValueChanged<String> onTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return ListView(
      children: [
        Text(
          l10n.t('search.topSearches'),
          style: const TextStyle(color: AppColors.brandLime, fontWeight: FontWeight.bold, fontSize: 15),
        ),
        const SizedBox(height: 12),
        for (final term in suggestions)
          InkWell(
            onTap: () => onTap(term),
            borderRadius: BorderRadius.circular(12),
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 10),
              child: Row(
                children: [
                  Container(
                    width: 34,
                    height: 34,
                    decoration: const BoxDecoration(color: AppColors.brandLime, shape: BoxShape.circle),
                    child: const Icon(Icons.search, color: AppColors.brandDark, size: 16),
                  ),
                  const SizedBox(width: 14),
                  Text(term, style: const TextStyle(color: Colors.white, fontSize: 15)),
                ],
              ),
            ),
          ),
      ],
    );
  }
}

class _SearchResults extends StatelessWidget {
  const _SearchResults({required this.results});

  final Map<String, dynamic> results;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final exercises = results['exercises'] as List<dynamic>? ?? [];
    final routines = results['routines'] as List<dynamic>? ?? [];
    final foods = results['foods'] as List<dynamic>? ?? [];
    final mealPlans = results['mealPlans'] as List<dynamic>? ?? [];

    final items = <Widget>[
      for (final e in exercises)
        _ResultCard(
          icon: Icons.fitness_center,
          title: (e as Map<String, dynamic>)['name'] as String? ?? '',
          subtitle: '${e['muscleGroup'] ?? ''} · ${e['difficultyLevel'] ?? ''}',
        ),
      for (final r in routines)
        _ResultCard(
          icon: Icons.list_alt,
          title: (r as Map<String, dynamic>)['name'] as String? ?? '',
          subtitle: '${r['level'] ?? ''}',
        ),
      for (final f in foods)
        _ResultCard(
          icon: Icons.restaurant,
          title: (f as Map<String, dynamic>)['name'] as String? ?? '',
          subtitle: '${f['calories'] ?? 0} Cal',
        ),
      for (final m in mealPlans)
        _ResultCard(
          icon: Icons.menu_book,
          title: (m as Map<String, dynamic>)['name'] as String? ?? '',
          subtitle: '${m['goal'] ?? ''}',
        ),
    ];

    if (items.isEmpty) {
      return Center(child: Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400)));
    }

    return ListView.separated(
      itemCount: items.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) => items[index],
    );
  }
}

class _ResultCard extends StatelessWidget {
  const _ResultCard({required this.icon, required this.title, required this.subtitle});

  final IconData icon;
  final String title;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(16)),
      padding: const EdgeInsets.all(14),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: const BoxDecoration(color: AppColors.brandLavender, shape: BoxShape.circle),
            child: Icon(icon, color: AppColors.brandDark, size: 20),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 15),
                ),
                const SizedBox(height: 4),
                Text(subtitle, style: TextStyle(color: Colors.grey.shade600, fontSize: 12)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _HeaderIconButton extends StatelessWidget {
  const _HeaderIconButton({required this.icon, this.onTap});

  final IconData icon;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 36,
        height: 36,
        decoration: BoxDecoration(color: AppColors.brandDark2, borderRadius: BorderRadius.circular(10)),
        child: Icon(icon, color: AppColors.brandLavender, size: 18),
      ),
    );
  }
}

class _FilterPill extends StatelessWidget {
  const _FilterPill({required this.label, required this.selected, required this.onTap});

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 8),
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.transparent,
          border: Border.all(color: selected ? AppColors.brandLime : Colors.grey.shade600),
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(
          label,
          style: TextStyle(color: selected ? AppColors.brandDark : Colors.white, fontWeight: FontWeight.w600, fontSize: 13),
        ),
      ),
    );
  }
}

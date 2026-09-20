import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../notification/presentation/notification_screen.dart';
import '../../profile/presentation/profile_screen.dart';
import '../../search/presentation/search_screen.dart';
import 'routine_detail_screen.dart';
import 'workout_providers.dart';

const _levelOptions = [
  ('Beginner', 'workout.filter.beginner'),
  ('Intermediate', 'workout.filter.intermediate'),
  ('Advanced', 'workout.filter.advance'),
];

String _levelDisplayLabel(String value, AppLocalizations l10n) {
  for (final option in _levelOptions) {
    if (option.$1 == value) return l10n.t(option.$2);
  }
  return value;
}

class WorkoutScreen extends ConsumerStatefulWidget {
  const WorkoutScreen({super.key});

  @override
  ConsumerState<WorkoutScreen> createState() => _WorkoutScreenState();
}

class _WorkoutScreenState extends ConsumerState<WorkoutScreen> {
  final Set<String> _selectedLevels = {};
  final Set<String> _optimisticallyFavorited = {};

  String get _levelParam => (_selectedLevels.toList()..sort()).join(',');

  Future<void> _addFavorite(String id) async {
    if (_optimisticallyFavorited.contains(id)) return;
    setState(() => _optimisticallyFavorited.add(id));
    try {
      await ref.read(workoutApiProvider).addFavorite({'favoritableType': 'Routine', 'favoritableId': id});
    } on DioException catch (e) {
      if (e.response?.statusCode != 409) {
        setState(() => _optimisticallyFavorited.remove(id));
      }
    }
  }

  bool _isFavorited(Map<String, dynamic> routine) {
    return routine['isFavorited'] == true || _optimisticallyFavorited.contains(routine['id'] as String);
  }

  void _toggleLevel(String value) {
    setState(() {
      if (_selectedLevels.contains(value)) {
        _selectedLevels.remove(value);
      } else {
        _selectedLevels.add(value);
      }
    });
  }

  void _openRoutine(String routineId) {
    Navigator.of(context).push(MaterialPageRoute(builder: (_) => RoutineDetailScreen(routineId: routineId)));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final levelParam = _levelParam;
    final routinesAsync = ref.watch(filteredRoutinesProvider(levelParam));

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
                      l10n.t('workout.title'),
                      style: GoogleFonts.poppins(color: Colors.white, fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                  ),
                  _HeaderIconButton(
                    icon: Icons.search,
                    onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const SearchScreen())),
                  ),
                  const SizedBox(width: 8),
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
              const SizedBox(height: 18),
              Row(
                children: [
                  for (var i = 0; i < _levelOptions.length; i++) ...[
                    Expanded(
                      child: _LevelPill(
                        label: l10n.t(_levelOptions[i].$2),
                        selected: _selectedLevels.contains(_levelOptions[i].$1),
                        onTap: () => _toggleLevel(_levelOptions[i].$1),
                      ),
                    ),
                    if (i != _levelOptions.length - 1) const SizedBox(width: 10),
                  ],
                ],
              ),
              const SizedBox(height: 18),
              Expanded(
                child: RefreshIndicator(
                  onRefresh: () async => ref.invalidate(filteredRoutinesProvider(levelParam)),
                  child: routinesAsync.when(
                    loading: () => const Center(child: CircularProgressIndicator()),
                    error: (err, _) => ListView(
                      children: [
                        Padding(
                          padding: const EdgeInsets.symmetric(vertical: 120),
                          child: Center(
                            child: Text(
                              l10n.t('common.error.loadFailed', {'error': err.toString()}),
                              style: const TextStyle(color: Colors.white),
                              textAlign: TextAlign.center,
                            ),
                          ),
                        ),
                      ],
                    ),
                    data: (routines) {
                      Map<String, dynamic>? featured;
                      for (final r in routines) {
                        if (r['isFeatured'] == true) {
                          featured = r;
                          break;
                        }
                      }
                      final rest = routines.where((r) => featured == null || r['id'] != featured['id']).toList();
                      final sectionTitle = _selectedLevels.length == 1
                          ? l10n.t('workout.section.letsGo', {'level': _levelDisplayLabel(_selectedLevels.first, l10n)})
                          : l10n.t('workout.section.generic');

                      return ListView(
                        children: [
                          if (featured != null) ...[
                            _FeaturedBanner(
                              routine: featured,
                              favorited: _isFavorited(featured),
                              onFavoriteTap: () => _addFavorite(featured!['id'] as String),
                              onTap: () => _openRoutine(featured!['id'] as String),
                            ),
                            const SizedBox(height: 24),
                          ],
                          Text(
                            sectionTitle,
                            style: GoogleFonts.poppins(color: AppColors.brandLime, fontSize: 18, fontWeight: FontWeight.bold),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            l10n.t('workout.section.subtitle'),
                            style: TextStyle(color: Colors.grey.shade400, fontSize: 13),
                          ),
                          const SizedBox(height: 16),
                          if (rest.isEmpty && featured == null)
                            Padding(
                              padding: const EdgeInsets.symmetric(vertical: 40),
                              child: Center(child: Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400))),
                            ),
                          for (final routine in rest) ...[
                            _RoutineCard(
                              routine: routine,
                              favorited: _isFavorited(routine),
                              onFavoriteTap: () => _addFavorite(routine['id'] as String),
                              onTap: () => _openRoutine(routine['id'] as String),
                            ),
                            const SizedBox(height: 14),
                          ],
                        ],
                      );
                    },
                  ),
                ),
              ),
            ],
          ),
        ),
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

class _LevelPill extends StatelessWidget {
  const _LevelPill({required this.label, required this.selected, required this.onTap});

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 10),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: selected ? AppColors.brandLime : Colors.white,
          borderRadius: BorderRadius.circular(999),
        ),
        child: Text(
          label,
          textAlign: TextAlign.center,
          style: TextStyle(
            color: selected ? AppColors.brandDark : AppColors.brandPurple,
            fontWeight: FontWeight.w600,
            fontSize: 13,
          ),
        ),
      ),
    );
  }
}

class _ImagePlaceholder extends StatelessWidget {
  const _ImagePlaceholder();

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.brandDark2,
      alignment: Alignment.center,
      child: Icon(Icons.fitness_center, color: Colors.grey.shade600, size: 32),
    );
  }
}

class _FavoriteStarBadge extends StatelessWidget {
  const _FavoriteStarBadge({required this.favorited, required this.onTap});

  final bool favorited;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 28,
        height: 28,
        decoration: const BoxDecoration(color: Colors.white, shape: BoxShape.circle),
        child: Icon(
          favorited ? Icons.star : Icons.star_border,
          size: 16,
          color: favorited ? AppColors.brandLime : AppColors.brandDark2,
        ),
      ),
    );
  }
}

class _FeaturedBadgePill extends StatelessWidget {
  const _FeaturedBadgePill({required this.text});

  final String text;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(color: AppColors.brandLime, borderRadius: BorderRadius.circular(999)),
      child: Text(text, style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 11)),
    );
  }
}

class _RoutineStatsRow extends StatelessWidget {
  const _RoutineStatsRow({
    required this.durationMinutes,
    required this.caloriesEstimate,
    required this.exerciseCount,
    required this.color,
    this.iconSize = 13,
    this.fontSize = 12,
  });

  final dynamic durationMinutes;
  final dynamic caloriesEstimate;
  final dynamic exerciseCount;
  final Color color;
  final double iconSize;
  final double fontSize;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Wrap(
      spacing: 12,
      runSpacing: 4,
      children: [
        if (durationMinutes != null)
          _StatItem(
            icon: Icons.access_time,
            text: l10n.t('home.stats.minutes', {'count': '$durationMinutes'}),
            color: color,
            iconSize: iconSize,
            fontSize: fontSize,
          ),
        if (caloriesEstimate != null)
          _StatItem(
            icon: Icons.local_fire_department,
            text: l10n.t('home.stats.kcal', {'count': '$caloriesEstimate'}),
            color: color,
            iconSize: iconSize,
            fontSize: fontSize,
          ),
        if (exerciseCount != null)
          _StatItem(
            icon: Icons.fitness_center,
            text: l10n.t('workout.stats.exercises', {'count': '$exerciseCount'}),
            color: color,
            iconSize: iconSize,
            fontSize: fontSize,
          ),
      ],
    );
  }
}

class _StatItem extends StatelessWidget {
  const _StatItem({required this.icon, required this.text, required this.color, required this.iconSize, required this.fontSize});

  final IconData icon;
  final String text;
  final Color color;
  final double iconSize;
  final double fontSize;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: iconSize, color: color),
        const SizedBox(width: 4),
        Text(text, style: TextStyle(color: color, fontSize: fontSize)),
      ],
    );
  }
}

class _FeaturedBanner extends StatelessWidget {
  const _FeaturedBanner({required this.routine, required this.favorited, required this.onFavoriteTap, required this.onTap});

  final Map<String, dynamic> routine;
  final bool favorited;
  final VoidCallback onFavoriteTap;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final name = routine['name'] as String? ?? '';
    final imageUrl = routine['imageUrl'] as String?;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(color: AppColors.brandLavender, borderRadius: BorderRadius.circular(24)),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(18),
              child: SizedBox(
                height: 160,
                width: double.infinity,
                child: Stack(
                  fit: StackFit.expand,
                  children: [
                    imageUrl == null
                        ? const _ImagePlaceholder()
                        : Image.network(
                            imageUrl,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) => const _ImagePlaceholder(),
                          ),
                    Positioned(top: 10, right: 10, child: _FeaturedBadgePill(text: l10n.t('workout.featured.badge'))),
                    Positioned(top: 10, left: 10, child: _FavoriteStarBadge(favorited: favorited, onTap: onFavoriteTap)),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 14),
            Text(
              name,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: GoogleFonts.poppins(color: AppColors.brandLime, fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            _RoutineStatsRow(
              durationMinutes: routine['durationMinutes'],
              caloriesEstimate: routine['caloriesEstimate'],
              exerciseCount: routine['exerciseCount'],
              color: AppColors.brandDark,
              iconSize: 14,
              fontSize: 12,
            ),
          ],
        ),
      ),
    );
  }
}

class _RoutineCard extends StatelessWidget {
  const _RoutineCard({required this.routine, required this.favorited, required this.onFavoriteTap, required this.onTap});

  final Map<String, dynamic> routine;
  final bool favorited;
  final VoidCallback onFavoriteTap;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final name = routine['name'] as String? ?? '';
    final imageUrl = routine['imageUrl'] as String?;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(20)),
        padding: const EdgeInsets.all(14),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    name,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 15),
                  ),
                  const SizedBox(height: 8),
                  _RoutineStatsRow(
                    durationMinutes: routine['durationMinutes'],
                    caloriesEstimate: routine['caloriesEstimate'],
                    exerciseCount: routine['exerciseCount'],
                    color: Colors.grey.shade600,
                  ),
                ],
              ),
            ),
            const SizedBox(width: 12),
            SizedBox(
              width: 84,
              height: 72,
              child: Stack(
                children: [
                  ClipRRect(
                    borderRadius: BorderRadius.circular(14),
                    child: SizedBox(
                      width: 84,
                      height: 72,
                      child: imageUrl == null
                          ? const _ImagePlaceholder()
                          : Image.network(
                              imageUrl,
                              fit: BoxFit.cover,
                              errorBuilder: (context, error, stackTrace) => const _ImagePlaceholder(),
                            ),
                    ),
                  ),
                  Positioned(top: 4, right: 4, child: _FavoriteStarBadge(favorited: favorited, onTap: onFavoriteTap)),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

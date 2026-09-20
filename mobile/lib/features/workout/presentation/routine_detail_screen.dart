import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../notification/presentation/notification_screen.dart';
import '../../profile/presentation/profile_screen.dart';
import '../../search/presentation/search_screen.dart';
import 'exercise_step_screen.dart';
import 'workout_providers.dart';

class RoutineDetailScreen extends ConsumerStatefulWidget {
  const RoutineDetailScreen({super.key, required this.routineId});

  final String routineId;

  @override
  ConsumerState<RoutineDetailScreen> createState() => _RoutineDetailScreenState();
}

class _RoutineDetailScreenState extends ConsumerState<RoutineDetailScreen> {
  bool? _favoritedOverride;

  Future<void> _toggleFavorite(String routineId) async {
    if (_favoritedOverride == true) return;
    setState(() => _favoritedOverride = true);
    try {
      await ref.read(workoutApiProvider).addFavorite({'favoritableType': 'Routine', 'favoritableId': routineId});
    } on DioException catch (e) {
      if (e.response?.statusCode != 409) {
        setState(() => _favoritedOverride = false);
      }
    }
  }

  List<List<Map<String, dynamic>>> _groupByRound(List<dynamic> exercises) {
    final groups = <List<Map<String, dynamic>>>[];
    List<Map<String, dynamic>>? current;
    dynamic currentRound;
    for (final raw in exercises) {
      final e = raw as Map<String, dynamic>;
      final round = e['roundNumber'];
      if (current == null || round != currentRound) {
        current = [];
        groups.add(current);
        currentRound = round;
      }
      current.add(e);
    }
    return groups;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final detailAsync = ref.watch(routineDetailProvider(widget.routineId));

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
                      detailAsync.value?['level'] as String? ?? '',
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
              Expanded(
                child: detailAsync.when(
                  loading: () => const Center(child: CircularProgressIndicator()),
                  error: (err, _) => Center(
                    child: Text(
                      l10n.t('common.error.loadFailed', {'error': err.toString()}),
                      style: const TextStyle(color: Colors.white),
                      textAlign: TextAlign.center,
                    ),
                  ),
                  data: (routine) {
                    final exercises = (routine['exercises'] as List<dynamic>? ?? []);
                    final rounds = _groupByRound(exercises);
                    final favorited = _favoritedOverride ?? (routine['isFavorited'] == true);

                    return ListView(
                      children: [
                        _RoutineHero(
                          routine: routine,
                          favorited: favorited,
                          onFavoriteTap: () => _toggleFavorite(routine['id'] as String),
                        ),
                        const SizedBox(height: 24),
                        for (final group in rounds) ...[
                          _RoundSection(
                            roundNumber: group.first['roundNumber'],
                            exercises: group,
                            onExerciseTap: (exercise) => Navigator.of(context)
                                .push(MaterialPageRoute(builder: (_) => ExerciseStepScreen(exercise: exercise))),
                          ),
                          const SizedBox(height: 20),
                        ],
                      ],
                    );
                  },
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

class _ImagePlaceholder extends StatelessWidget {
  const _ImagePlaceholder();

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.brandDark2,
      alignment: Alignment.center,
      child: Icon(Icons.fitness_center, color: Colors.grey.shade600, size: 40),
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
        width: 30,
        height: 30,
        decoration: const BoxDecoration(color: Colors.white, shape: BoxShape.circle),
        child: Icon(
          favorited ? Icons.star : Icons.star_border,
          size: 17,
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

class _RoutineHero extends StatelessWidget {
  const _RoutineHero({required this.routine, required this.favorited, required this.onFavoriteTap});

  final Map<String, dynamic> routine;
  final bool favorited;
  final VoidCallback onFavoriteTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final imageUrl = routine['imageUrl'] as String?;
    final isFeatured = routine['isFeatured'] == true;
    final durationMinutes = routine['durationMinutes'];
    final caloriesEstimate = routine['caloriesEstimate'];
    final level = routine['level'] as String? ?? '';

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(color: AppColors.brandLavender, borderRadius: BorderRadius.circular(24)),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(18),
            child: SizedBox(
              height: 200,
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
                  if (isFeatured)
                    Positioned(top: 10, right: 10, child: _FeaturedBadgePill(text: l10n.t('workout.featured.badge'))),
                  Positioned(top: 10, left: 10, child: _FavoriteStarBadge(favorited: favorited, onTap: onFavoriteTap)),
                ],
              ),
            ),
          ),
          const SizedBox(height: 14),
          Wrap(
            spacing: 16,
            runSpacing: 6,
            children: [
              if (durationMinutes != null)
                _StatItem(icon: Icons.access_time, text: l10n.t('home.stats.minutes', {'count': '$durationMinutes'})),
              if (caloriesEstimate != null)
                _StatItem(icon: Icons.local_fire_department, text: l10n.t('home.stats.kcal', {'count': '$caloriesEstimate'})),
              _StatItem(icon: Icons.fitness_center, text: level),
            ],
          ),
        ],
      ),
    );
  }
}

class _StatItem extends StatelessWidget {
  const _StatItem({required this.icon, required this.text});

  final IconData icon;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppColors.brandDark),
        const SizedBox(width: 4),
        Text(text, style: const TextStyle(color: AppColors.brandDark, fontSize: 12, fontWeight: FontWeight.w600)),
      ],
    );
  }
}

class _RoundSection extends StatelessWidget {
  const _RoundSection({required this.roundNumber, required this.exercises, required this.onExerciseTap});

  final dynamic roundNumber;
  final List<Map<String, dynamic>> exercises;
  final void Function(Map<String, dynamic> exercise) onExerciseTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.t('workout.round', {'number': '$roundNumber'}),
          style: GoogleFonts.poppins(color: AppColors.brandLime, fontSize: 16, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 10),
        for (var i = 0; i < exercises.length; i++) ...[
          _ExerciseRow(
            exercise: exercises[i],
            circleColor: i.isEven ? AppColors.brandPurple : AppColors.brandLime,
            onTap: () => onExerciseTap(exercises[i]),
          ),
          if (i != exercises.length - 1) const SizedBox(height: 10),
        ],
      ],
    );
  }
}

class _ExerciseRow extends StatelessWidget {
  const _ExerciseRow({required this.exercise, required this.circleColor, required this.onTap});

  final Map<String, dynamic> exercise;
  final Color circleColor;
  final VoidCallback onTap;

  String _formatDuration(int seconds) {
    final minutes = seconds ~/ 60;
    final secs = seconds % 60;
    return '${minutes.toString().padLeft(2, '0')}:${secs.toString().padLeft(2, '0')}';
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final name = exercise['exerciseName'] as String? ?? '';
    final durationSeconds = exercise['durationSeconds'] as int?;
    final reps = exercise['reps'];
    final playIconColor = circleColor == AppColors.brandLime ? AppColors.brandDark : Colors.white;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(999)),
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
        child: Row(
          children: [
            Container(
              width: 44,
              height: 44,
              decoration: BoxDecoration(color: circleColor, shape: BoxShape.circle),
              child: Icon(Icons.play_arrow, color: playIconColor, size: 22),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    name,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 14),
                  ),
                  if (durationSeconds != null) ...[
                    const SizedBox(height: 2),
                    Text(_formatDuration(durationSeconds), style: TextStyle(color: Colors.grey.shade600, fontSize: 11)),
                  ],
                ],
              ),
            ),
            const SizedBox(width: 8),
            Text(
              l10n.t('workout.repetition', {'reps': '$reps'}),
              style: const TextStyle(color: AppColors.brandPurple, fontWeight: FontWeight.bold, fontSize: 13),
            ),
          ],
        ),
      ),
    );
  }
}

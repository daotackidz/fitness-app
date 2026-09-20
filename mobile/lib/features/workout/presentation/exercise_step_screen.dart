import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import 'workout_providers.dart';

class ExerciseStepScreen extends ConsumerStatefulWidget {
  const ExerciseStepScreen({super.key, required this.exercise});

  final Map<String, dynamic> exercise;

  @override
  ConsumerState<ExerciseStepScreen> createState() => _ExerciseStepScreenState();
}

class _ExerciseStepScreenState extends ConsumerState<ExerciseStepScreen> {
  bool _favorited = false;

  Future<void> _toggleFavorite() async {
    if (_favorited) return;
    setState(() => _favorited = true);
    final exerciseId = widget.exercise['exerciseId'] as String?;
    if (exerciseId == null) return;
    try {
      await ref.read(workoutApiProvider).addFavorite({'favoritableType': 'Exercise', 'favoritableId': exerciseId});
    } on DioException catch (e) {
      if (e.response?.statusCode != 409) {
        setState(() => _favorited = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final exercise = widget.exercise;
    final name = exercise['exerciseName'] as String? ?? '';
    final description = exercise['description'] as String?;
    final imageUrl = exercise['imageUrl'] as String?;
    final durationSeconds = exercise['durationSeconds'];
    final reps = exercise['reps'];
    final difficultyLevel = exercise['difficultyLevel'] as String? ?? '';

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
                      difficultyLevel,
                      style: GoogleFonts.poppins(color: Colors.white, fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 18),
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Container(
                        height: 320,
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(color: AppColors.brandLavender, borderRadius: BorderRadius.circular(24)),
                        child: Stack(
                          fit: StackFit.expand,
                          children: [
                            ClipRRect(
                              borderRadius: BorderRadius.circular(18),
                              child: imageUrl == null
                                  ? const _ImagePlaceholder()
                                  : Image.network(
                                      imageUrl,
                                      fit: BoxFit.cover,
                                      errorBuilder: (context, error, stackTrace) => const _ImagePlaceholder(),
                                    ),
                            ),
                            Center(
                              child: Container(
                                width: 76,
                                height: 76,
                                decoration: const BoxDecoration(color: AppColors.brandPurple, shape: BoxShape.circle),
                                child: const Icon(Icons.play_arrow, color: Colors.white, size: 42),
                              ),
                            ),
                            Positioned(
                              top: 8,
                              right: 8,
                              child: _FavoriteStarBadge(favorited: _favorited, onTap: _toggleFavorite),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 20),
                      Container(
                        padding: const EdgeInsets.all(20),
                        decoration: BoxDecoration(color: AppColors.brandLime, borderRadius: BorderRadius.circular(24)),
                        child: Column(
                          children: [
                            Text(
                              name,
                              textAlign: TextAlign.center,
                              style: GoogleFonts.poppins(color: AppColors.brandDark, fontSize: 20, fontWeight: FontWeight.bold),
                            ),
                            if (description != null) ...[
                              const SizedBox(height: 10),
                              Text(
                                description,
                                textAlign: TextAlign.center,
                                style: const TextStyle(color: AppColors.brandDark, fontSize: 13),
                              ),
                            ],
                            const SizedBox(height: 18),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                              children: [
                                if (durationSeconds != null)
                                  _StatColumn(icon: Icons.access_time, text: l10n.t('workout.exercise.seconds', {'count': '$durationSeconds'})),
                                if (reps != null)
                                  _StatColumn(icon: Icons.fitness_center, text: l10n.t('workout.exercise.rep', {'count': '$reps'})),
                                _StatColumn(icon: Icons.person_outline, text: difficultyLevel),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
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

class _ImagePlaceholder extends StatelessWidget {
  const _ImagePlaceholder();

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.brandDark2,
      alignment: Alignment.center,
      child: Icon(Icons.fitness_center, color: Colors.grey.shade600, size: 48),
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

class _StatColumn extends StatelessWidget {
  const _StatColumn({required this.icon, required this.text});

  final IconData icon;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, color: AppColors.brandDark, size: 18),
        const SizedBox(height: 4),
        Text(text, style: const TextStyle(color: AppColors.brandDark, fontSize: 11, fontWeight: FontWeight.w600)),
      ],
    );
  }
}

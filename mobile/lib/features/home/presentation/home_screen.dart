import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../community/presentation/community_screen.dart';
import '../../notification/presentation/notification_screen.dart';
import '../../nutrition/presentation/nutrition_screen.dart';
import '../../profile/presentation/profile_screen.dart';
import '../../search/presentation/search_screen.dart';
import '../../workout/presentation/workout_screen.dart';
import 'home_providers.dart';

class HomeScreen extends ConsumerStatefulWidget {
  const HomeScreen({super.key});

  @override
  ConsumerState<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends ConsumerState<HomeScreen> {
  // Cac id da duoc gan sao (favorite) trong phien lam viec nay, de cap nhat UI ngay
  // ma khong can cho response cua provider tai lai toan bo du lieu.
  final Set<String> _optimisticallyFavorited = {};

  Future<void> _addFavorite({required String type, required String id}) async {
    if (_optimisticallyFavorited.contains(id)) return;
    setState(() => _optimisticallyFavorited.add(id));
    try {
      await ref.read(homeApiProvider).addFavorite({'favoritableType': type, 'favoritableId': id});
    } on DioException catch (e) {
      // 409 nghia la da favorite tu truoc - coi nhu thanh cong, khong bao loi.
      if (e.response?.statusCode != 409) {
        setState(() => _optimisticallyFavorited.remove(id));
      }
    }
  }

  bool _isFavorited(Map<String, dynamic> item) {
    return item['isFavorited'] == true || _optimisticallyFavorited.contains(item['id'] as String);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final homeAsync = ref.watch(homeProvider);

    return Scaffold(
      backgroundColor: AppColors.brandDark,
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () async => ref.invalidate(homeProvider),
          child: homeAsync.when(
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
            data: (data) => _HomeContent(
              data: data,
              isFavorited: _isFavorited,
              onFavorite: _addFavorite,
            ),
          ),
        ),
      ),
    );
  }
}

class _HomeContent extends StatelessWidget {
  const _HomeContent({required this.data, required this.isFavorited, required this.onFavorite});

  final Map<String, dynamic> data;
  final bool Function(Map<String, dynamic> item) isFavorited;
  final Future<void> Function({required String type, required String id}) onFavorite;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final fullName = (data['fullName'] as String?)?.trim() ?? '';
    final firstName = fullName.isEmpty ? '' : fullName.split(RegExp(r'\s+')).first;
    final recommendations = (data['recommendations'] as List<dynamic>? ?? []).cast<Map<String, dynamic>>();
    final articles = (data['articles'] as List<dynamic>? ?? []).cast<Map<String, dynamic>>();
    final featuredChallenge = data['featuredChallenge'] as Map<String, dynamic>?;

    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
      children: [
        _HeaderRow(firstName: firstName),
        const SizedBox(height: 24),
        const _QuickNavRow(),
        const SizedBox(height: 28),
        _SectionHeader(title: l10n.t('home.recommendations.title'), seeAll: l10n.t('home.recommendations.seeAll')),
        const SizedBox(height: 12),
        _RecommendationsList(items: recommendations, isFavorited: isFavorited, onFavorite: onFavorite),
        if (featuredChallenge != null) ...[
          const SizedBox(height: 28),
          _WeeklyChallengeBanner(challenge: featuredChallenge),
        ],
        const SizedBox(height: 28),
        _SectionHeader(title: l10n.t('home.articles.title')),
        const SizedBox(height: 12),
        _ArticlesList(items: articles, isFavorited: isFavorited, onFavorite: onFavorite),
        const SizedBox(height: 16),
      ],
    );
  }
}

class _HeaderRow extends StatelessWidget {
  const _HeaderRow({required this.firstName});

  final String firstName;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Hi, $firstName',
                style: GoogleFonts.poppins(
                  color: AppColors.brandLavender,
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 4),
              Text(
                l10n.t('home.greeting.subtitle'),
                style: TextStyle(color: Colors.grey.shade400, fontSize: 13),
              ),
            ],
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
        decoration: BoxDecoration(
          color: AppColors.brandDark2,
          borderRadius: BorderRadius.circular(10),
        ),
        child: Icon(icon, color: AppColors.brandLavender, size: 18),
      ),
    );
  }
}

class _QuickNavRow extends StatelessWidget {
  const _QuickNavRow();

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final items = [
      (Icons.fitness_center, l10n.t('home.quickNav.workout'), () => const WorkoutScreen()),
      (Icons.assessment_outlined, l10n.t('home.quickNav.progressTracking'), null),
      (Icons.apple, l10n.t('home.quickNav.nutrition'), () => const NutritionScreen()),
      (Icons.people_outline, l10n.t('home.quickNav.community'), () => const CommunityScreen()),
    ];

    return Row(
      children: [
        for (var i = 0; i < items.length; i++) ...[
          Expanded(
            child: _QuickNavItem(
              icon: items[i].$1,
              label: items[i].$2,
              onTap: items[i].$3 == null
                  ? null
                  : () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => items[i].$3!())),
            ),
          ),
          if (i == 2)
            Container(
              width: 1,
              height: 48,
              color: Colors.white.withValues(alpha: 0.12),
            ),
        ],
      ],
    );
  }
}

class _QuickNavItem extends StatelessWidget {
  const _QuickNavItem({required this.icon, required this.label, this.onTap});

  final IconData icon;
  final String label;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Column(
        children: [
          Icon(icon, color: AppColors.brandLime, size: 28),
          const SizedBox(height: 6),
          Text(
            label,
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: AppColors.brandLavender, fontSize: 11),
          ),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  const _SectionHeader({required this.title, this.seeAll});

  final String title;
  final String? seeAll;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          title,
          style: GoogleFonts.poppins(color: AppColors.brandLime, fontSize: 18, fontWeight: FontWeight.bold),
        ),
        if (seeAll != null)
          Text(seeAll!, style: TextStyle(color: Colors.grey.shade300, fontSize: 12)),
      ],
    );
  }
}

class _RecommendationsList extends StatelessWidget {
  const _RecommendationsList({required this.items, required this.isFavorited, required this.onFavorite});

  final List<Map<String, dynamic>> items;
  final bool Function(Map<String, dynamic> item) isFavorited;
  final Future<void> Function({required String type, required String id}) onFavorite;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    if (items.isEmpty) {
      return Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400));
    }

    return SizedBox(
      height: 200,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: items.length,
        separatorBuilder: (_, _) => const SizedBox(width: 14),
        itemBuilder: (context, index) {
          final item = items[index];
          final id = item['id'] as String;
          return _RecommendationCard(
            item: item,
            favorited: isFavorited(item),
            onFavoriteTap: () => onFavorite(type: 'Exercise', id: id),
          );
        },
      ),
    );
  }
}

class _RecommendationCard extends StatelessWidget {
  const _RecommendationCard({required this.item, required this.favorited, required this.onFavoriteTap});

  final Map<String, dynamic> item;
  final bool favorited;
  final VoidCallback onFavoriteTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final name = item['name'] as String? ?? '';
    final imageUrl = item['imageUrl'] as String?;
    final durationMinutes = item['durationMinutes'];
    final caloriesEstimate = item['caloriesEstimate'];

    return SizedBox(
      width: 170,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            height: 110,
            width: 170,
            child: Stack(
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: SizedBox(
                    height: 110,
                    width: 170,
                    child: imageUrl == null
                        ? const _ImagePlaceholder(icon: Icons.fitness_center)
                        : Image.network(
                            imageUrl,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) =>
                                const _ImagePlaceholder(icon: Icons.fitness_center),
                          ),
                  ),
                ),
                Positioned(top: 8, right: 8, child: _FavoriteBadge(favorited: favorited, onTap: onFavoriteTap)),
                const Positioned(bottom: 8, right: 8, child: _PlayBadge()),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Text(
            name,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 14),
          ),
          const SizedBox(height: 4),
          Row(
            children: [
              if (durationMinutes != null) ...[
                Icon(Icons.access_time, size: 12, color: Colors.grey.shade400),
                const SizedBox(width: 4),
                Text(
                  l10n.t('home.stats.minutes', {'count': '$durationMinutes'}),
                  style: TextStyle(color: Colors.grey.shade400, fontSize: 11),
                ),
              ],
              if (durationMinutes != null && caloriesEstimate != null) const SizedBox(width: 10),
              if (caloriesEstimate != null) ...[
                Icon(Icons.local_fire_department, size: 12, color: Colors.grey.shade400),
                const SizedBox(width: 4),
                Text(
                  l10n.t('home.stats.kcal', {'count': '$caloriesEstimate'}),
                  style: TextStyle(color: Colors.grey.shade400, fontSize: 11),
                ),
              ],
            ],
          ),
        ],
      ),
    );
  }
}

class _ImagePlaceholder extends StatelessWidget {
  const _ImagePlaceholder({required this.icon});

  final IconData icon;

  @override
  Widget build(BuildContext context) {
    return Container(
      color: AppColors.brandDark2,
      alignment: Alignment.center,
      child: Icon(icon, color: Colors.grey.shade600, size: 32),
    );
  }
}

class _FavoriteBadge extends StatelessWidget {
  const _FavoriteBadge({required this.favorited, required this.onTap});

  final bool favorited;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 26,
        height: 26,
        decoration: const BoxDecoration(color: Colors.white, shape: BoxShape.circle),
        child: Icon(
          favorited ? Icons.star : Icons.star_border,
          size: 15,
          color: favorited ? AppColors.brandLime : AppColors.brandDark2,
        ),
      ),
    );
  }
}

class _PlayBadge extends StatelessWidget {
  const _PlayBadge();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 26,
      height: 26,
      decoration: const BoxDecoration(color: AppColors.brandLime, shape: BoxShape.circle),
      child: const Icon(Icons.play_arrow, size: 16, color: AppColors.brandDark2),
    );
  }
}

class _WeeklyChallengeBanner extends StatelessWidget {
  const _WeeklyChallengeBanner({required this.challenge});

  final Map<String, dynamic> challenge;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final name = challenge['name'] as String? ?? '';
    final imageUrl = challenge['imageUrl'] as String?;

    return Container(
      constraints: const BoxConstraints(minHeight: 140),
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: AppColors.brandLavender,
        borderRadius: BorderRadius.circular(24),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Expanded(
            flex: 55,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  l10n.t('home.weeklyChallenge.label'),
                  style: GoogleFonts.poppins(
                    color: AppColors.brandDark,
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    height: 1.15,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  name,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(color: AppColors.brandDark, fontSize: 13, fontWeight: FontWeight.normal),
                ),
              ],
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            flex: 45,
            child: ClipRRect(
              borderRadius: BorderRadius.circular(16),
              child: SizedBox(
                height: 100,
                child: imageUrl == null
                    ? const _ImagePlaceholder(icon: Icons.emoji_events)
                    : Image.network(
                        imageUrl,
                        fit: BoxFit.cover,
                        width: double.infinity,
                        errorBuilder: (context, error, stackTrace) =>
                            const _ImagePlaceholder(icon: Icons.emoji_events),
                      ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ArticlesList extends StatelessWidget {
  const _ArticlesList({required this.items, required this.isFavorited, required this.onFavorite});

  final List<Map<String, dynamic>> items;
  final bool Function(Map<String, dynamic> item) isFavorited;
  final Future<void> Function({required String type, required String id}) onFavorite;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;

    if (items.isEmpty) {
      return Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400));
    }

    return SizedBox(
      height: 170,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: items.length,
        separatorBuilder: (_, _) => const SizedBox(width: 14),
        itemBuilder: (context, index) {
          final item = items[index];
          final id = item['id'] as String;
          return _ArticleCard(
            item: item,
            favorited: isFavorited(item),
            onFavoriteTap: () => onFavorite(type: 'Article', id: id),
          );
        },
      ),
    );
  }
}

class _ArticleCard extends StatelessWidget {
  const _ArticleCard({required this.item, required this.favorited, required this.onFavoriteTap});

  final Map<String, dynamic> item;
  final bool favorited;
  final VoidCallback onFavoriteTap;

  @override
  Widget build(BuildContext context) {
    final title = item['title'] as String? ?? '';
    final coverImage = item['coverImage'] as String?;

    return SizedBox(
      width: 170,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            height: 110,
            width: 170,
            child: Stack(
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(16),
                  child: SizedBox(
                    height: 110,
                    width: 170,
                    child: coverImage == null
                        ? const _ImagePlaceholder(icon: Icons.article)
                        : Image.network(
                            coverImage,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) =>
                                const _ImagePlaceholder(icon: Icons.article),
                          ),
                  ),
                ),
                Positioned(top: 8, right: 8, child: _FavoriteBadge(favorited: favorited, onTap: onFavoriteTap)),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Text(
            title,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 14),
          ),
        ],
      ),
    );
  }
}

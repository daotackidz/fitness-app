import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/theme/app_colors.dart';
import '../../notification/presentation/notification_screen.dart';
import '../../search/presentation/search_screen.dart';
import 'profile_providers.dart';
import 'profile_screen.dart';

class FavoritesScreen extends ConsumerStatefulWidget {
  const FavoritesScreen({super.key, this.showBackButton = true});

  final bool showBackButton;

  @override
  ConsumerState<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends ConsumerState<FavoritesScreen> {
  String? _type;
  List<Map<String, dynamic>>? _items;

  Future<void> _removeFavorite(String favoriteId) async {
    final previous = _items;
    setState(() => _items = _items?.where((e) => e['id'] != favoriteId).toList());
    try {
      await ref.read(profileApiProvider).deleteFavorite(favoriteId);
    } catch (_) {
      if (mounted) setState(() => _items = previous);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final favoritesAsync = ref.watch(favoritesProvider(_type));

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
                  if (widget.showBackButton)
                    IconButton(
                      padding: EdgeInsets.zero,
                      constraints: const BoxConstraints(),
                      onPressed: () => Navigator.of(context).maybePop(),
                      icon: const Icon(Icons.arrow_back_ios_new, color: AppColors.brandLime, size: 18),
                    ),
                  if (widget.showBackButton) const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      l10n.t('profile.favorites.title'),
                      style: const TextStyle(color: Colors.white, fontSize: 20, fontWeight: FontWeight.bold),
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
              const SizedBox(height: 16),
              Row(
                children: [
                  _SortPill(
                    label: l10n.t('profile.favorites.all'),
                    selected: _type == null,
                    onTap: () => setState(() {
                      _type = null;
                      _items = null;
                    }),
                  ),
                  const SizedBox(width: 10),
                  _SortPill(
                    label: l10n.t('profile.favorites.video'),
                    selected: _type == 'Video',
                    onTap: () => setState(() {
                      _type = 'Video';
                      _items = null;
                    }),
                  ),
                  const SizedBox(width: 10),
                  _SortPill(
                    label: l10n.t('profile.favorites.article'),
                    selected: _type == 'Article',
                    onTap: () => setState(() {
                      _type = 'Article';
                      _items = null;
                    }),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Expanded(
                child: favoritesAsync.when(
                  loading: () => const Center(child: CircularProgressIndicator()),
                  error: (err, _) => Center(
                    child: Text(
                      l10n.t('common.error.loadFailed', {'error': err.toString()}),
                      style: const TextStyle(color: Colors.white),
                    ),
                  ),
                  data: (data) {
                    _items ??= data;
                    if (_items!.isEmpty) {
                      return Center(child: Text(l10n.t('common.noData'), style: TextStyle(color: Colors.grey.shade400)));
                    }
                    return ListView.separated(
                      itemCount: _items!.length,
                      separatorBuilder: (_, _) => const SizedBox(height: 14),
                      itemBuilder: (context, index) => _FavoriteCard(
                        item: _items![index],
                        onRemove: () => _removeFavorite(_items![index]['id'] as String),
                      ),
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

class _SortPill extends StatelessWidget {
  const _SortPill({required this.label, required this.selected, required this.onTap});

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

class _FavoriteCard extends StatelessWidget {
  const _FavoriteCard({required this.item, required this.onRemove});

  final Map<String, dynamic> item;
  final VoidCallback onRemove;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final title = item['title'] as String? ?? '';
    final imageUrl = item['imageUrl'] as String?;
    final description = item['description'] as String?;
    final durationMinutes = item['durationMinutes'];
    final caloriesEstimate = item['caloriesEstimate'];
    final exerciseCount = item['exerciseCount'];
    final favoritableType = item['favoritableType'] as String?;
    final hasStats = durationMinutes != null;

    return Container(
      decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(20)),
      padding: const EdgeInsets.all(14),
      child: Stack(
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(color: AppColors.brandDark, fontWeight: FontWeight.bold, fontSize: 15),
                    ),
                    const SizedBox(height: 8),
                    if (hasStats)
                      Wrap(
                        spacing: 12,
                        runSpacing: 4,
                        children: [
                          if (durationMinutes != null)
                            _StatChip(icon: Icons.access_time, text: l10n.t('home.stats.minutes', {'count': '$durationMinutes'})),
                          if (caloriesEstimate != null)
                            _StatChip(icon: Icons.local_fire_department, text: l10n.t('home.stats.kcal', {'count': '$caloriesEstimate'})),
                          if (exerciseCount != null)
                            _StatChip(icon: Icons.fitness_center, text: l10n.t('profile.favorites.exercises', {'count': '$exerciseCount'})),
                        ],
                      )
                    else if (description != null)
                      Text(
                        description,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(color: Colors.grey.shade600, fontSize: 12),
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
                            ? _ThumbPlaceholder(favoritableType: favoritableType)
                            : Image.network(
                                imageUrl,
                                fit: BoxFit.cover,
                                errorBuilder: (context, error, stackTrace) => _ThumbPlaceholder(favoritableType: favoritableType),
                              ),
                      ),
                    ),
                    if (durationMinutes != null && exerciseCount != null)
                      const Positioned(bottom: 4, right: 4, child: _MiniPlayBadge()),
                  ],
                ),
              ),
            ],
          ),
          Positioned(top: -2, right: -2, child: _StarBadge(onTap: onRemove)),
        ],
      ),
    );
  }
}

class _ThumbPlaceholder extends StatelessWidget {
  const _ThumbPlaceholder({this.favoritableType});

  final String? favoritableType;

  @override
  Widget build(BuildContext context) {
    final icon = favoritableType == 'Article' ? Icons.article : Icons.fitness_center;
    return Container(color: AppColors.brandLavenderSoft, alignment: Alignment.center, child: Icon(icon, color: AppColors.brandLavender, size: 28));
  }
}

class _StatChip extends StatelessWidget {
  const _StatChip({required this.icon, required this.text});

  final IconData icon;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 12, color: Colors.grey.shade600),
        const SizedBox(width: 4),
        Text(text, style: TextStyle(color: Colors.grey.shade600, fontSize: 11)),
      ],
    );
  }
}

class _MiniPlayBadge extends StatelessWidget {
  const _MiniPlayBadge();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 22,
      height: 22,
      decoration: const BoxDecoration(color: AppColors.brandLime, shape: BoxShape.circle),
      child: const Icon(Icons.play_arrow, size: 14, color: AppColors.brandDark2),
    );
  }
}

class _StarBadge extends StatelessWidget {
  const _StarBadge({required this.onTap});

  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 28,
        height: 28,
        decoration: const BoxDecoration(color: AppColors.brandLime, shape: BoxShape.circle),
        child: const Icon(Icons.star, size: 16, color: AppColors.brandDark2),
      ),
    );
  }
}

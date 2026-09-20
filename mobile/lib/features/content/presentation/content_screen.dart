import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/localization/app_localizations.dart';
import '../../../core/network/dio_client.dart';
import '../data/content_api.dart';

final contentApiProvider = Provider<ContentApi>((ref) => ContentApi(ref.watch(dioProvider)));

final articlesProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(contentApiProvider).getArticles({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

final videosProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(contentApiProvider).getVideos({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

class ContentScreen extends ConsumerWidget {
  const ContentScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;

    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: Text(l10n.t('content.title')),
          bottom: TabBar(tabs: [
            Tab(text: l10n.t('content.tabs.articles')),
            Tab(text: l10n.t('content.tabs.videos')),
          ]),
        ),
        body: TabBarView(children: [_ArticleList(), _VideoList()]),
      ),
    );
  }
}

class _ArticleList extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final articlesAsync = ref.watch(articlesProvider);
    final l10n = context.l10n;
    return articlesAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
      data: (items) => ListView.separated(
        itemCount: items.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final a = items[index];
          return ListTile(title: Text(a['title'] as String), subtitle: Text(a['category'] ?? ''));
        },
      ),
    );
  }
}

class _VideoList extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final videosAsync = ref.watch(videosProvider);
    final l10n = context.l10n;
    return videosAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text(l10n.t('common.error.loadFailed', {'error': err.toString()}))),
      data: (items) => ListView.separated(
        itemCount: items.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final v = items[index];
          return ListTile(
            leading: const Icon(Icons.play_circle_outline),
            title: Text(v['title'] as String),
            subtitle: Text(l10n.t('content.video.durationSeconds', {'seconds': '${v['durationSeconds'] ?? 0}'})),
          );
        },
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

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
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Bai viet & Video'),
          bottom: const TabBar(tabs: [Tab(text: 'Bai viet'), Tab(text: 'Video')]),
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
    return articlesAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
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
    return videosAsync.when(
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
      data: (items) => ListView.separated(
        itemCount: items.length,
        separatorBuilder: (_, _) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final v = items[index];
          return ListTile(
            leading: const Icon(Icons.play_circle_outline),
            title: Text(v['title'] as String),
            subtitle: Text('${v['durationSeconds'] ?? 0}s'),
          );
        },
      ),
    );
  }
}

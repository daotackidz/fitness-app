import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/dio_client.dart';
import '../data/community_api.dart';

final communityApiProvider = Provider<CommunityApi>((ref) => CommunityApi(ref.watch(dioProvider)));

final forumPostsProvider = FutureProvider.autoDispose<List<Map<String, dynamic>>>((ref) async {
  final response = await ref.watch(communityApiProvider).getPosts({'page': 1, 'limit': 50});
  return (response.data['data'] as List<dynamic>).cast<Map<String, dynamic>>();
});

class CommunityScreen extends ConsumerWidget {
  const CommunityScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final postsAsync = ref.watch(forumPostsProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Cong dong')),
      body: postsAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, _) => Center(child: Text('Loi tai du lieu: $err')),
        data: (posts) => ListView.separated(
          itemCount: posts.length,
          separatorBuilder: (_, _) => const Divider(height: 1),
          itemBuilder: (context, index) {
            final p = posts[index];
            return ListTile(
              title: Text(p['title'] as String),
              subtitle: Text(p['userFullName'] as String),
              trailing: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Icon(Icons.favorite_border, size: 18),
                  const SizedBox(width: 4),
                  Text('${p['likesCount']}'),
                ],
              ),
            );
          },
        ),
      ),
    );
  }
}

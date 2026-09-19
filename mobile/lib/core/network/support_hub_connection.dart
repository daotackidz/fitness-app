import 'package:signalr_netcore/signalr_client.dart';

import '../app_config.dart';

class SupportHubConnection {
  HubConnection? _connection;

  Future<HubConnection> connect(String ticketId, String accessToken, void Function(Map<String, dynamic>) onMessage) async {
    await _connection?.stop();

    final connection = HubConnectionBuilder()
        .withUrl(
          '${AppConfig.hubBaseUrl}/hubs/support/$ticketId',
          options: HttpConnectionOptions(accessTokenFactory: () async => accessToken),
        )
        .build();

    connection.on('ReceiveMessage', (args) {
      if (args != null && args.isNotEmpty) {
        onMessage(args[0] as Map<String, dynamic>);
      }
    });

    await connection.start();
    _connection = connection;
    return connection;
  }

  Future<void> dispose() async {
    await _connection?.stop();
    _connection = null;
  }
}

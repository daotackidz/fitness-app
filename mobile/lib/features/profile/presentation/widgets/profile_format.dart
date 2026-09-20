import 'package:intl/intl.dart';

String _ordinal(int day) {
  if (day >= 11 && day <= 13) return '${day}th';
  switch (day % 10) {
    case 1:
      return '${day}st';
    case 2:
      return '${day}nd';
    case 3:
      return '${day}rd';
    default:
      return '${day}th';
  }
}

String formatBirthday(String? isoDate) {
  if (isoDate == null || isoDate.isEmpty) return '--';
  final date = DateTime.tryParse(isoDate);
  if (date == null) return '--';
  final month = DateFormat('MMMM').format(date);
  return '$month ${_ordinal(date.day)}';
}

int? computeAge(String? isoDate) {
  if (isoDate == null || isoDate.isEmpty) return null;
  final date = DateTime.tryParse(isoDate);
  if (date == null) return null;
  final now = DateTime.now();
  var age = now.year - date.year;
  if (now.month < date.month || (now.month == date.month && now.day < date.day)) {
    age--;
  }
  return age;
}

String formatDdMmYyyy(DateTime date) => DateFormat('dd/MM/yyyy').format(date);

String formatIsoDate(DateTime date) => DateFormat('yyyy-MM-dd').format(date);

import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import '../../../../core/theme/app_colors.dart';

/// Header dung cho tung buoc cua ProfileSetupFlow. Giong het giao dien cua
/// AuthTopSection (dung o luong dang nhap/dang ky) nhung nut back goi mot
/// callback noi bo de chuyen ve buoc truoc do, thay vi Navigator.maybePop -
/// vi luong nay khong dung named routes cho tung buoc (khong co gi de pop).
class ProfileSetupHeader extends StatelessWidget {
  final String title;
  final String? heading;
  final String? description;

  /// Truyen null de an nut back (vd: buoc dau tien cua flow thuc su).
  final VoidCallback? onBack;

  const ProfileSetupHeader({super.key, required this.title, this.heading, this.description, this.onBack});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      color: AppColors.brandDark,
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 28),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              if (onBack != null)
                IconButton(
                  onPressed: onBack,
                  icon: const Icon(Icons.arrow_back_ios_new, color: AppColors.brandLime, size: 18),
                )
              else
                const SizedBox(width: 12),
              Expanded(
                child: Text(
                  title,
                  textAlign: TextAlign.center,
                  style: GoogleFonts.poppins(color: AppColors.brandLime, fontSize: 20, fontWeight: FontWeight.bold),
                ),
              ),
              SizedBox(width: onBack != null ? 48 : 12),
            ],
          ),
          if (heading != null) ...[
            const SizedBox(height: 24),
            Text(
              heading!,
              textAlign: TextAlign.center,
              style: GoogleFonts.poppins(color: Colors.white, fontSize: 26, fontWeight: FontWeight.w800),
            ),
          ],
          if (description != null) ...[
            const SizedBox(height: 12),
            Text(
              description!,
              textAlign: TextAlign.center,
              style: TextStyle(color: Colors.white.withValues(alpha: 0.7), fontSize: 14, height: 1.4),
            ),
          ],
        ],
      ),
    );
  }
}

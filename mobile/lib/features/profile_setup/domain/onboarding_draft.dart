// Du lieu thu thap qua 8 buoc cua luong profile setup (onboarding sau dang
// ky). Day la mot class Dart thuan tuy (khong @JsonSerializable) vi chi song
// tam thoi trong State cua ProfileSetupFlow va duoc build thu cong thanh
// body request khi submit o buoc cuoi.
class OnboardingDraft {
  /// 'Male' hoac 'Female' (theo dung gia tri enum backend mong doi).
  String? gender;

  /// Tuoi nguoi dung chon o buoc Age, dung de suy ra dateOfBirth khi submit.
  int age = 25;

  /// Luon luu theo kg (don vi API yeu cau), bat ke nguoi dung dang xem KG hay LB.
  double weightKg = 70;

  /// Chi anh huong hien thi o man Weight, khong gui len API.
  bool weightUnitIsLb = false;

  double heightCm = 165;

  /// 'LoseWeight' | 'GainWeight' | 'MuscleMassGain' | 'ShapeBody' | 'Others'
  String? fitnessGoal;

  /// 'Beginner' | 'Intermediate' | 'Advanced'
  String? activityLevel;

  bool get isGenderValid => gender != null;
  bool get isGoalValid => fitnessGoal != null;
  bool get isActivityValid => activityLevel != null;
}

namespace FitBodyApp.Domain.Enums;

public enum Gender { Male, Female, Other }

public enum FitnessGoal { LoseWeight, BuildMuscle, Maintain, ImproveEndurance }

public enum ActivityLevel { Sedentary, Light, Moderate, Active, VeryActive }

public enum UserStatus { Active, Locked, Deleted }

public enum UserRole { User, Moderator, Admin, SuperAdmin }

public enum AuthProviderType { Email, Google, Facebook, Fingerprint }

public enum DifficultyLevel { Beginner, Intermediate, Advanced }

public enum MealType { Breakfast, Lunch, Dinner, Snack }

public enum FavoritableType { Article, Video }

public enum ChallengeType { Weekly, Competition }

public enum NotificationType { WorkoutReminder, System }

public enum SupportTicketStatus { Open, Pending, Closed }

public enum SupportSenderType { User, Agent, Bot }

public enum ReportableType { ForumPost, Comment }

public enum ReportStatus { Pending, Reviewed, Dismissed }

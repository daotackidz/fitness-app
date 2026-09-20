using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Infrastructure.Persistence;

public class FitBodyDbContext : DbContext
{
    public FitBodyDbContext(DbContextOptions<FitBodyDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuthProvider> AuthProviders => Set<AuthProvider>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<RoutineExercise> RoutineExercises => Set<RoutineExercise>();
    public DbSet<WorkoutLog> WorkoutLogs => Set<WorkoutLog>();
    public DbSet<ProgressTracking> ProgressTrackings => Set<ProgressTracking>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeParticipant> ChallengeParticipants => Set<ChallengeParticipant>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportMessage> SupportMessages => Set<SupportMessage>();

    public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();
    public DbSet<ReportedContent> ReportedContents => Set<ReportedContent>();

    public DbSet<VideoFile> VideoFiles => Set<VideoFile>();
    public DbSet<ImageFile> ImageFiles => Set<ImageFile>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitBodyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

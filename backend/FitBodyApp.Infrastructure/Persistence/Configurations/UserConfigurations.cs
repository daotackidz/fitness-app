using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(100);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
        builder.Property(x => x.Gender).HasColumnName("gender").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
        builder.Property(x => x.HeightCm).HasColumnName("height_cm").HasColumnType("decimal(5,1)");
        builder.Property(x => x.WeightKg).HasColumnName("weight_kg").HasColumnType("decimal(5,1)");
        builder.Property(x => x.FitnessGoal).HasColumnName("fitness_goal").HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ActivityLevel).HasColumnName("activity_level").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.AvatarImageId).HasColumnName("avatar_image_id");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).HasDefaultValue(Domain.Enums.UserStatus.Active);
        builder.Property(x => x.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(20).HasDefaultValue(Domain.Enums.UserRole.User);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("idx_users_email");
        builder.HasIndex(x => x.Phone).IsUnique().HasDatabaseName("idx_users_phone");
        builder.HasIndex(x => x.Status).HasDatabaseName("idx_users_status");
        builder.HasIndex(x => x.Role).HasDatabaseName("idx_users_role");

        builder.HasOne(x => x.Settings).WithOne(x => x.User).HasForeignKey<UserSetting>(x => x.UserId);
        builder.HasOne(x => x.AvatarImage).WithMany().HasForeignKey(x => x.AvatarImageId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class AuthProviderConfiguration : IEntityTypeConfiguration<AuthProvider>
{
    public void Configure(EntityTypeBuilder<AuthProvider> builder)
    {
        builder.ToTable("auth_providers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Provider).HasColumnName("provider").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ProviderUid).HasColumnName("provider_uid").HasMaxLength(255);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UserId).HasColumnName("user_id");

        builder.HasIndex(x => new { x.Provider, x.ProviderUid }).IsUnique().HasDatabaseName("idx_auth_provider_user");
        builder.HasIndex(x => x.UserId).HasDatabaseName("idx_auth_providers_user_id");

        builder.HasOne(x => x.User).WithMany(x => x.AuthProviders).HasForeignKey(x => x.UserId);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(255);
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.Revoked).HasColumnName("revoked").HasDefaultValue(false);

        builder.HasIndex(x => x.UserId).HasDatabaseName("idx_refresh_tokens_user_id");
        builder.HasIndex(x => x.ExpiresAt).HasDatabaseName("idx_refresh_tokens_expires_at");

        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
    }
}

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.ToTable("user_settings");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.NotificationEnabled).HasColumnName("notification_enabled").HasDefaultValue(true);
        builder.Property(x => x.WorkoutReminderTime).HasColumnName("workout_reminder_time");
        builder.Property(x => x.Language).HasColumnName("language").HasMaxLength(10).HasDefaultValue("vi");
    }
}

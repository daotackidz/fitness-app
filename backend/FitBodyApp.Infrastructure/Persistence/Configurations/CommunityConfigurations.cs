using FitBodyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBodyApp.Infrastructure.Persistence.Configurations;

public class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
{
    public void Configure(EntityTypeBuilder<ForumPost> builder)
    {
        builder.ToTable("forum_posts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");
        builder.Property(x => x.LikesCount).HasColumnName("likes_count").HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.UserId).HasDatabaseName("idx_forum_posts_user_id");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_forum_posts_created_at");

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.ImageFile).WithMany().HasForeignKey(x => x.ImageFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("post_likes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PostId).HasColumnName("post_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.PostId, x.UserId }).IsUnique().HasDatabaseName("idx_post_likes_unique");

        builder.HasOne(x => x.Post).WithMany(x => x.Likes).HasForeignKey(x => x.PostId);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PostId).HasColumnName("post_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => x.PostId).HasDatabaseName("idx_comments_post_id");

        builder.HasOne(x => x.Post).WithMany(x => x.Comments).HasForeignKey(x => x.PostId);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable("challenges");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");
        builder.Property(x => x.GoalMetric).HasColumnName("goal_metric").HasMaxLength(100);
        builder.Property(x => x.Reward).HasColumnName("reward").HasMaxLength(150);
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");

        builder.HasIndex(x => new { x.Type, x.StartDate, x.EndDate }).HasDatabaseName("idx_challenges_type_dates");
        builder.HasOne(x => x.ImageFile).WithMany().HasForeignKey(x => x.ImageFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ChallengeParticipantConfiguration : IEntityTypeConfiguration<ChallengeParticipant>
{
    public void Configure(EntityTypeBuilder<ChallengeParticipant> builder)
    {
        builder.ToTable("challenge_participants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChallengeId).HasColumnName("challenge_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Progress).HasColumnName("progress").HasColumnType("decimal(5,2)");
        builder.Property(x => x.Rank).HasColumnName("rank");
        builder.Property(x => x.JoinedAt).HasColumnName("joined_at");

        builder.HasIndex(x => new { x.ChallengeId, x.UserId }).IsUnique().HasDatabaseName("idx_challenge_participant");
        builder.HasIndex(x => x.ChallengeId).HasDatabaseName("idx_challenge_participants_challenge_id");

        builder.HasOne(x => x.Challenge).WithMany(x => x.Participants).HasForeignKey(x => x.ChallengeId);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}

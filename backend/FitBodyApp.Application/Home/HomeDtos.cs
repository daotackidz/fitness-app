using FitBodyApp.Application.Community;

namespace FitBodyApp.Application.Home;

public record HomeExerciseCardDto(Guid Id, string Name, string? ImageUrl, int? DurationMinutes, int? CaloriesEstimate, bool IsFavorited);

public record HomeArticleCardDto(Guid Id, string Title, string? CoverImage, bool IsFavorited);

public record HomeDto(string FullName, List<HomeExerciseCardDto> Recommendations, ChallengeDto? FeaturedChallenge, List<HomeArticleCardDto> Articles);

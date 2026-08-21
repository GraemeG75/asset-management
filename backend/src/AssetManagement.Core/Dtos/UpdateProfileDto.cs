namespace AssetManagement.Core.Dtos
{
    public record UpdateProfileDto(
        string FirstName,
        string LastName,
        string? PreferredLanguage,
        string? AvatarUrl
    );
}

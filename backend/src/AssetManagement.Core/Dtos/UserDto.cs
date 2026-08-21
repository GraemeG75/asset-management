using System;

namespace AssetManagement.Core.Dtos
{
    public record UserDto(
        string Id,
        string FirstName,
        string LastName,
        string Name,
        string Email,
        string Role,
        string Provider,
        string? AvatarUrl,
        string PreferredLanguage,
        DateTime CreatedAt
    );
}

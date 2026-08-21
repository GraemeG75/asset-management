using System;

namespace AssetManagement.Core.Dtos
{
    public record UserDto(
        string Id,
        string Name,
        string Email,
        string Role,
        string Provider,
        string? AvatarUrl,
        DateTime CreatedAt
    );
}

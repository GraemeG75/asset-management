namespace AssetManagement.Core.Dtos;

public record LoginRequestDto(string Email, string? Password, bool RememberMe = true);

public record SsoLoginRequestDto(string Provider, bool RememberMe = true);

public record UserDto(
    string Id,
    string Name,
    string Email,
    string Role,
    string Provider,
    string? AvatarUrl,
    DateTime CreatedAt
);

public record AuthResponseDto(
    UserDto User,
    string Token,
    long ExpiresAt
);

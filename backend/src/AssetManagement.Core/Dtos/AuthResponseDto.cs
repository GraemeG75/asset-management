namespace AssetManagement.Core.Dtos
{
    public record AuthResponseDto(
        UserDto User,
        string Token,
        long ExpiresAt
    );
}

namespace AssetManagement.Core.Dtos
{
    public record LoginRequestDto(string Email, string? Password, bool RememberMe = true);
}

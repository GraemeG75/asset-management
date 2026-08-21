namespace AssetManagement.Core.Dtos
{
    public record SsoLoginRequestDto(string Provider, bool RememberMe = true);
}

using System;
using AssetManagement.Core.Generated.PickLists;

namespace AssetManagement.Core.Dtos
{
    public record UserDto(string Id, string FirstName, string LastName, string Name, string Email, int Role, string Provider, string? AvatarUrl, string PreferredLanguage, DateTime DateCreated)
    {
        public UserRolesEnum RoleEnum => (UserRolesEnum)Role;
        public string RoleName => UserRolesPickList.GetName(Role);
    }
}

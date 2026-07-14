using Microsoft.AspNetCore.Identity;

namespace WebLes1Nike.Data.Entities.Identity;

public class RoleEntity : IdentityRole<int>
{
    public ICollection<UserRoleEntity>? UsersRoles { get; set; }
}
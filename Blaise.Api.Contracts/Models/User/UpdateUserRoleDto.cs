namespace Blaise.Api.Contracts.Models.User
{
    public class UpdateUserRoleDto : UserAuditInfoDto
    {
        public string Role { get; set; }
    }
}

namespace SoundSphereApi.DTOs.Admin
{
    public class UserGetDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}

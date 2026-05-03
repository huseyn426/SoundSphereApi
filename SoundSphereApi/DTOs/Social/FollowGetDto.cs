namespace SoundSphereApi.DTOs.Social
{
    public class FollowGetDto
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public string FollowerUserName { get; set; } = null!;
        public int FollowingId { get; set; }
        public string FollowingUserName { get; set; } = null!;
    }
}

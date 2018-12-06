namespace Assets.Scripts.DTO
{
    public class FullFriendInfoModel
    {
        public FriendModel[] Friends { get; set; }
        public FriendModel[] FriendRequests { get; set; }
        public string FriendsHash { get; set; }
    }
}

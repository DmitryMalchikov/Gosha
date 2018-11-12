using System.Collections.Generic;

public class FullFriendInfoModel
{
    public List<FriendModel> Friends { get; set; }
    public List<FriendModel> FriendRequests { get; set; }
    public string FriendsHash { get; set; }
}

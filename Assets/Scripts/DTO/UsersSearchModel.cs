using System.Collections.Generic;

public class UsersSearchModel
{
    public List<FriendRequestModel> Users { get; set; }
    public int TotalCount;

    public UsersSearchModel()
    {
        Users = new List<FriendRequestModel>();
    }
}

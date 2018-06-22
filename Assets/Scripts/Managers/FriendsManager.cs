using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendsManager : Manager
{

    public static FriendsManager Instance { get; private set; }

    public string GetFriendsOffersUrl = "/api/friends/friendoffers";
    public string GetFriendsUrl = "/api/friends/friendslist";
    public string OfferFriendshipUrl = "/api/friends/offerfriendship";
    public string AcceptFriendshipUrl = "/api/friends/acceptfrienship";
    public string DeclineFriendshipUrl = "/api/friends/declinefrienship";
    public string SearchFriendsUrl = "/api/friends/friendssearch";
    public string SearchPlayersUrl = "/api/friends/playerssearch";

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetUrls();
    }

    public void SetUrls()
    {
        ServerInfo.SetUrl(ref GetFriendsOffersUrl);
        ServerInfo.SetUrl(ref GetFriendsUrl);
        ServerInfo.SetUrl(ref OfferFriendshipUrl);
        ServerInfo.SetUrl(ref AcceptFriendshipUrl);
        ServerInfo.SetUrl(ref DeclineFriendshipUrl);
        ServerInfo.SetUrl(ref SearchFriendsUrl);
        ServerInfo.SetUrl(ref SearchPlayersUrl);
    }

    public void GetFriendsAsync(ResultCallback callback = null)
    {
        CoroutineManager.SendRequest(GetFriendsUrl, null, (FullFriendInfoModel model) =>
        {
            GameController.SetHash("FriendsHash", model.FriendsHash);

            Canvaser.Instance.FriendsPanel.SetFriends(model.Friends);
            Canvaser.Instance.FriendsPanel.SetFriendRequests(model.FriendRequests);

            if (callback != null)
            {
                callback();
            }
        }, type: DataType.Friends, loadingPanelsKey: "friends");
    }

    public void GetFriendsOffersAsync()
    {
        CoroutineManager.SendRequest(GetFriendsOffersUrl, null, (List<FriendOfferModel> friends) =>
        {
            Canvaser.Instance.FriendsPanel.SetFriendOffers(friends);
        }, loadingPanelsKey: "addfriend");
    }

    public void OfferFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        CoroutineManager.SendRequest(OfferFriendshipUrl, input, () =>
       {
           Debug.Log("OK");
            //show info
        });
    }

    public void AcceptFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        CoroutineManager.SendRequest(AcceptFriendshipUrl, input, () =>
       {
           Debug.Log("OK");
            //show info
        });
    }

    public void DeclineFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        CoroutineManager.SendRequest(DeclineFriendshipUrl, input, () =>
       {
           Debug.Log("OK");
            //show info
        });
    }

    public void Search()
    {
        SearchPlayersAsync("@");
    }

    //public void SearchFriendsAsync(string searchString, int page, int itemsPerPage)
    //{
    //    PlayerSearchModel search = new PlayerSearchModel() { SearchString = searchString, Page = page, ItemsPerPage = itemsPerPage };

    //    StartCoroutine(NetworkHelper.SendRequest(SearchFriendsUrl, search,  (response) =>
    //    {
    //        Debug.Log("OK");
    //        Debug.Log("OK");
    //        FriendsSearchModel result = JsonConvert.DeserializeObject<FriendsSearchModel>(response.Text);
    //        //show info
    //    }));
    //}

    public void SearchPlayersAsync(string searchString)
    {
        InputString search = new InputString() { Value = searchString };

        CoroutineManager.SendRequest(SearchPlayersUrl, search, (FriendOfferModel result) =>
       {
           if (result.Id != 0)
           {
               Canvaser.Instance.FriendsPanel.PlayerSearchResult.SetPlayerObject(result);
               Canvaser.Instance.FriendsPanel.PlayerSearchResult.gameObject.SetActive(true);
           }
           else
           {
               Canvaser.Instance.FriendsPanel.PlayerSearchResult.Warning.SetActive(true);
               Canvaser.Instance.FriendsPanel.PlayerSearchResult.gameObject.SetActive(false);
           }
            //show info
        });
    }
}

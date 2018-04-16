using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{

    public static FriendsManager Instance { get; private set; }
    public string GetFriendsOffersUrl = "/api/friends/friendoffers";

    public string GetFriendsRequestsUrl = "/api/friends/friendrequests";
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
        GetFriendsOffersUrl = ServerInfo.GetUrl(GetFriendsOffersUrl);
        GetFriendsRequestsUrl = ServerInfo.GetUrl(GetFriendsRequestsUrl);
        GetFriendsUrl = ServerInfo.GetUrl(GetFriendsUrl);
        OfferFriendshipUrl = ServerInfo.GetUrl(OfferFriendshipUrl);
        AcceptFriendshipUrl = ServerInfo.GetUrl(AcceptFriendshipUrl);
        DeclineFriendshipUrl = ServerInfo.GetUrl(DeclineFriendshipUrl);
        SearchFriendsUrl = ServerInfo.GetUrl(SearchFriendsUrl);
        SearchPlayersUrl = ServerInfo.GetUrl(SearchPlayersUrl);
    }

    public void GetFriendsRequestsAsync(ResultCallback callback=null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetFriendsRequestsUrl, null, "application/x-www-form-urlencoded", (response) =>
        {
            Debug.Log("OK");
            List<FriendModel> friendRequests = JsonConvert.DeserializeObject<List<FriendModel>>(response.Text);
            Canvaser.Instance.FriendsPanel.SetFriendRequests(friendRequests);

            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void GetFriendsAsync(ResultCallback callback = null)
    {
        StartCoroutine(NetworkHelper.SendRequest(GetFriendsUrl, null, "application/x-www-form-urlencoded", (response) =>
        {
            Debug.Log("OK");
            List<FriendModel> friends = JsonConvert.DeserializeObject<List<FriendModel>>(response.Text);
            Canvaser.Instance.FriendsPanel.SetFriends(friends);

            if (callback != null)
            {
                callback();
            }
        }));
    }

    public void GetFriendsOffersAsync()
    {
        StartCoroutine(NetworkHelper.SendRequest(GetFriendsOffersUrl, null, "application/x-www-form-urlencoded", (response) =>
        {
            Debug.Log("OK");
            List<FriendOfferModel> friends = JsonConvert.DeserializeObject<List<FriendOfferModel>>(response.Text);
            Canvaser.Instance.FriendsPanel.SetFriendOffers(friends);
        }));
    }

    public void OfferFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        StartCoroutine(NetworkHelper.SendRequest(OfferFriendshipUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            //show info
        }));
    }

    public void AcceptFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        StartCoroutine(NetworkHelper.SendRequest(AcceptFriendshipUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            //show info
        }));
    }

    public void DeclineFriendshipAsync(int userId)
    {
        InputInt input = new InputInt() { Value = userId };

        StartCoroutine(NetworkHelper.SendRequest(DeclineFriendshipUrl, input, "application/json", (response) =>
        {
            Debug.Log("OK");
            //show info
        }));
    }

    public void Search()
    {
        SearchPlayersAsync("@");
    }

    public void SearchFriendsAsync(string searchString, int page, int itemsPerPage)
    {
        PlayerSearchModel search = new PlayerSearchModel() { SearchString = searchString, Page = page, ItemsPerPage = itemsPerPage };

        StartCoroutine(NetworkHelper.SendRequest(SearchFriendsUrl, search, "application/json", (response) =>
        {
            Debug.Log("OK");
            Debug.Log("OK");
            FriendsSearchModel result = JsonConvert.DeserializeObject<FriendsSearchModel>(response.Text);
            //show info
        }));
    }

    public void SearchPlayersAsync(string searchString)
    {
        InputString search = new InputString() { Value = searchString };

        StartCoroutine(NetworkHelper.SendRequest(SearchPlayersUrl, search, "application/json", (response) =>
        {
            Debug.Log("OK");
            Debug.Log("OK");
            FriendOfferModel result = JsonConvert.DeserializeObject<FriendOfferModel>(response.Text);
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
        }));
    }
}

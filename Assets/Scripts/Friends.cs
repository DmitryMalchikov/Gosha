using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friends : MonoBehaviour
{

    public Transform RequestsContent;
    public Transform FriendsContent;
    public Transform FriendOffersContent;
    public Transform PlayerSearchContent;

    public GameObject RequestObject;
    public GameObject FriendObject;
    public GameObject FriendOfferObject;

    public UserInfo RequesterInfo;
    public UserInfo FriendInfo;
    public UserInfo FriendOfferInfo;
    public GameObject FriendAdded;

    public GameObject SearchPlayer;

    public InputField PlayerSearchInput;
    public InputField FriendSearchInput;

    public FriendObject PlayerSearchResult;

    public GameObject DuelPanel;
    public int PlayerForDuelID;
    public InputField DuelBet;
    public Text DuelBetPlaceholder;

    public GameObject DuelOfferedPanel;

    public FriendModel TraderFriend;

    public List<FriendObject> FriendObjects;
    public List<FriendObject> FriendRequestsObjects;

    public GameObject NoFriendsMsg;
    public GameObject NoRequestsMsg;
    public GameObject FriendNotFoundMsg;
    public Text DuelWarningMsg;

    public Toggle FriendsToggle;
    public Toggle RequestsToggle;

    public bool IsFriend(int id)
    {
        return FriendObjects.Find(x => x.Info.Id == id) != null;
    }

    public void OpenDirectlyRequests()
    {
        CleanContent(FriendsContent);
        CleanContent(RequestsContent);
        
        FriendsManager.Instance.GetFriendsAsync(GetComponent<Panel>().LoadingPanels,() => OpenRequests());
    }

    void OpenRequests()
    {
        gameObject.SetActive(true);
        RequestsToggle.isOn = true;
    }

    public void OpenFriendsPanel()
    {
        gameObject.SetActive(true);
        CleanContent(FriendsContent);
        CleanContent(RequestsContent);
        
        FriendsManager.Instance.GetFriendsAsync(this.LoadingPanels());
    }

    public void SetFriendRequests(List<FriendModel> requests)
    {
        NoRequestsMsg.SetActive(requests.Count == 0);
        SetContentWith(requests, RequestsContent, RequestObject);
        FriendRequestsObjects = new List<FriendObject>(FriendsContent.GetComponentsInChildren<FriendObject>());
    }

    public void SetFriends(List<FriendModel> friends)
    {
        NoFriendsMsg.SetActive(friends.Count == 0);
        SetContentWith(friends, FriendsContent, FriendObject);
        FriendObjects = new List<FriendObject>(FriendsContent.GetComponentsInChildren<FriendObject>());
        FriendNotFoundMsg.SetActive(false);
        FriendSearchInput.text = "";
    }

    public void SetFriendOffers(List<FriendOfferModel> friends)
    {
        if(!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        for (int i = 0; i < friends.Count; i++)
        {
            var newFriend = Instantiate(FriendOfferObject, FriendOffersContent);
            FriendObject friendObj = newFriend.GetComponent<FriendObject>();
            if (friends[i].Id == LoginManager.Instance.User.Id)
            {
                friendObj.YourPanel(friends[i].Place);
            }
            else
            {
                friendObj.SetPlayerObject(friends[i]);
            }
        }
    }

    public void RemoveFromFriends(int id)
    {
        var toDelete = FriendObjects.Find(fo => fo.OfferInfo.Id == id);
        Destroy(toDelete.gameObject);
    }

    public void RemoveFromFriendRequests(int id)
    {
        var toDelete = FriendRequestsObjects.Find(fo => fo.OfferInfo.Id == id);
        Destroy(toDelete.gameObject);
    }

    public void SetContentWith(List<FriendModel> friends, Transform content, GameObject item)
    {
        for (int i = 0; i < friends.Count; i++)
        {
            AddFriendTo(friends[i], content, item);
        }
    }

    public void AddFriendTo(FriendModel friend, Transform content, GameObject item)
    {
        var newFriend = Instantiate(item, content);
        FriendObject friendObj = newFriend.GetComponent<FriendObject>();
        friendObj.SetFriendObject(friend);
    }

    public void AddFriendToRequests(FriendModel friend)
    {
        AddFriendTo(friend, RequestsContent, RequestObject);
    }
    public void AddFriendToFriends(FriendModel friend)
    {
        AddFriendTo(friend, FriendsContent, FriendObject);
    }

    public void AddOfferToFriends(FriendOfferModel friend)
    {
        var newFriend = Instantiate(FriendOfferObject, FriendOffersContent);
        FriendObject friendObj = newFriend.GetComponent<FriendObject>();
        friendObj.SetPlayerObject(friend);
    }

    public void OpenFriendOffersPanel()
    {
        CleanContent(FriendOffersContent);
        FriendsManager.Instance.GetFriendsOffersAsync(SearchPlayer.LoadingPanels());
        SearchPlayer.SetActive(true);
    }

    public void CleanContent(Transform content)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
    }

    public void SearchFriend(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            for (int i = 0; i < FriendObjects.Count; i++)
            {
                FriendObjects[i].gameObject.SetActive(true);
            }
            FriendNotFoundMsg.SetActive(false);
        }
        else
        {
            bool found = false;
            for (int i = 0; i < FriendObjects.Count; i++)
            {
                if (!FriendObjects[i].Info.Nickname.ToLower().Contains(name.ToLower()))
                {
                    FriendObjects[i].gameObject.SetActive(false);
                }
                else
                {
                    found = true;
                    FriendObjects[i].gameObject.SetActive(true);
                }
            }
            FriendNotFoundMsg.SetActive(!found);
        }
    }

    public void SearchFriend()
    {
        SearchFriend(FriendSearchInput.text);
    }

    public void SearchPlayerByName()
    {
        FriendsManager.Instance.SearchPlayersAsync(PlayerSearchInput.text);
    }

    public void OpenDuelPanel(int PlayerID)
    {
        PlayerForDuelID = PlayerID;
        DuelPanel.SetActive(true);
    }

    public void CorrectBet(string bet)
    {
        if (int.Parse(bet) > LoginManager.Instance.User.IceCream)
        {
            DuelBet.text = LoginManager.Instance.User.IceCream.ToString();
        }
    }

    public void SetDuel()
    {
        if (!string.IsNullOrEmpty(DuelBet.text))
        {
            DuelManager.Instance.OfferDuelAsync(PlayerForDuelID, int.Parse(DuelBet.text));
            DuelBet.text = "";
            DuelBetPlaceholder.text = "";
        }
        else
        {
            DuelBetPlaceholder.text = LocalizationManager.GetLocalizedValue("enterduelbet");
        }
    }
    public void DuelOfferAnswer()
    {
        DuelPanel.SetActive(false);
        DuelOfferedPanel.SetActive(true);
        DuelWarningMsg.gameObject.SetActive(false);
    }

    public void SetDuelWarning(string errorMsg)
    {
        DuelWarningMsg.text = LocalizationManager.GetLocalizedValue(errorMsg);
        DuelWarningMsg.gameObject.SetActive(true);
    }

    public void CloseDuelOffer()
    {
        DuelPanel.SetActive(false);
        DuelWarningMsg.gameObject.SetActive(false);
    }

    public void Trade(FriendModel info)
    {
        TraderFriend = info;
        TradeManager.Instance.GetTradeItemsAsync(LoginManager.Instance.User.Id);
    }
}

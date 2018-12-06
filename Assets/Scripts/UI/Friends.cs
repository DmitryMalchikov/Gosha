using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
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

        public FriendObject[] FriendObjects;
        public FriendObject[] FriendRequestsObjects;

        public GameObject NoFriendsMsg;
        public GameObject NoRequestsMsg;
        public GameObject FriendNotFoundMsg;
        public Text DuelWarningMsg;

        public Toggle FriendsToggle;
        public Toggle RequestsToggle;

        public bool IsFriend(int id)
        {
            return FriendObjects.Any(x => x.Info.Id == id);
        }

        public void OpenDirectlyRequests()
        {
            FriendsContent.ClearContent();
            RequestsContent.ClearContent();
            FriendsManager.Instance.GetFriendsAsync(() => OpenRequests());
        }

        void OpenRequests()
        {
            gameObject.SetActive(true);
            RequestsToggle.isOn = true;
        }

        public void OpenFriendsPanel()
        {
            if (!Canvaser.Instance.IsLoggedIn()) return;
            gameObject.SetActive(true);
            FriendsContent.ClearContent();
            RequestsContent.ClearContent();

            FriendsManager.Instance.GetFriendsAsync();
        }

        public void SetFriendRequests(FriendModel[] requests)
        {
            NoRequestsMsg.SetActive(requests.Length == 0);
            SetContentWith(requests, RequestsContent, RequestObject);
            FriendRequestsObjects = FriendsContent.GetComponentsInChildren<FriendObject>();
        }

        public void SetFriends(FriendModel[] friends)
        {
            NoFriendsMsg.SetActive(friends.Length == 0);
            SetContentWith(friends, FriendsContent, FriendObject);
            FriendObjects = FriendsContent.GetComponentsInChildren<FriendObject>();
            FriendNotFoundMsg.SetActive(false);
            FriendSearchInput.text = string.Empty;
        }

        public void SetFriendOffers(List<FriendOfferModel> friends)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            for (int i = 0; i < friends.Count; i++)
            {
                var newFriend = Instantiate(FriendOfferObject, FriendOffersContent);
                FriendObject friendObj = newFriend.GetComponent<FriendObject>();
                if (friends[i].Id == LoginManager.User.Id)
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
            var toDelete = FriendObjects.FirstOrDefault(fo => fo.Info.Id == id);
            Destroy(toDelete.gameObject);
        }

        public void RemoveFromFriendRequests(int id)
        {
            var toDelete = FriendRequestsObjects.FirstOrDefault(fo => fo.Info.Id == id);
            Destroy(toDelete.gameObject);
        }

        public void SetContentWith(FriendModel[] friends, Transform content, GameObject item)
        {
            for (int i = 0; i < friends.Length; i++)
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
            FriendOffersContent.ClearContent();
            FriendsManager.Instance.GetFriendsOffersAsync();
            PlayerSearchInput.text = "";
            PlayerSearchResult.gameObject.SetActive(false);
            SearchPlayer.SetActive(true);
        }

        public void SearchFriend(string name)
        {
            bool found = false;
            for (int i = 0; i < FriendObjects.Length; i++)
            {
                if (FriendObjects[i].Info.Nickname.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = true;
                    FriendObjects[i].gameObject.SetActive(true);
                }
                else
                {
                    FriendObjects[i].gameObject.SetActive(false);
                }
            }
            FriendNotFoundMsg.SetActive(!found);
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
            if (int.Parse(bet) > LoginManager.User.IceCream)
            {
                DuelBet.text = LoginManager.User.IceCream.ToString();
            }
        }

        public void SetDuel()
        {
            if (!string.IsNullOrEmpty(DuelBet.text))
            {
                DuelManager.Instance.OfferDuelAsync(PlayerForDuelID, int.Parse(DuelBet.text));
                DuelBet.text = string.Empty;
                DuelBetPlaceholder.text = string.Empty;
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
            TradeManager.Instance.GetTradeItemsAsync(LoginManager.User.Id);
        }
    }
}

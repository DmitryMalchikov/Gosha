using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UserInfo : MonoBehaviour
    {
        private FriendObject _info;
        public Text Name;
        public Text Region;
        public Text Record;
        public Text Duels;
        public Text IceCreamCount;
        public Text AddToFriendsText;
        public Button AddToFriendsBtn;
        public Image Avatar;
        public Button TradeBtn;

        public void SetInfo(FriendObject user, bool isFriend = true)
        {
            SetUserObject(user);
            if (isFriend)
            {
                TradeBtn.interactable = LoginManager.User.CanOfferTrade;
            }
            gameObject.SetActive(true);
        }

        public void SetOfferInfo(FriendObject user, bool requestSent = false)
        {
            SetUserObject(user);
            SetAddToFriendsButton(requestSent);
            gameObject.SetActive(true);
        }

        public void SetUserObject(FriendObject user)
        {
            SetAvatarDownloadedCallback(user);
            _info = user;
            Name.text = user.Info.Nickname;
            Region.text = user.Info.Region;
            Record.text = LocalizationManager.GetLocalizedValue("highscore") + user.Info.Highscore.ToString();
            Duels.text = LocalizationManager.GetLocalizedValue("duelwins") + user.Info.DuelWins.ToString();
            IceCreamCount.text = user.Info.IceCream.ToString();
            Avatar.sprite = user.Avatar.sprite;
        }

        public void SetAvatarDownloadedCallback(FriendObject user)
        {
            if (_info != null)
            {
                _info.OnAvatarDownloaded -= SetSprite;
            }
            user.OnAvatarDownloaded += SetSprite;
        }

        public void SetSprite(Sprite sprite)
        {
            Avatar.sprite = sprite;
        }

        private void SetAddToFriendsButton(bool alreadySent)
        {
            if (alreadySent)
            {
                AddToFriendsBtn.interactable = false;
                AddToFriendsText.text = LocalizationManager.GetLocalizedValue("friendrequestalreadysend");
            }
            else if (AddToFriendsBtn)
            {
                AddToFriendsBtn.interactable = true;
                AddToFriendsText.text = LocalizationManager.GetLocalizedValue("addtofriends");
            }
        }

        public void OfferOrAddFriend()
        {
            if (Canvaser.Instance.IsLoggedIn())
            {
                FriendshipStatus newStatus = _info.OfferOrAddfriend();
                if (newStatus == FriendshipStatus.OutgoingRequest)
                {
                    SetAddToFriendsButton(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void AddFriend(bool toAdd)
        {
            _info.AcceptFriend(toAdd);
            gameObject.SetActive(false);
        }

        public void Trade()
        {
            _info.Trade();
            gameObject.SetActive(false);
        }

        public void Duel()
        {
            if (Canvaser.Instance.IsLoggedIn())
            {
                Canvaser.Instance.FriendsPanel.OpenDuelPanel(_info.Info.Id);
                gameObject.SetActive(false);
            }
        }
    }
}

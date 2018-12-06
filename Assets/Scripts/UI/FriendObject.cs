using System;
using Assets.Scripts.DTO;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FriendObject : MonoBehaviour, IAvatarSprite
    {
        public FriendModel Info;

        public FriendOfferModel OfferInfo
        {
            get
            {
                return Info as FriendOfferModel;
            }
        }

        public Text Name;
        public Text Record;
        public Text Position;
        public bool isFriend;
        public GameObject Warning;
        public GameObject OpenText;
        public GameObject InfoButton;
        public Image Avatar;
        public GameObject YouPanel;
        public Color YourColor = Color.green;

        public event Action<Sprite> OnAvatarDownloaded;

        public Sprite AvatarSprite
        {
            set
            {
                Avatar.sprite = value;
            }
        }

        public void AcceptFriend(bool isAccepted)
        {
            if (isAccepted)
            {
                FriendsManager.Instance.AcceptFriendshipAsync(Info.Id);
                Canvaser.Instance.FriendsPanel.AddFriendToFriends(Info);
            }
            else
            {
                FriendsManager.Instance.DeclineFriendshipAsync(Info.Id);
            }

            Destroy(gameObject);
        }

        public void ShowPlayerInfo()
        {
            if (OfferInfo.FriendshipStatus != FriendshipStatus.AreFriends)
            {
                Canvaser.Instance.FriendsPanel.FriendOfferInfo.SetOfferInfo(this, OfferInfo.FriendshipStatus == FriendshipStatus.OutgoingRequest);
            }
            else
            {
                Canvaser.Instance.FriendsPanel.FriendInfo.SetOfferInfo(this);
            }
        }

        public FriendshipStatus OfferOrAddfriend()
        {
            if (OfferInfo.FriendshipStatus == FriendshipStatus.NotFriends)
            {
                FriendsManager.Instance.OfferFriendshipAsync(Info.Id);
                OfferInfo.FriendshipStatus = FriendshipStatus.OutgoingRequest;
            }
            else
            {
                FriendsManager.Instance.AcceptFriendshipAsync(Info.Id);
                OfferInfo.FriendshipStatus = FriendshipStatus.AreFriends;
            }

            return OfferInfo.FriendshipStatus;
        }

        public void ShowFriendInfo()
        {
            if (Warning && Warning.activeInHierarchy) return;

            if (isFriend)
                Canvaser.Instance.FriendsPanel.FriendInfo.SetInfo(this);
            else
                Canvaser.Instance.FriendsPanel.RequesterInfo.SetInfo(this, false);
        }

        public void SetFriendObject(FriendModel friend)
        {
            Info = friend;
            Name.text = friend.Nickname;
            if (isFriend)
                Record.text = friend.Highscore + LocalizationManager.GetLocalizedValue("meter");
            LoginManager.Instance.GetUserImage(this, friend.Id);
        }

        public void SetDuelPlayerObject(PlayerDuelModel friend)
        {
            Name.text = friend.Nickname;
            if (isFriend)
                Record.text = friend.Result + LocalizationManager.GetLocalizedValue("meter");
            LoginManager.Instance.GetUserImage(this, friend.Id);
        }

        public void SetPlayerObject(FriendOfferModel friendOffer)
        {
            if (Warning)
                Warning.SetActive(false);
            //if (friendOffer.FriendshipStatus == 0)
            //{
            Info = friendOffer;
            //}
            if (Position)
                Position.text = friendOffer.Place.ToString();
            Name.text = friendOffer.Nickname;
            LoginManager.Instance.GetUserImage(this, friendOffer.Id);
        }

        public void YourPanel(int position)
        {
            OpenText.SetActive(false);
            InfoButton.SetActive(false);
            Name.text = LocalizationManager.GetLocalizedValue("you");
            Position.text = position.ToString();
            Position.color = Color.white;
            YouPanel.SetActive(true);
            Name.color = Color.white;
            Avatar.sprite = Canvaser.Instance.Avatar;
            Destroy(gameObject.GetComponent<Button>());
        }

        public void YourPanelTournament()
        {
            GetComponent<Image>().color = YourColor;
            Name.color = Color.white;
            Record.color = Color.white;
            Destroy(gameObject.GetComponent<Button>());
        }

        public void SetInfo(FriendOfferStatisticsModel friend, int place)
        {
            bool you = friend.Id == LoginManager.User.Id;
            string nickname = you ? LocalizationManager.GetLocalizedValue("you") : friend.Nickname;

            Info = friend;
            Name.text = string.Format("{0}. {1}", place + 1, nickname);
            Record.text = friend.Points.ToString() + LocalizationManager.GetLocalizedValue("meter");

            if (you)
            {
                YourPanelTournament();
            }
        }

        public void SetStatisticsObject(FriendOfferStatisticsModel friend, int place)
        {
            SetInfo(friend, place);
            LoginManager.Instance.GetUserImage(this, friend.Id);
        }

        public void Trade()
        {
            Canvaser.Instance.ClearOffer();
            Canvaser.Instance.FriendsPanel.Trade(Info);
        }

        public void ShowLeaderInfo()
        {
            if (Canvaser.Instance.FriendsPanel.IsFriend(Info.Id))
            {
                Canvaser.Instance.FriendsPanel.FriendInfo.SetOfferInfo(this);
            }
            else
            {
                Canvaser.Instance.FriendsPanel.FriendOfferInfo.SetOfferInfo(this, OfferInfo.FriendshipStatus == FriendshipStatus.OutgoingRequest);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (Avatar)
            {
                Avatar.sprite = sprite;
            }

            if (OnAvatarDownloaded != null)
            {
                OnAvatarDownloaded(sprite);
            }
        }
    }
}

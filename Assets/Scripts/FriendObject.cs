using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendObject : MonoBehaviour
{

    public FriendModel Info;

    public FriendOfferModel OfferInfo;

    public Text Name;
    public Text Record;

    public Text Position;

    public bool isFriend;

    public GameObject Warning;

    public GameObject OpenText;
    public GameObject InfoButton;

    public Image Avatar;

	public GameObject YouPanel;


    public void AcceptFriend(bool isAccepted)
    {
        if (isAccepted)
        {
            FriendsManager.Instance.AcceptFriendshipAsync(Info.Id);
            Canvaser.Instance.FriendsPanel.AddFriendToFriends(Info);
            //add to friends list
        }
        else
        {
            FriendsManager.Instance.DeclineFriendshipAsync(Info.Id);
        }

        Destroy(gameObject);
        //remove from requests
    }

    public void AcceptOffer(bool isAccepted)
    {
        if (isAccepted)
        {
            FriendsManager.Instance.AcceptFriendshipAsync(OfferInfo.Id);
            Canvaser.Instance.FriendsPanel.AddOfferToFriends(OfferInfo);
            //add to friends list
        }
        else
        {
            FriendsManager.Instance.DeclineFriendshipAsync(OfferInfo.Id);
        }

        Destroy(gameObject);
        //remove from requests
    }

    public void RequestDuel()
    {

    }


    public void ShowPlayerInfo()
    {
        if (OfferInfo.FriendshipStatus != 1)
        {
            Canvaser.Instance.FriendsPanel.FriendOfferInfo.SetOfferInfo(this);
        }
        else
        {
            Canvaser.Instance.FriendsPanel.FriendInfo.SetOfferInfo(this);
        }
    }

    public void OfferOrAddfriend()
    {
        Debug.Log(OfferInfo.FriendshipStatus);
        if (OfferInfo.FriendshipStatus == 2)
        {
            FriendsManager.Instance.OfferFriendshipAsync(OfferInfo.Id);
        }
        else
        {
            FriendsManager.Instance.AcceptFriendshipAsync(OfferInfo.Id);
        }
    }

    public void ShowFriendInfo()
    {
        if (!Warning || !Warning.activeInHierarchy)
        {
            if (isFriend)
                Canvaser.Instance.FriendsPanel.FriendInfo.SetInfo(this);
            else
                Canvaser.Instance.FriendsPanel.RequesterInfo.SetInfo(this);
        }
    }

    public void SetFriendObject(FriendModel friend)
    {
        Info = friend;
        Name.text = friend.Nickname;
        if (isFriend)
            Record.text = friend.Highscore + LocalizationManager.GetLocalizedValue("meter");
        LoginManager.Instance.GetUserImage(friend.Id, Avatar);
    }

    public void SetDuelPlayerObject(PlayerDuelModel friend)
    {
        Name.text = friend.Nickname;
        if (isFriend)
            Record.text = friend.Result + LocalizationManager.GetLocalizedValue("meter");
        LoginManager.Instance.GetUserImage(friend.Id, Avatar);
    }

    public void SetPlayerObject(FriendOfferModel friendOffer)
    {
        if (Warning)
            Warning.SetActive(false);
        //if (friendOffer.FriendshipStatus == 0)
        //{
            OfferInfo = friendOffer;
        //}
        if (Position)
            Position.text = friendOffer.Place.ToString();
        Name.text = friendOffer.Nickname;
        LoginManager.Instance.GetUserImage(friendOffer.Id, Avatar);
    }

    public void YourPanel(int position)
    {
        OpenText.SetActive(false);
        InfoButton.SetActive(false);
        Name.text = LocalizationManager.GetLocalizedValue("you");
		Name.color = Color.white;
        Position.text = position.ToString();
		Position.color = Color.white;
		Avatar.sprite = Canvaser.Instance.Avatar;
		YouPanel.SetActive(true);
    }

    public void Trade()
    {
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
            Canvaser.Instance.FriendsPanel.FriendOfferInfo.SetOfferInfo(this);
        }
    }

    public void SetTournamentObject(FriendModel friend, int place)
    {
        Info = friend;
        Name.text = string.Format("{0}. {1}", place + 1, friend.Nickname);
        Record.text = friend.Highscore + LocalizationManager.GetLocalizedValue("meter");
        LoginManager.Instance.GetUserImage(friend.Id, Avatar);
    }
}

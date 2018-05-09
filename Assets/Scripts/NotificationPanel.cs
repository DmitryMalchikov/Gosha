using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour {

    public Text NotificationText;

    private NotificationType _currentType;

    public void OpenNotification(NotificationType type, int count)
    {
        _currentType = type;

        string typeText = "friendrequests";

        switch(type)
        {
            case NotificationType.DuelRequest:
                typeText = "duelrequests";
                break;
            case NotificationType.FriendRequest:
                typeText = "friendrequests";
                break;
            case NotificationType.TradeRequest:
                typeText = "traderequests";
                break;
        }

        NotificationText.text = string.Format(LocalizationManager.GetLocalizedValue("youhavenewnotifications"), count, LocalizationManager.GetLocalizedValue(typeText));

        gameObject.SetActive(true);

        Canvaser.Instance.OpenPanel(true);
    }

    public void GoToNotifications()
    {
        switch(_currentType)
        {
            case NotificationType.DuelRequest:
                Canvaser.Instance.Duels.Open();
                break;
            case NotificationType.FriendRequest:
                Canvaser.Instance.FriendsPanel.OpenDirectlyOffers();
                break;
            case NotificationType.TradeRequest:
                Canvaser.Instance.GetTrades();
                break;
        }
        Canvaser.Instance.OpenPanel(false);
        gameObject.SetActive(false);
    }
}

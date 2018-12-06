using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NotificationPanel : MonoBehaviour
    {
        public Text NotificationText;

        private NotificationType _currentType;

        public void OpenNotification(NotificationType type, int count)
        {
            _currentType = type;

            string typeText = "friendrequests";

            switch (type)
            {
                case NotificationType.DuelRequest:
                    typeText = "notificationduel";
                    break;
                case NotificationType.FriendRequest:
                    typeText = "notificationfriendship";
                    break;
                case NotificationType.TradeRequest:
                    typeText = "notificationtrade";
                    break;
            }

            NotificationText.text = string.Format(LocalizationManager.GetLocalizedValue("youhavenewnotifications"), LocalizationManager.GetLocalizedValue(typeText), count);

            gameObject.SetActive(true);
        }

        public void GoToNotifications()
        {
            switch (_currentType)
            {
                case NotificationType.DuelRequest:
                    Canvaser.Instance.Duels.OpenDirectlyRequests();
                    break;
                case NotificationType.FriendRequest:
                    Canvaser.Instance.FriendsPanel.OpenDirectlyRequests();
                    break;
                case NotificationType.TradeRequest:
                    Canvaser.Instance.GetTrades();
                    break;
            }
            gameObject.SetActive(false);
        }
    }
}

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uSignalR.Hubs;

public class NotificationsManager : Singleton<NotificationsManager>
{
    public delegate void NotificationReceivedHandler(int playerId, string playerNickname);
    public delegate void RegisterResultHandler();

    public event NotificationReceivedHandler OnFriendNotification;
    public event NotificationReceivedHandler OnTradeNotification;
    public event NotificationReceivedHandler OnDuelNotification;
    public event RegisterResultHandler OnRegisterSuccess;
    public event RegisterResultHandler OnRegisterError;

    IHubProxy proxy;
    HubConnection connection;
    static bool _initialized = false;
    static bool _connectionStarted = false;

    public List<NotificationObject> Notifications = new List<NotificationObject>();

    void Start()
    {
        string url = string.Empty;
        ServerInfo.SetUrl(ref url);
        connection = new HubConnection(url);
        proxy = connection.CreateProxy("NotificationsHub");

        proxy.Subscribe("FriendNotification").Data += data =>
        {
            Debug.Log("Notification");
            if (OnFriendNotification != null)
            {
                var parameters = data[0] as JToken;
                Notifications.Add(new NotificationObject(parameters["UserId"].ToObject<int>(), parameters["UserNickname"].ToString(), NotificationType.FriendRequest));
            }
        };

        proxy.Subscribe("TradeNotification").Data += data =>
        {
            Debug.Log("Trade not");
            if (OnTradeNotification != null)
            {
                var parameters = data[0] as JToken;
                Notifications.Add(new NotificationObject(parameters["UserId"].ToObject<int>(), parameters["UserNickname"].ToString(), NotificationType.TradeRequest));
            }
        };

        proxy.Subscribe("DuelNotification").Data += data =>
        {
            Debug.Log("Duel not");
            if (OnDuelNotification != null)
            {
                var parameters = data[0] as JToken;
                Notifications.Add(new NotificationObject(parameters["UserId"].ToObject<int>(), parameters["UserNickname"].ToString(), NotificationType.DuelRequest));
            }
        };

        connection.Start();
        _connectionStarted = true;

        StartCoroutine(CheckNotifications());
    }

    static bool _sending = false;

    public static void Register(int userId)
    {
        if (!_sending && !_initialized)
        {
            _sending = true;
            Instance.StartCoroutine(Instance.WaitPoxy(userId));
        }
    }

    IEnumerator WaitPoxy(int userId)
    {
        yield return new WaitUntil(() => _connectionStarted);
        proxy.Invoke<object>("Register", userId).ContinueWith((task) =>
        {
            Debug.Log("Registered");
            var result = task.Result as JToken;
            bool success = result["Registered"].ToObject<bool>();
            if (success)
            {
                if (OnRegisterSuccess != null)
                {
                    OnRegisterSuccess();
                }
            }
            else
            {
                if (OnRegisterError != null)
                {
                    OnRegisterError();
                }
            }
        });
        _initialized = true;
    }

    IEnumerator CheckNotifications()
    {
        while (true)
        {
            if (Notifications.Count == 0)
            {
                yield return null;
            }

            for (int i = 0; i < Notifications.Count; i++)
            {
                InvokeEvent(Notifications[i]);
                Notifications.RemoveAt(i);
                i--;
                yield return null;
            }
        }
    }

    private void InvokeEvent(NotificationObject not)
    {
        switch (not.Type)
        {
            case NotificationType.DuelRequest:
                OnDuelNotification(not.UserId, not.UserNickname);
                break;
            case NotificationType.FriendRequest:
                OnFriendNotification(not.UserId, not.UserNickname);
                break;
            case NotificationType.TradeRequest:
                OnTradeNotification(not.UserId, not.UserNickname);
                break;
        }
    }

    private void OnApplicationQuit()
    {
        connection.Stop();
    }
}

public enum NotificationType
{
    FriendRequest, DuelRequest, TradeRequest
}

public class NotificationObject
{
    public int UserId { get; set; }
    public string UserNickname { get; set; }
    public NotificationType Type { get; set; }

    public NotificationObject(int id, string nick, NotificationType type)
    {
        UserId = id;
        UserNickname = nick;
        Type = type;
    }
}

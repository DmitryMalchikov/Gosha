using Newtonsoft.Json.Linq;
using SignalR.Client._20.Hubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    public delegate void NotificationReceivedHandler(string playerNickname);

    public static NotificationsManager Instance;

    public event NotificationReceivedHandler OnFriendNotification;
    public event NotificationReceivedHandler OnTradeNotification;
    public event NotificationReceivedHandler OnDuelNotification;
    public event NotificationReceivedHandler OnRegisterSuccess;
    public event NotificationReceivedHandler OnRegisterError;

    IHubProxy proxy;
    HubConnection connection;
    static bool _initialized = false;
    static bool _connectionStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        connection = new HubConnection("http://goshagame-001-site2.atempurl.com");
        proxy = connection.CreateProxy("NotificationsHub");

        proxy.Subscribe("FriendNotification").Data += data =>
        {
            Debug.Log(data[0]);
            if (OnFriendNotification != null)
            {
                OnFriendNotification(data[0].ToString());
            }
        };

        proxy.Subscribe("TradeNotification").Data += data =>
        {
            Debug.Log("Trade not");
            if (OnTradeNotification != null)
            {
                OnTradeNotification(data[0].ToString());
            }
        };

        proxy.Subscribe("DuelNotification").Data += data =>
        {
            Debug.Log("Duel not");
            if (OnDuelNotification != null)
            {
                OnDuelNotification(data[0].ToString());
            }
        };

        connection.Start();
        _connectionStarted = true;
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
        proxy.Invoke("Register", userId).Finished += (sender, e) =>
        {
            var result = e.Result as JToken;
            bool success = result["Registered"].ToObject<bool>();
            if (success)
            {
                if (OnRegisterSuccess != null)
                {
                    OnRegisterSuccess("");
                }
            }
            else
            {
                if (OnRegisterError != null)
                {
                    OnRegisterError("");
                }
            }
        };
        _initialized = true;
    }

    private void OnApplicationQuit()
    {
        connection.Stop();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Canvaser : MonoBehaviour
{

    public static Canvaser Instance { get; private set; }
    public static bool ErrorChecked = false;
    public static bool Error = false;

    public GameObject GameOverPanel;
    public Text Score;
    int score;
    public Text Coins;
    int coins;
    public Text Cases;
    int cases;
    public Text HighScore;

    public Registration RegistrationPanel;
    public GameObject RegistrationFinishedPanel;
    public GameObject LoginPanel;
    public LoginCanvas LoginC;
    public GameObject MainMenu;
    public GamePanel GamePanel;
    public StartBonuses SBonuses;
    public Friends FriendsPanel;
    public GameObject TasksPanel;

    public Notification FriendsNotification;
    public Notification TradesNotification;
    public Notification DuelsNotification;
    public Notification CasesNotification;

    public BuyInfo BuyInfoPanel;
    public BuyInfo UpgradeInfoPanel;

    public ShopPanel Shop;
    public DuelsPanel Duels;

    public Text GameOverDistance;
    public Text GameOverIceCream;
    public Text GameOverCases;

    public SuitsPanel Suits;
    public GameObject PausePanel;

    public TradePanel MyOffer;
    public TradePanel OpponentsOffer;

    public TradeOfferModel offer;

    public ShowTradesPanel TradePanel;

    public AchievementPanel AchievementsPanel;
    public AchievementInfo AchievementInfo;

    public TournamentPanel Tournament;
    public StatisticsPanel Stats;

    public Sprite Avatar;
    public Image SettingsAvatar;
    public Image GameAvatar;
    public LocalizedText SettingsRegion;

    public Text Test;
    public ToggleGroup BonusesToggleGroup;
    public AudioMixer MainMixer;

    public Button DailyBonusBtn;
    public Button WeeklyTasksBtn;
    public Button TournamentBtn;
    public Slider VolumeSlider;
    public ChangeState ToggleMute;
    
    public AdsPanel ADSPanel;

    public static float CurrentVolume = 0;
    public Dropdown LanguageDropdown;

    public SuitsPanel CasesPanel;

    public List<Text> IceCreamPanels;

    public DailyBonusPanel DailyBonus;
    public WeeklyTasksPanel WeeklyTasks;

    public ForgotPasswordPanel ForgotPassword;

    public GameObject Loading;
    public GameObject Countdown;

    public AchievementPopUp PopUpPanel;

    public ContinuePanel ContinueForMoney;

    public GameObject LoadingPanel;
    public GameObject ErrorWindow;

    private static Stack<BackButton> _backButtons = new Stack<BackButton>();

    public void CloseLoading()
    {
        LoadingPanel.GetComponent<Animator>().SetBool("Loaded", true);
        StartCoroutine(LoadingClosing());
    }

    IEnumerator LoadingClosing()
    {
        yield return new WaitForSeconds(1);
        LoadingPanel.SetActive(false);
    }

    private void Start()
    {
        CurrentVolume = PlayerPrefs.GetFloat("GameVolume");
        SetVolume(CurrentVolume);
        VolumeSlider.value = CurrentVolume;
        Application.runInBackground = true;
        StartCoroutine(WaitErrorCheck());
    }
    
    public void SetAvatar(Sprite sprt)
    {
        Avatar = sprt;
        SettingsAvatar.sprite = sprt;
        //SettingsAvatar.SetNativeSize();
        GameAvatar.sprite = sprt;
        //GameAvatar.SetNativeSize();
    }


    public void SetScore(int points)
    {
        if(!string.IsNullOrEmpty(LoginManager.Instance.User.Nickname) && points > LoginManager.Instance.User.HighScore)
        {
            HighScore.text = points + LocalizationManager.GetLocalizedValue("meter");
        }
        score = points;
        Score.text = points + LocalizationManager.GetLocalizedValue("meter");
    }
    public void AddCoin()
    {
        coins++;
        Coins.text = string.Format("{0}", coins);
    }

    public void AddCase()
    {
        cases++;
        Cases.text = string.Format("{0}", cases);
    }

    public static void ShowLoading(bool toShow)
    {
        Instance.Loading.SetActive(toShow);
    }

    public void SetGameOverPanel()
    {
        GameOverCases.text = string.Format("x{0}",GameController.Instance.CurrentBoxes);
        GameOverDistance.text = score + LocalizationManager.GetLocalizedValue("meter");
        GameOverIceCream.text = string.Format("{0}", coins);
        //GameOverAllIceCream.text = 
        coins = 0;
        GamePanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;
        offer = new TradeOfferModel();
    }

    public void StartRun()
    {
        MainMenu.SetActive(false);
        CameraFollow.Instance.ChangeCamera();
        PlayerController.Instance.PlayerAnimator.SetTrigger("Change");
    }

    public void SetNotifications(UserInfoModel info)
    {
        FriendsNotification.SetCount(info.IncomingFriendships);
        TradesNotification.SetCount(info.IncomingTrades);
        DuelsNotification.SetCount(info.IncomingDuels);
        CasesNotification.SetCount(info.Cases);
    }

    public void OpenShopPanel()
    {
        Shop.Open();
    }
    
    public void SetTradeItems(TradeItemsModel items)
    {
        if(offer.OfferItem !=null)
        {
            OpponentsOffer.SetContent(items);
        }
        else
        {
            MyOffer.SetContent(items);
        }
    }
    public void ContinueTrade(InventoryItem myOffer)
    {
        offer.UserId = FriendsPanel.TraderFriend.Id;
        offer.Nickname = FriendsPanel.TraderFriend.Nickname;
        offer.OfferItem = myOffer;
        TradeManager.Instance.GetTradeItemsAsync(FriendsPanel.TraderFriend.Id);
    }

    public void SendOffer(InventoryItem request)
    {
        offer.RequestItem = request;
        TradeManager.Instance.OfferTradeAsync(offer);
    }

    public void TradeOffered()
    {
        OpponentsOffer.gameObject.SetActive(false);
        offer.OfferItem = null;
    }

    public void ClearOffer()
    {
        offer = new TradeOfferModel();
    }

    public void GetTrades()
    {
        ShowLoading(true);
        TradeManager.Instance.GetTradeOffersAsync();
    }

    public void SetVolume(float volume)
    {
        MainMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("GameVolume", volume);        

        if (volume > -80)
        {
            ToggleMute.SwitchOff();
        }
        else
        {
            ToggleMute.SwitchOn();
        }
    }

    public void Mute(bool mute)
    {
        if (mute)
        {
            CurrentVolume = VolumeSlider.value;
            VolumeSlider.value = -80;
        }
        else
        {
            VolumeSlider.value = CurrentVolume;
        }
    }

    public void SetAllIceCreams(int amount)
    {
        foreach (Text item in IceCreamPanels)
        {
            item.text = amount.ToString();
        }
    }

    public static void AddButton(BackButton btn)
    {
        _backButtons.Push(btn);
    }

    public static void PressBack()
    {
        if (_backButtons.Count > 0)
        {
            _backButtons.Peek().PressButton();
        }
    }

    public static void RemoveButton()
    {
        _backButtons.Pop();
    }

    IEnumerator WaitErrorCheck()
    {
        yield return new WaitUntil(() => ErrorChecked == true);

        if (Error)
        {
            ErrorWindow.SetActive(true);
        }
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}


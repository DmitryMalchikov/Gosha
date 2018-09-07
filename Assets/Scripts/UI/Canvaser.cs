using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Canvaser : Singleton<Canvaser>
{
    public static Queue<Exception> Errors = new Queue<Exception>();
    public AchievementInfo AchievementInfo;
    public AchievementPanel AchievementsPanel;
    public AdsPanel ADSPanel;
    public Sprite Avatar;
    public ToggleGroup BonusesToggleGroup;
    public BuyInfo BuyInfoPanel;
    public Text Cases;
    public Text CasesCount;
    public Notification CasesNotification;
    public SuitsPanel CasesPanel;
    public Text Coins;
    public ContinuePanel ContinueForMoney;
    public GameObject Countdown;
    public DailyBonusPanel DailyBonus;
    public bool DoubleIcecreamClicked;
    public GameObject DoubleScoreButton;
    public DuelsPanel Duels;
    public Notification DuelsNotification;
    public AudioMixer EffectsMixer;
    public Slider EffectsVolumeSlider;
    public GameObject ErrorWindow;
    public ForgotPasswordPanel ForgotPassword;
    public Notification FriendsNotification;
    public Friends FriendsPanel;
    public Text GameOverCases;
    public Text GameOverDistance;
    public IceCreamChanger GameOverIceCream;
    public GameObject GameOverPanel;
    public GamePanel GamePanel;
    public List<IceCreamChanger> IceCreamPanels;
    public Dropdown LanguageDropdown;
    public GameObject LoadingPanel;
    public CanvasGroup LoadingPanelCanvasGroup;
    public GameObject LoginPanel;
    public GameObject LoginWarning;
    public MainMenuPanel MainMenu;
    public Transform MainPanel;
    public AudioMixer MusicMixer;
    public Slider MusicVolumeSlider;
    public GameObject MyBonusesPanel;
    public TradePanel MyOffer;
    public Text Nickname;
    public List<NotificationPanel> NotificationsPanels;
    public TradeOfferModel offer;
    public TradePanel OpponentsOffer;
    public GameObject PausePanel;
    public AchievementPopUp PopUpPanel;
    public GameObject RegistrationFinishedPanel;
    public Registration RegistrationPanel;
    public StartBonuses SBonuses;
    public Text Score;
    public Image SettingsAvatar;
    public Image SettingsAvatarBorder;
    public Text SettingsNickname;
    public LocalizedText SettingsRegion;
    public LocalizedText SettingsRegionWord;
    public ShopPanel Shop;
    public StatisticsPanel Stats;
    public SuitsPanel Suits;
    public GameObject TasksPanel;
    public TournamentPanel Tournament;
    public Button TournamentBtn;
    public ShowTradesPanel TradePanel;
    public Notification TradesNotification;
    public BuyInfo UpgradeInfoPanel;
    public WeeklyTasksPanel WeeklyTasks;
    public Button WeeklyTasksBtn;
    private static Stack<BackButton> _backButtons = new Stack<BackButton>();
    int cases;
    int coins;
    int score;

    public static void AddButton(BackButton btn)
    {
        _backButtons.Push(btn);
    }

    public void OpenMyBonusesPanel(bool open)
    {
        MyBonusesPanel.SetActive(open);
    }

    public bool IsLoggedIn()
    {
        if (LoginManager.LocalUser)
        {
            LoginWarning.SetActive(true);
        }
        return !LoginManager.LocalUser;
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

    public string AddBrackets(string input)
    {
        int index = input.LastIndexOf(" ");
        input = input.Insert(index + 1, "(");
        input += ")";
        input = input.Replace(" Card", "");
        input = input.Replace(" Suit", "");
        return input;
    }

    public void AddCase()
    {
        cases++;
        Cases.text = cases.ToString();
    }

    public void AddCoin()
    {
        coins++;
        Coins.text = coins.ToString();
    }

    public void ClearOffer()
    {
        offer = new TradeOfferModel();
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void CloseLoading()
    {
        //LoadingPanel.GetComponent<Animator>().SetBool("Loaded", true);
        StartCoroutine(LoadingClosing());
    }

    public void ContinueTrade(InventoryItem myOffer)
    {
        offer.UserId = FriendsPanel.TraderFriend.Id;
        offer.Nickname = FriendsPanel.TraderFriend.Nickname;
        offer.OfferItem = myOffer;
        TradeManager.Instance.GetTradeItemsAsync(FriendsPanel.TraderFriend.Id);
    }

    public void DoubleScore()
    {
        DoubleIcecreamClicked = true;
        DoubleScoreButton.SetActive(false);
        ADSPanel.transform.SetAsLastSibling();
        //LoadingPanel.transform.SetAsLastSibling();
        ADSPanel.DoubleScore();
        coins = GameOverIceCream.CurrentCount;
        ScoreManager.Instance.SubmitScoreAsync(0, coins, 0);
        GameOverIceCream.ChangeIceCream(coins * 2);
        coins = 0;
    }

    public void GetTrades()
    {
        TradeManager.Instance.GetTradeOffersAsync();
    }

    public void OpenNotificationPanel(NotificationType notType, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < NotificationsPanels.Count; i++)
            {
                if (!NotificationsPanels[i].gameObject.activeInHierarchy)
                {
                    NotificationsPanels[i].OpenNotification(notType, count);
                    break;
                }
            }
        }
    }
    public void OpenShopPanel()
    {
        Shop.Open();
    }

    public void SendOffer(InventoryItem request)
    {
        offer.RequestItem = request;
        TradeManager.Instance.OfferTradeAsync(offer);
    }

    public void SetAllIceCreams(int amount)
    {
        foreach (IceCreamChanger item in IceCreamPanels)
        {
            item.ChangeIceCream(amount);
        }
    }

    public void SetAvatar(Sprite sprt)
    {
        Avatar = sprt;
        SettingsAvatar.sprite = sprt;
        //SettingsAvatar.SetNativeSize();
        //GameAvatar.sprite = sprt;
        //GameAvatar.SetNativeSize();
    }

    public void SetEffectsVolume(float volume)
    {
        EffectsMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("effects_volume_gosha", volume);
    }

    public void SetGameOverPanel()
    {
        DoubleScoreButton.SetActive(true);
        GameOverCases.text = string.Format("x{0}", GameController.Instance.CurrentBoxes);
        GameOverDistance.text = score + LocalizationManager.GetLocalizedValue("meter");
        GameOverIceCream.SetIceCream(coins);
        //GameOverAllIceCream.text = 
        coins = 0;
        GamePanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(true);
        CasesPanel.CaseCamera.SetActive(true);
    }

    public void SetMusicVolume(float volume)
    {
        MusicMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("music_volume_gosha", volume);
    }

    public void SetNotifications(UserInfoModel info)
    {
        FriendsNotification.SetCount(info.IncomingFriendships);
        TradesNotification.SetCount(info.IncomingTrades);
        DuelsNotification.SetCount(info.IncomingDuels);
        CasesNotification.SetCount(info.Cases);
        CasesCount.text = ": " + info.Cases;
    }

    public void SetScore(int points)
    {
        //        if(!string.IsNullOrEmpty(LoginManager.Instance.User.Nickname) && points > LoginManager.Instance.User.HighScore)
        //        {
        //            HighScore.text = points + LocalizationManager.GetLocalizedValue("meter");
        //        }
        score = points;
        Score.text = points + LocalizationManager.GetLocalizedValue("meter");
    }

    public void SetTradeItems(TradeItemsModel items)
    {
        if (offer.OfferItem != null)
        {
            OpponentsOffer.SetContent(items);
        }
        else
        {
            MyOffer.SetContent(items);
        }
    }

    public void StartRun()
    {
        MainMenu.SetActive(false);
        CameraFollow.Instance.ChangeCamera();
        PlayerController.Instance.PlayerAnimator.SetTrigger("Change");
    }

    public void TradeOffered()
    {
        OpponentsOffer.gameObject.SetActive(false);
        offer.OfferItem = null;
    }

    public void TurnOffGameOverPanel()
    {
        if (!DoubleIcecreamClicked)
        {
            GameOverPanel.SetActive(false);
            CasesPanel.CaseCamera.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    IEnumerator ErrorCheck()
    {
        while (true)
        {
            yield return null;

            if (Errors.Count > 0)
            {
                ErrorWindow.SetActive(true);
                Errors.Clear();
            }
        }
    }

    IEnumerator LoadingClosing()
    {
        float opacity = 1;

        while (opacity > 0)
        {
            yield return null;
            opacity -= Time.deltaTime;
            LoadingPanelCanvasGroup.alpha = opacity;
        }

        LoadingPanel.SetActive(false);
    }

    private void Start()
    {
        var musicVolume = PlayerPrefs.GetFloat("music_volume_gosha");
        var effectsVolume = PlayerPrefs.GetFloat("effects_volume_gosha");

        MusicMixer.SetFloat("MainVolume", musicVolume);
        EffectsMixer.SetFloat("MainVolume", effectsVolume);

        MusicVolumeSlider.value = musicVolume;
        EffectsVolumeSlider.value = effectsVolume;

        Application.runInBackground = true;
        offer = new TradeOfferModel();
        StartCoroutine(ErrorCheck());
    }
}


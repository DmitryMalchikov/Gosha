using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Canvaser : MonoBehaviour
{
    public static Dictionary<string, List<GameObject>> LoadingPanels = new Dictionary<string, List<GameObject>>();

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
    public IceCreamChanger GameOverIceCream;
    public Text GameOverCases;
	public GameObject DoubleScoreButton;

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

    public Text Nickname;
    public ToggleGroup BonusesToggleGroup;
    public AudioMixer MusicMixer;
	public AudioMixer EffectsMixer;

    public Button DailyBonusBtn;
    public Button WeeklyTasksBtn;
    public Button TournamentBtn;

    public Slider MusicVolumeSlider;
	public Slider EffectsVolumeSlider;
    
    public AdsPanel ADSPanel;

    public Dropdown LanguageDropdown;

    public SuitsPanel CasesPanel;

    public List<IceCreamChanger> IceCreamPanels;

    public DailyBonusPanel DailyBonus;
    public WeeklyTasksPanel WeeklyTasks;

    public ForgotPasswordPanel ForgotPassword;
    public GameObject Countdown;

    public AchievementPopUp PopUpPanel;

    public ContinuePanel ContinueForMoney;

    public GameObject LoadingPanel;
    public GameObject ErrorWindow;

    public Text CasesCount;

    private static Stack<BackButton> _backButtons = new Stack<BackButton>();
    
    public bool DoubleIcecreamClicked;

    public List<NotificationPanel> NotificationsPanels;

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
		var musicVolume = PlayerPrefs.GetFloat("music_volume_gosha");
		var effectsVolume = PlayerPrefs.GetFloat("effects_volume_gosha");

        MusicMixer.SetFloat("MainVolume", musicVolume);
        EffectsMixer.SetFloat("MainVolume", effectsVolume);

        MusicVolumeSlider.value = musicVolume;
		EffectsVolumeSlider.value = effectsVolume;

        Application.runInBackground = true;
        StartCoroutine(WaitErrorCheck());
    }
    
    public void SetAvatar(Sprite sprt)
    {
        Avatar = sprt;
        SettingsAvatar.sprite = sprt;
        //SettingsAvatar.SetNativeSize();
        //GameAvatar.sprite = sprt;
        //GameAvatar.SetNativeSize();
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
    public void AddCoin()
    {
        coins++;
		Coins.text = coins.ToString();
    }

    public void AddCase()
    {
        cases++;
		Cases.text = cases.ToString();
    }

    public static void AddLoadingPanel(List<GameObject> panel, string url)
    {
        if (LoadingPanels.ContainsKey(url))
        {
            LoadingPanels[url].AddRange(panel);
        }
        else
        {
            LoadingPanels[url] = panel;
        }
    }

    public static void ShowLoading(bool toShow, string url)
    {
        if (!LoadingPanels.ContainsKey(url)) return;

        for (int i = 0; i < LoadingPanels[url].Count; i++)
        {
            LoadingPanels[url][i].SetActive(toShow);
        }

        if (!toShow)
        {
            LoadingPanels.Remove(url);
        }
    }

    public static void ShowLoading(bool toShow, string url, List<GameObject> panels)
    {
        if (!LoadingPanels.ContainsKey(url)) return;

        for (int i = 0; i < LoadingPanels[url].Count; i++)
        {
            if (panels.Contains(LoadingPanels[url][i]))
            {
                LoadingPanels[url][i].SetActive(toShow);
                LoadingPanels[url].RemoveAt(i);
                i--;
            }
        }

        if (!toShow)
        {
            if (LoadingPanels[url].Count == 0)
            {
                LoadingPanels.Remove(url);
            }
        }
    }

    public void SetGameOverPanel()
    {
        DoubleScoreButton.SetActive(true);
        GameOverCases.text = string.Format("x{0}",GameController.Instance.CurrentBoxes);
        GameOverDistance.text = score + LocalizationManager.GetLocalizedValue("meter");
        GameOverIceCream.SetIceCream(coins);
        //GameOverAllIceCream.text = 
        coins = 0;
        GamePanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(true);
        CasesPanel.CaseCamera.SetActive(true);
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

	private void Awake()
    {
        Instance = this;
        offer = new TradeOfferModel();
    }

    public void StartRun()
    {
        //if (!SomePanelOpened)
        //{
            MainMenu.SetActive(false);
            CameraFollow.Instance.ChangeCamera();
            PlayerController.Instance.PlayerAnimator.SetTrigger("Change");
        //}
    }

    public void SetNotifications(UserInfoModel info)
    {
        FriendsNotification.SetCount(info.IncomingFriendships);
        TradesNotification.SetCount(info.IncomingTrades);
        DuelsNotification.SetCount(info.IncomingDuels);
        CasesNotification.SetCount(info.Cases);
        CasesCount.text = ": " + info.Cases;
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
        TradeManager.Instance.GetTradeOffersAsync();
    }

    public void SetEffectsVolume(float volume)
    {
		EffectsMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("effects_volume_gosha", volume);        
    }

	public void SetMusicVolume(float volume)
	{
		MusicMixer.SetFloat("MainVolume", volume);
		PlayerPrefs.SetFloat("music_volume_gosha", volume);        
	}
		

    public void SetAllIceCreams(int amount)
    {
        foreach (IceCreamChanger item in IceCreamPanels)
        {
            item.ChangeIceCream(amount);
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

    public string AddBrackets(string input)
    {
        int index = input.LastIndexOf(" ");
        input = input.Insert(index + 1, "(");
        input += ")";
        input = input.Replace(" Card", "");
        return input;
    }
}


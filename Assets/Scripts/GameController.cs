using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VacuumShaders.CurvedWorld;

public struct BonusesOn
{
    public string name;
    public bool isOn;


    public BonusesOn(string s)
    {
        name = s;
        isOn = false;
    }
}

public class GameController : MonoBehaviour
{

    public static GameController Instance { get; private set; }

    public static WaitForEndOfFrame Frame { get; private set; }
    //public int ScoresSpeed = 10;
    public int StartSpeed = -6;
    public int MaxSpeed = -35;
	public float IncreaseTimeStep;
	public float IncreaseValue;

    public float ShieldTime;
    public float MagnetTime;
    public float DecelerationTime;
    public float RocketTime;

    public float ShieldTimeLeft;
    public float MagnetTimeLeft;
    public float DecelerationTimeLeft;
    public float RocketTimeLeft;

    public float RocketDistance
    {
        get
        {
            return Mathf.Abs(RocketTime * Speed.z);
        }
    }

    public float CurrentPoints = 0;
    public int CurrentCoins = 0;
    public int CurrentBoxes = 0;
    public Vector3 Speed = Vector3.back * 10;
    public float CoinSpeed = 10;
    public float RocketPower = 600;
    public float RocketHeight = 5;

    public float ScoreSpeed;

    public bool Deceleration = false;
    public bool Rocket = false;
    public bool Shield = false;
    public bool Magnet = false;

    public bool NormalSpeed = true;
    public bool BlockMoving = false;

    Vector3 CurrentSpeed;

    public Transform mainCamera;
    public Vector3 RocketSwitchSpeed;

    public float returnTime = 1f;

    public int BonusChance;
    public int BoxChance = 1;

    public RocketCoins rocketCoins;

    public List<BonusesOn> ActiveBonuses;

    public bool Started;

    float points;

    public bool InDuel;
    public int DuelID;

    public float PauseSpeed;
    public Bonus CurrentBonus;

    public bool Continued = false;
	public string CancelBtn = "Cancel";
	public TextureAnimator MainRoad;

    CurvedWorld_Controller curvController;

    void SetActiveBonuses()
    {
        ActiveBonuses = new List<BonusesOn>
        {
            new BonusesOn("Magnet"),
            new BonusesOn("Shield"),
            new BonusesOn("Rocket"),
            new BonusesOn("Decelerator")
        };
    }

    public void ResetScores()
    {
		Rocket = false;
		Deceleration = false;
		Shield = false;
        //+ LocalizationManager.GetLocalizedValue("meter");
        //Canvaser.Instance.Coins.text = "0";
        //Canvaser.Instance.HighScore.text = LoginManager.Instance.User.HighScore + LocalizationManager.GetLocalizedValue("meter");
        Time.timeScale = 1;
        Started = true;
        Continued = false;
        Speed.z = StartSpeed;
        CurrentCoins = 0;
        CurrentBoxes = 0;
        CurrentPoints = 0;
        CurrentSpeed = Speed;
        PlayerController.Instance.animator.SetBool(PlayerController.StartedHash, true);
        StartCoroutine(IncreaseSpeed());
        StartCoroutine(ChangeDirection());
        StartCoroutine(GameStarted());
    }

    public void FinishGame()
    {
        Time.timeScale = 0;

        if (!Continued)
        {
            Canvaser.Instance.ContinueForMoney.OpenContinuePanel();
        }
        else
        {
            SuperFinish();
        }
    }

    public void SuperFinish()
    {
        Canvaser.Instance.SetGameOverPanel();
		IceCreamRotator.SetRotator (false);
        Speed.z = 0;
        Started = false;
        CameraFollow.Instance.ChangeCamera();
        PlayerController.Instance.PlayerAnimator.SetTrigger("Change");
        PlayerController.Instance.animator.SetBool(PlayerController.StartedHash, false);
        Time.timeScale = 1;
		AchievementsManager.Instance.CheckAchievements(TasksTypes.Loose);
        ScoreManager.Instance.SubmitScoreAsync((int)CurrentPoints, CurrentCoins, CurrentBoxes);
        if (InDuel)
        {
            DuelManager.Instance.SubmitDuelResultAsync(DuelID, (int)CurrentPoints);
            InDuel = false;
        }

        Canvaser.Instance.PausePanel.SetActive(false);
        Canvaser.Instance.GamePanel.gameObject.SetActive(false);
		Canvaser.Instance.Coins.text = "0";
		Canvaser.Instance.Score.text = "0";

        AchievementsManager.Instance.SubmitAllAchievements(true);
        TasksManager.Instance.SubmitAllTasks(true);
        StopAllCoroutines();
        PlayerController.Instance.ResetSas();
        Collector.Instance.ResetSas();
        Canvaser.Instance.GamePanel.TurdOffBonuses();
        CoinGenerator.Instance.TurnOffCoins();
        MapGenerator.Instance.StartGeneration();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && Started)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Canvaser.Instance.PausePanel.SetActive(true);
    }

    public void ContinueGame()
    {
        Started = true;
        Canvaser.Instance.PausePanel.SetActive(false);
        Canvaser.Instance.Countdown.SetActive(true);
    }

    public void ContinueGameForMoney()
    {
        Continued = true;
        Started = true;
        PlayerController.Instance.animator.SetBool(PlayerController.StartedHash, true);
		PlayerController.Instance.animator.SetTrigger("Reset");
		PlayerController.Instance.animator.transform.rotation = new Quaternion ();
		PlayerController.Instance.transform.position += Vector3.right * (PlayerController.Instance.CurrentX - PlayerController.Instance.transform.position.x);
        Canvaser.Instance.Countdown.SetActive(true);
		PlayerController.Instance.RemoveObstcles();
		CameraFollow.Instance.offset.z = CameraFollow.Instance.ZOffset;
    }

    public void LoadBonusesTime(List<BonusUpgrade> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            switch (items[i].BonusName)
            {
                case "ShieldUpgrade":
                    ShieldTime = items[i].BonusTime;
                    break;
                case "MagnetUpgrade":
                    MagnetTime = items[i].BonusTime;
                    break;
                case "RocketUpgrade":
                    RocketTime = items[i].BonusTime;
                    break;
                case "FreezeUpgrade":
                    DecelerationTime = items[i].BonusTime;
                    break;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        Frame = new WaitForEndOfFrame();
    }

    private void Start()
    {
        SetActiveBonuses();
        curvController = FindObjectOfType<CurvedWorld_Controller>();
    }

    private void Update()
    {
		if (Input.GetButtonDown(CancelBtn))
        {
            Canvaser.PressBack();
        }
		if (Input.GetKeyDown (KeyCode.D)) {
			ApplyDeceleration ();
		}
    }

	bool setRun = false;

    IEnumerator GameStarted()
    {
        while (Started)
        {
            CurrentPoints -= Speed.z * ScoreSpeed * Time.deltaTime;

            points = Mathf.Round(CurrentPoints);
            Canvaser.Instance.SetScore((int)points);
			if (points % 10 == 0 && points != 0) {
				if (setRun) {
					AchievementsManager.Instance.CheckAchievements (TasksTypes.Run, 10);
					TasksManager.Instance.CheckTasks (TasksTypes.Run, 10);
					setRun = false;
				}
			} else {
				setRun = true;
			}
            if (Input.GetKeyDown(KeyCode.R))
            {
                ApplyRocket();
            }
			if (Input.GetKeyDown (KeyCode.M)) {
				Collector.Instance.UseMagnet ();
			}
			if (Input.GetKeyDown (KeyCode.S)) {
				PlayerController.Instance.ApplyShield ();
			}

            yield return null;
        }
    }

    public void AddCoin()
    {
        Canvaser.Instance.AddCoin();
        CurrentCoins++;
		PlayerController.PickIceCream ();
		AchievementsManager.Instance.CheckAchievements(TasksTypes.CollectIceCream);
    }

    public void AddBox()
    {
        CurrentBoxes++;
    }

    public void ApplyRocket()
    {
        RocketTimeLeft = RocketTime;

        if (!Rocket)
        {
            StartCoroutine(UseRocket());
        }
    }

    IEnumerator UseRocket()
    {
		PlayerController.TurnOnEffect (EffectType.Rocket);
        Rocket = true;
        BlockMoving = true;
        StartCoroutine(CoinGenerator.Instance.StartGeneration());

        PlayerController.Instance.rb.useGravity = false;
        PlayerController.Instance.rb.velocity += Vector3.up * (RocketPower - PlayerController.Instance.rb.velocity.y);
        //PlayerController.Instance.rb.AddForce(Vector3.up * RocketPower, ForceMode.Acceleration);
        PlayerController.Instance.col.enabled = false;
		PlayerController.Instance.Collisions.Clear();
        PlayerController.Instance.OnGround = false;
        Canvaser.Instance.GamePanel.Rocket.gameObject.SetActive(true);
        PlayerController.Instance.animator.SetBool(PlayerController.RocketHash, true);
        PlayerController.Instance.animator.SetTrigger("RocketTrigger");

        yield return new WaitUntil(() =>
        {
            RocketTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Rocket.SetTimer(RocketTimeLeft);
            return PlayerController.Instance.transform.position.y >= RocketHeight;
        });

		PlayerController.Instance.transform.position = new Vector3 (PlayerController.Instance.transform.position.x, RocketHeight, PlayerController.Instance.transform.position.z);
        PlayerController.Instance.col.enabled = true;
        PlayerController.Instance.rb.velocity = Vector3.zero;
		PlayerController.Instance.rb.constraints = RigidbodyConstraints.FreezeAll;
        BlockMoving = false;


        while (RocketTimeLeft > 0)
        {
            RocketTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Rocket.SetTimer(RocketTimeLeft);
            yield return Frame;
        }

        Rocket = false;
		PlayerController.TurnOffEffect (EffectType.Rocket);

        PlayerController.Instance.animator.SetBool(PlayerController.RocketHash, false);
        Canvaser.Instance.GamePanel.Rocket.gameObject.SetActive(false);
        PlayerController.Instance.rb.useGravity = true;
		PlayerController.Instance.rb.constraints = PlayerController.FreezeExceptJump;
    }

    public void UseBonus()
    {
        if (CurrentBonus == null || CurrentBonus.Amount < 1) return;

        switch (CurrentBonus.Name)
        {
            case "Shield":
                PlayerController.Instance.ApplyShield();
                break;
            case "Magnet":
                Collector.Instance.UseMagnet();
                break;
            case "Freeze":
                ApplyDeceleration();
                break;
        }

        CurrentBonus.Amount -= 1;
        ScoreManager.Instance.UseBonusAsync(CurrentBonus.Id);
    }

    public void ApplyDeceleration()
    {
        DecelerationTimeLeft = DecelerationTime;

        if (!Deceleration)
        {
            if (NormalSpeed)
            {
                CurrentSpeed = Speed;
                Speed = Speed * 0.7f;
            }
            else
            {
                Speed = CurrentSpeed * 0.7f;
                NormalSpeed = true;
            }

            StartCoroutine(ReturnSpeed());
        }
    }

    IEnumerator ReturnSpeed()
    {
        Deceleration = true;

		PlayerController.TurnOnEffect (EffectType.Freeze);

        Canvaser.Instance.GamePanel.Decelerator.gameObject.SetActive(true);
        while (DecelerationTimeLeft > 0)
        {
            yield return Frame;
            DecelerationTimeLeft -= Time.deltaTime;

            Canvaser.Instance.GamePanel.Decelerator.SetTimer(DecelerationTimeLeft);
        }

        Canvaser.Instance.GamePanel.Decelerator.gameObject.SetActive(false);
        Deceleration = false;
        NormalSpeed = false;
		PlayerController.TurnOffEffect (EffectType.Freeze);

        var increaseValue = (CurrentSpeed.z - Speed.z) / (returnTime * 10);

        while (Speed.z > CurrentSpeed.z && !NormalSpeed)
        {
            yield return new WaitForSeconds(0.1f);

            Speed.z += increaseValue;
        }

        NormalSpeed = true;
    }

    IEnumerator IncreaseSpeed()
    {
        while (Speed.z > MaxSpeed)
        {
			yield return new WaitForSeconds(IncreaseTimeStep);
            if (Started)
            {
				if (!Deceleration && !Rocket)
                {
					CurrentSpeed.z -= IncreaseValue;
                    Speed = CurrentSpeed;
                }
            }
        }
    }

    public void SetBonusesTime(List<BonusUpgrade> upgrades)
    {
        if (upgrades.Count > 0)
        {
            RocketTime = upgrades.Find(x => x.BonusName == "Rocket").BonusTime;
            MagnetTime = upgrades.Find(x => x.BonusName == "Magnet").BonusTime;
            DecelerationTime = upgrades.Find(x => x.BonusName == "Freeze").BonusTime;
            ShieldTime = upgrades.Find(x => x.BonusName == "Shield").BonusTime;
        }
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            int rand = Random.Range(10, 21);
            yield return new WaitForSeconds(rand);
            float direction = (rand - 15) / 2;

            while (Mathf.Abs(curvController._V_CW_Bend_Y - direction) > 0.01f && Time.timeScale > 0 && Started)
            {
                yield return null;
                curvController._V_CW_Bend_Y -= Mathf.Sign(curvController._V_CW_Bend_Y - direction) * 0.005f;
            }
        }
    }
}

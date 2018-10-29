using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VacuumShaders.CurvedWorld;

public class GameController : Singleton<GameController>
{
    public static bool Paused = false;
    public static float SpeedMultiplyer = 1;
    public bool BlockMoving = false;
    public int BonusChance;
    public int BoxChance = 1;
    public string CancelBtn = "Cancel";
    public bool CanUseCurrentBonus = true;
    public float CoinSpeed = 10;
    public bool Continued = false;
    public Bonus CurrentBonus;
    public int CurrentBoxes = 0;
    public int CurrentCoins = 0;
    public float CurrentPoints = 0;
    public bool Deceleration = false;
    public float DecelerationTime;
    public float DecelerationTimeLeft;
    public int DuelID;
    public float IncreaseTimeStep;
    public float IncreaseValue;
    public bool InDuel;
    public bool Magnet = false;
    public float MagnetTime;
    public float MagnetTimeLeft;
    public int MaxSpeed = -35;
    public bool NormalSpeed = true;
    public float returnTime = 1f;
    public bool Rocket = false;
    public float RocketHeight = 5;
    public float RocketPower = 600;
    public float RocketTime;
    public float RocketTimeLeft;
    public float RotationCoef;
    public float ScoreSpeed;
    public bool Shield = false;
    public float ShieldTime;
    public float ShieldTimeLeft;
    public Material Skybox;
    public Vector3 Speed = Vector3.back * 10;
    public bool Started;
    public int StartSpeed = -6;
    Vector3 CurrentSpeed;
    CurvedWorld_Controller curvController;
    float points;
    bool setRun = false;
    private float SkyboxRotation = 0f;
    public static string DuelsHash { get; set; }
    public static string FriendsHash { get; set; }
    public static string PersistentDataPath { get; private set; }
    public static string ShopHash { get; set; }
    public static string SuitsHash { get; set; }
    public static string TradesHash { get; set; }
    public float RocketDistance
    {
        get
        {
            return Mathf.Abs(RocketTime * Speed.z);
        }
    }

    public static void SetHash(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
        LoadHashes();
    }

    public static void TurnFreezeOff()
    {
        Instance.Deceleration = false;
        PlayerController.TurnOffEffect(EffectType.Freeze);
        Canvaser.Instance.GamePanel.Decelerator.Activate(false);
    }

    public static void TurnFreezeOn()
    {
        Instance.Deceleration = true;
        AudioManager.PlayFreezeStartEffect();
        PlayerController.TurnOnEffect(EffectType.Freeze);
        Canvaser.Instance.GamePanel.Decelerator.Activate(true);
    }

    public static void TurnOffAllBonuses()
    {
        Collector.Instance.ResetMagnet();
        Canvaser.Instance.GamePanel.TurdOffBonuses();
        TurnRocketOff();
        TurnFreezeOff();
        PlayerController.TurnShieldOff();
        Instance.NormalSpeed = true;
    }

    public static void TurnRocketOff()
    {
        PlayerController.TurnOffEffect(EffectType.Rocket);
        Instance.Rocket = false;
        Instance.BlockMoving = false;
        PlayerController.Instance.rb.useGravity = true;
        PlayerController.Instance.col.enabled = true;
        Canvaser.Instance.GamePanel.Rocket.Activate(false);
        PlayerController.Instance.animator.SetBool(PlayerController.RocketHash, false);
        PlayerController.Instance.animator.ResetTrigger("RocketTrigger");
        PlayerController.Instance.rb.constraints = PlayerController.FreezeExceptJump;
        AudioManager.StopEffectsSound();
    }

    public static void TurnRocketOn()
    {
        PlayerController.Instance.rb.constraints = PlayerController.FreezeExceptJump;
        PlayerController.TurnOnEffect(EffectType.Rocket);
        Instance.Rocket = true;
        Instance.BlockMoving = true;
        PlayerController.Instance.rb.useGravity = false;
        PlayerController.Instance.col.enabled = false;
        PlayerController.Instance.Collisions.Clear();
        PlayerController.Instance.OnGround = false;
        Canvaser.Instance.GamePanel.Rocket.Activate(true);
        PlayerController.Instance.animator.SetBool(PlayerController.RocketHash, true);
        PlayerController.Instance.animator.SetTrigger("RocketTrigger");
    }

    public void AddBox()
    {
        CurrentBoxes++;
    }

    public void AddCoin()
    {
        Canvaser.Instance.AddCoin();
        CurrentCoins++;
        PlayerController.PickIceCream();
        AchievementsManager.Instance.CheckAchievements(TasksTypes.CollectIceCream);
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
            SpeedMultiplyer = Speed.z / StartSpeed;

            StartCoroutine(ReturnSpeed());
        }
    }

    public void ApplyRocket()
    {
        RocketTimeLeft = RocketTime;

        if (!Rocket)
        {
            StartCoroutine(UseRocket());
        }
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
        StartCoroutine(GameStarted());
        Canvaser.Instance.Countdown.SetActive(true);
        CameraFollow.Instance.offset.z = CameraFollow.Instance.ZOffset;
    }

    public void FinishGame()
    {
        Time.timeScale = 0;

        if (!Continued && !InDuel)
        {
            Canvaser.Instance.ContinueForMoney.OpenContinuePanel();
        }
        else
        {
            SuperFinish();
        }
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

    public void ResetBonusesTime()
    {
        ShieldTime = 6;
        MagnetTime = 6;
        RocketTime = 6;
        DecelerationTime = 6;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Paused = true;
        Canvaser.Instance.PausePanel.SetActive(true);
    }

    public void ResetScores()
    {
        Rocket = false;
        Deceleration = false;
        Shield = false;
        Time.timeScale = 1;
        Started = true;
        Continued = false;
        Speed.z = StartSpeed;
        SpeedMultiplyer = 1;
        CurrentCoins = 0;
        CurrentBoxes = 0;
        CurrentPoints = 0;
        CurrentSpeed = Speed;
        PlayerController.Instance.animator.SetBool(PlayerController.StartedHash, true);
        PlayerController.Instance.animator.ResetTrigger("Reset");
        TasksManager.Instance.CheckTasks(TasksTypes.Play);
        StopAllCoroutines();
        StartCoroutine(IncreaseSpeed());
        StartCoroutine(ChangeDirection());
        StartCoroutine(GameStarted());
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

    public void SuperFinish()
    {
        Canvaser.Instance.SetGameOverPanel();
        IceCreamRotator.SetRotator(false);
        Speed.z = 0;
        SpeedMultiplyer = 0;
        Started = false;
        Paused = false;
        CanUseCurrentBonus = true;
        CameraFollow.Instance.ChangeCamera();
        PlayerController.Instance.PlayerAnimator.SetTrigger("Change");
        PlayerController.Instance.animator.SetBool(PlayerController.StartedHash, false);
        Time.timeScale = 1;
        AchievementsManager.Instance.CheckAchievements(TasksTypes.Loose);
        ScoreManager.Instance.SubmitScoreAsync((int)CurrentPoints, CurrentCoins, CurrentBoxes);
        TouchReader.ClearInputs();
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
        TurnOffAllBonuses();
        PlayerController.Instance.ResetSas();
        CoinGenerator.Instance.TurnOffCoins();
        MapGenerator.Instance.StartGeneration();
    }
    public void UseBonus()
    {
        if (CurrentBonus == null || CurrentBonus.Amount < 1) return;

        CanUseCurrentBonus = false;

        switch (CurrentBonus.Name.Name)
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
        if (!LoginManager.LocalUser)
        {
            ScoreManager.Instance.UseBonusAsync(CurrentBonus.Id);
        }
        else
        {
            Canvaser.Instance.SBonuses.SetStartBonuses(LoginManager.User.Bonuses);

            Extensions.SaveJsonDataAsync(DataType.UserInfo, JsonConvert.SerializeObject(LoginManager.User));
        }
    }

    private static void LoadHashes()
    {
        FriendsHash = PlayerPrefs.GetString("FriendsHash");
        DuelsHash = PlayerPrefs.GetString("DuelsHash");
        SuitsHash = PlayerPrefs.GetString("SuitsHash");
        ShopHash = PlayerPrefs.GetString("ShopHash");
        TradesHash = PlayerPrefs.GetString("TradesHash");
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

    IEnumerator GameStarted()
    {
        while (Started)
        {
            CurrentPoints -= Speed.z * ScoreSpeed * Time.deltaTime;

            points = Mathf.Round(CurrentPoints);
            Canvaser.Instance.SetScore((int)points);
            if (points % 10 == 0 && points != 0)
            {
                if (setRun)
                {
                    AchievementsManager.Instance.CheckAchievements(TasksTypes.Run, 10);
                    TasksManager.Instance.CheckTasks(TasksTypes.Run, 10);
                    setRun = false;
                }
            }
            else
            {
                setRun = true;
            }


#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
            {
                ApplyRocket();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Collector.Instance.UseMagnet();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                PlayerController.Instance.ApplyShield();
            }
#endif

            SkyboxRotation = (SkyboxRotation + CurvedWorld_Controller.get._V_CW_Bend_Y * RotationCoef * Time.deltaTime) % 360;
            Skybox.SetFloat("_Rotation", SkyboxRotation);

            yield return null;
        }
    }

    IEnumerator IncreaseSpeed()
    {
        while (Speed.z > MaxSpeed && Started)
        {
            yield return new WaitForSeconds(IncreaseTimeStep);
            if (Started && !Paused)
            {
                if (!Deceleration && !Rocket)
                {
                    CurrentSpeed.z -= IncreaseValue;
                    Speed = CurrentSpeed;
                    SpeedMultiplyer = Speed.z / StartSpeed;
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && Started && Time.timeScale > 0)
        {
            PauseGame();
        }
    }
    IEnumerator ReturnSpeed()
    {
        TurnFreezeOn();

        while (DecelerationTimeLeft > 0 && Deceleration)
        {
            yield return CoroutineManager.Frame;
            DecelerationTimeLeft -= Time.deltaTime;

            Canvaser.Instance.GamePanel.Decelerator.SetTimer(DecelerationTimeLeft);
        }

        if (!Deceleration)
        {
            yield break;
        }

        TurnFreezeOff();
        Canvaser.Instance.GamePanel.DeceleratorCD.OpenCooldownPanel();
        NormalSpeed = false;
        AudioManager.PlayEffectEnd();

        var increaseValue = (CurrentSpeed.z - Speed.z) / (returnTime * 10);

        while (Speed.z > CurrentSpeed.z && !NormalSpeed)
        {
            yield return new WaitForSeconds(0.1f);

            Speed.z += increaseValue;
            SpeedMultiplyer = Speed.z / StartSpeed;
        }

        NormalSpeed = true;
    }

    private void Start()
    {
        curvController = FindObjectOfType<CurvedWorld_Controller>();

        LoadHashes();
        PersistentDataPath = Application.persistentDataPath;
    }
    private void Update()
    {
        if (Input.GetButtonDown(CancelBtn))
        {
            Canvaser.PressBack();
        }

#if UNITY_EDITOR
        if (!Started)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ApplyDeceleration();
        }
#endif
    }
    IEnumerator UseRocket()
    {
        StartCoroutine(CoinGenerator.Instance.StartGeneration());

        TurnRocketOn();
        PlayerController.Instance.rb.velocity += Vector3.up * (RocketPower - PlayerController.Instance.rb.velocity.y);

        yield return new WaitUntil(() =>
        {
            RocketTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Rocket.SetTimer(RocketTimeLeft);
            return PlayerController.Instance.transform.position.y >= RocketHeight || !Rocket;
        });

        if (!Rocket)
        {
            yield break;
        }

        AudioManager.PlayRocketEffect();
        PlayerController.Instance.transform.position = new Vector3(PlayerController.Instance.transform.position.x, RocketHeight, PlayerController.Instance.transform.position.z);
        PlayerController.Instance.col.enabled = true;
        PlayerController.Instance.rb.velocity = Vector3.zero;
        PlayerController.Instance.rb.constraints = RigidbodyConstraints.FreezeAll;
        BlockMoving = false;


        while (RocketTimeLeft > 0 && Rocket)
        {
            RocketTimeLeft -= Time.deltaTime;
            Canvaser.Instance.GamePanel.Rocket.SetTimer(RocketTimeLeft);
            yield return CoroutineManager.Frame;
        }

        if (!Rocket)
        {
            yield break;
        }
        TurnRocketOff();
        AudioManager.PlayEffectEnd();

        Canvaser.Instance.GamePanel.RocketCD.OpenCooldownPanel();
    }
}

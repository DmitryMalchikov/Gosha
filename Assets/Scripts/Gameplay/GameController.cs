using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public static bool Paused { get; private set; }
    public string CancelBtn = "Cancel";
    public bool CanUseCurrentBonus = true;
    public float CoinSpeed = 10;
    public bool Continued = false;
    public IBonus CurrentBonus;
    public int CurrentBoxes = 0;
    public int CurrentCoins = 0;
    public float CurrentPoints = 0;
    public float DecelerationTime;
    public float MagnetTime;
    public float returnTime = 1f;
    public float RocketTime;
    public float ScoreSpeed;
    public float ShieldTime;
    public static bool Started { get; private set; }
    float points;
    bool setRun = false;
    public static string PersistentDataPath { get; private set; }

    public static void OnHit()
    {
        TurnOffAllBonuses();
        Started = false;
    }

    public static void TurnOffAllBonuses()
    {
        Collector.Instance.ResetMagnet();
        Canvaser.Instance.GamePanel.TurdOffBonuses();
        PlayerRocket.TurnRocketOff();
        SpeedController.TurnFreezeOff();
        PlayerShield.ResetShield();
    }

    public void AddBox()
    {
        CurrentBoxes++;
    }

    public void AddCoin()
    {
        Canvaser.Instance.AddCoin();
        CurrentCoins++;
        EffectsManager.PlayIceCreamPicked();
        AchievementsManager.Instance.CheckAchievements(TasksTypes.CollectIceCream);
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
        SpeedController.Continue();
        Canvaser.Instance.Countdown.SetActive(true);
        CameraFollow.ResetOffset();
    }

    public void FinishGame()
    {
        Time.timeScale = 0;

        if (!Continued && !DuelManager.InDuel)
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

    public void ContinueAfterCountdown()
    {
        Time.timeScale = 1;
        Paused = false;
    }

    public void ResetScores()
    {
        Time.timeScale = 1;
        Started = true;
        Continued = false;
        CurrentCoins = 0;
        CurrentBoxes = 0;
        CurrentPoints = 0;
        PlayerAnimator.Started(true);
        TasksManager.Instance.CheckTasks(TasksTypes.Play);
        SpeedController.ResetSpeed();
        RoadBend.Instance.StartBending();
        PlayerShield.ResetShield();
        PlayerRocket.ResetRocket();
        StopAllCoroutines();
        StartCoroutine(GameStarted());
    }

    public void SuperFinish()
    {
        Canvaser.Instance.SetGameOverPanel();
        IceCreamRotator.SetRotator(false);
        SpeedController.Stop();
        Started = false;
        Paused = false;
        CanUseCurrentBonus = true;
        CameraFollow.Instance.ChangeCamera();
        PlayerAnimator.Started(false);
        Time.timeScale = 1;
        AchievementsManager.Instance.CheckAchievements(TasksTypes.Loose);
        ScoreManager.Instance.SubmitScoreAsync((int)CurrentPoints, CurrentCoins, CurrentBoxes);
        TouchReader.ClearInputs();
        DuelManager.Instance.SubmitDuelResultAsync((int)CurrentPoints);
        Canvaser.ResetCanvas();
        AchievementsManager.Instance.SubmitAllAchievements(true);
        TasksManager.Instance.SubmitAllTasks(true);
        TurnOffAllBonuses();
        PlayerController.Instance.ResetSas();
        CoinGenerator.Instance.TurnOffCoins();
        MapGenerator.Instance.StartGeneration();
    }
    public void UseBonus()
    {
        if (CurrentBonus == null || !CurrentBonus.UseBonus()) return;
        CanUseCurrentBonus = false;
    }

    IEnumerator GameStarted()
    {
        while (Started)
        {
            CurrentPoints -= SpeedController.Speed.z * ScoreSpeed * Time.deltaTime;

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
                PlayerRocket.Instance.ApplyRocket();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Collector.Instance.UseMagnet();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                PlayerShield.Instance.ApplyShield();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                SpeedController.Instance.ApplyDeceleration();
            }
#endif
            SkyboxRotator.Rotate();

            yield return null;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && Started && Time.timeScale > 0)
        {
            PauseGame();
        }
    }

    private void Start()
    {
        PersistentDataPath = Application.persistentDataPath;
        Paused = false;
    }
    private void Update()
    {
        if (Input.GetButtonDown(CancelBtn))
        {
            Canvaser.PressBack();
        }
    }
}

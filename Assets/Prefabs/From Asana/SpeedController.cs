using System.Collections;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

public class SpeedController : Singleton<SpeedController>
{
    public static float SpeedMultiplier { get; private set; }
    public static Vector3 Speed
    {
        get
        {
            return Instance._speed;
        }
    }
    private Vector3 _speed;

    public static float StartSpeed
    {
        get
        {
            return Instance._startSpeed;
        }
    }

    [SerializeField]
    private int _startSpeed = -6;
    [SerializeField]
    private float _returnTime = 1f;
    [SerializeField]
    private float _maxSpeed = -35;
    [SerializeField]
    public float _increaseTimeStep;
    [SerializeField]
    public float _increaseValue;

    private float _decelerationTimeLeft = 0;
    private Vector3 _currentSpeed;
    private bool _normalSpeed = true;
    private bool _decelerationInProgress = false;
    private static WaitForSeconds _waitingSpeedIncrease;

    private void Awake()
    {
        _speed = Vector3.back * 8;
        _waitingSpeedIncrease = new WaitForSeconds(_increaseTimeStep);
    }

    public void ApplyDeceleration()
    {
        _decelerationTimeLeft = GameController.Instance.DecelerationTime;

        if (!_decelerationInProgress)
        {
            if (_normalSpeed)
            {
                _currentSpeed = Speed;
                _speed = Speed * 0.7f;
            }
            else
            {
                _speed = _currentSpeed * 0.7f;
                _normalSpeed = true;
            }
            SpeedMultiplier = Speed.z / _startSpeed;

            StartCoroutine(ReturnSpeed());
        }
    }

    IEnumerator ReturnSpeed()
    {
        TurnFreezeOn();

        while (_decelerationTimeLeft > 0 && _decelerationInProgress)
        {
            yield return CoroutineManager.Frame;
            _decelerationTimeLeft -= Time.deltaTime;

            Canvaser.Instance.GamePanel.Decelerator.SetTimer(_decelerationTimeLeft);
        }

        if (!_decelerationInProgress)
        {
            yield break;
        }

        TurnFreezeOff();
        Canvaser.Instance.GamePanel.DeceleratorCD.OpenCooldownPanel();
        _normalSpeed = false;
        AudioManager.PlayEffectEnd();

        var increaseValue = (_currentSpeed.z - Speed.z) / (_returnTime * 10);

        while (Speed.z > _currentSpeed.z && !_normalSpeed)
        {
            yield return new WaitForSeconds(0.1f);

            Instance._speed.z += increaseValue;
            SpeedMultiplier = Speed.z / _startSpeed;
        }

        _normalSpeed = true;
    }

    public static void TurnFreezeOff()
    {
        Instance._decelerationInProgress = false;
        EffectsManager.PlayIceEffect(false);
        Canvaser.Instance.GamePanel.Decelerator.Activate(false);
    }

    public static void TurnFreezeOn()
    {
        Instance._decelerationInProgress = true;
        AudioManager.PlayFreezeStartEffect();
        EffectsManager.PlayIceEffect(true);
        Canvaser.Instance.GamePanel.Decelerator.Activate(true);
    }

    public static void ResetSpeed()
    {
        Instance._speed.z = Instance._startSpeed;
        SpeedMultiplier = 1;
        Instance._decelerationInProgress = false;
        Instance._currentSpeed = Instance._speed;
        RestartCoroutines();
    }

    private static void RestartCoroutines()
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.IncreaseSpeed());
    }

    public static void Stop()
    {
        Instance._speed.z = 0;
        SpeedMultiplier = 0;
    }

    public static void Continue()
    {
        Instance._speed.z = Instance._currentSpeed.z;
        SpeedMultiplier = 1;
        Instance._normalSpeed = true;
        RestartCoroutines();
    }

    IEnumerator IncreaseSpeed()
    {
        while (Speed.z > _maxSpeed && GameController.Started)
        {
            yield return _waitingSpeedIncrease;
            if (GameController.Started && !GameController.Paused)
            {
                if (!_decelerationInProgress && !PlayerRocket.RocketInProgress)
                {
                    _currentSpeed.z -= _increaseValue;
                    _speed = _currentSpeed;
                    SpeedMultiplier = Speed.z / _startSpeed;
                }
            }
        }
    }
}

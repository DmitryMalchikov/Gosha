using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public static CoinGenerator Instance { get; private set; }

    public Transform Generator;

    public float CoinDistance = 0.2f;
    public int MinLine = 5;
    public int MaxLine = 15;
    public int ZDIstance = 150;

    bool _generationStarted = false;
    Coin[] _coins;
    List<Coin> _avaliableCoins;
    float _deltaTime = 0;
    int _currentLine = 0;
    float _playerDistance;
    int _coinsNumber = 0;
    int _totalCoins = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _coins = GetComponentsInChildren<Coin>();
        _avaliableCoins = new List<Coin>(_coins);

        for (int i = 0; i < _avaliableCoins.Count; i++)
        {
            _avaliableCoins[i].gameObject.SetActive(false);
        }
    }

    public void TurnOffCoins()
    {
        for (int i = 0; i < _coins.Length; i++)
        {
            ResetCoin(_coins[i]);
            _coins[i].gameObject.SetActive(false);
        }
       
    }

    public void BeginGeneration(float startTime)
    {
        _generationStarted = true;
        _deltaTime = Mathf.Abs(CoinDistance / GameController.Instance.Speed.z);
        _playerDistance = Mathf.Abs(Generator.position.z - PlayerController.Instance.transform.position.z);        

        float totalTime = GameController.Instance.RocketTime - (Mathf.Abs(_playerDistance / GameController.Instance.Speed.z) + (Time.time - startTime));

        _totalCoins = 0;
        _coinsNumber = (int)(totalTime * GameController.Instance.Speed.z/ CoinDistance);
       StartCoroutine(Generation());
       StartCoroutine(Stop(totalTime));
    }

    public void StopGeneration()
    {
        _generationStarted = false;
    }

    public IEnumerator StartGeneration()
    {
        Generator.position = new Vector3(Generator.position.x, Generator.position.y, PlayerController.Instance.transform.position.z - GameController.Instance.Speed.z * 1.5f);
        float distance = GameController.Instance.RocketDistance + PlayerController.Instance.transform.position.z;

        float startTime = Time.time;

        while (Generator.position.z < ZDIstance && Generator.position.z < distance)
        {
            int coinsNumber = Random.Range(MinLine, MaxLine + 1);            

            for (; coinsNumber >= 0 && Generator.position.z < ZDIstance && Generator.position.z < distance; coinsNumber--)
            {
                yield return NextCoin();
                if (coinsNumber > 0)
                {
                    Generator.position += Vector3.forward * CoinDistance;
                }
                yield return GameController.Frame;
            }

            if (Generator.position.z < ZDIstance && Generator.position.z < distance)
            {
                yield return ChangeLine(true);
            }
        }

        BeginGeneration(startTime);
    }

    IEnumerator ChangeLine(bool move)
    {
        var prevLine = _currentLine;
        sbyte lineDelta = (sbyte)Mathf.Sign(Random.Range(-1, 1));
        _currentLine += lineDelta;

        if (Mathf.Abs(_currentLine) > 1)
        {
            _currentLine = 0;
        }

        if (move)
        {
            Generator.position = new Vector3(((float)(_currentLine + prevLine) / 2) * PlayerController.Instance.Step, Generator.position.y, Generator.position.z + (CoinDistance / 3));
        }
        else
        {            
            Generator.position = new Vector3(((float)(_currentLine + prevLine) / 2) * PlayerController.Instance.Step, Generator.position.y, Generator.position.z);
            yield return new WaitForSeconds(_deltaTime / 3);
        }
        
        yield return NextCoin();

        if (move)
        {
            Generator.position = new Vector3(_currentLine * PlayerController.Instance.Step, Generator.position.y, Generator.position.z + (CoinDistance / 3));
        }
        else
        {
            Generator.position = new Vector3(_currentLine * PlayerController.Instance.Step, Generator.position.y, Generator.position.z);
            yield return new WaitForSeconds(_deltaTime / 3);
        }
    }

    public IEnumerator NextCoin()
    {
        while (_avaliableCoins.Count < 1)
        {
            yield return GameController.Frame;
        }

        var coin = _avaliableCoins[0];

        coin.transform.position = Generator.position;
        coin.gameObject.SetActive(true);
        _totalCoins += 1;

        _avaliableCoins.RemoveAt(0);
    }

    public void ResetCoin(Coin coin)
    {
        _avaliableCoins.Add(coin);
        //coin.transform.position = transform.position;
        //coin.gameObject.SetActive(false);
    }

    IEnumerator Generation()
    {
        while (_generationStarted && _totalCoins < _coinsNumber)
        {
            int coinsNumber = Random.Range(MinLine, MaxLine + 1);            

            for (; coinsNumber >= 0 && _generationStarted; coinsNumber--)
            {
                yield return NextCoin();
                if (coinsNumber > 0)
                {
                    yield return new WaitForSeconds(_deltaTime);
                }
            }

            if (_generationStarted)
            yield return ChangeLine(false);
        }
    }

    IEnumerator Stop(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        _generationStarted = false;
    }

}

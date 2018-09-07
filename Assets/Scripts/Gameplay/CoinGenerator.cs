using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : Singleton<CoinGenerator>
{
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
	private Vector3 PreviousRot = Vector3.zero;

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
        _avaliableCoins = new List<Coin>(_coins);

        for (int i = 0; i < _coins.Length; i++)
        {
            _coins[i].gameObject.SetActive(false);
        }
       
    }

    public void BeginGeneration(float startTime)
    {        
        _deltaTime = Mathf.Abs(CoinDistance / GameController.Instance.Speed.z);
        _playerDistance = Mathf.Abs(Generator.position.z - PlayerController.Instance.transform.position.z);        

        float totalTime = GameController.Instance.RocketTime - (Mathf.Abs(_playerDistance / GameController.Instance.Speed.z) + (Time.time - startTime));


		if (totalTime > 0) {
			_totalCoins = 0;
			_coinsNumber = (int)(-totalTime * GameController.Instance.Speed.z/ CoinDistance);
			_generationStarted = true;
			StartCoroutine (Generation ());
			StartCoroutine (Stop (totalTime));
		}
    }

    public void StopGeneration()
    {
        _generationStarted = false;
    }

	private bool EnoughTime(float endTime){
		return 		(Generator.position.z - PlayerController.Instance.transform.position.z) <=
		-GameController.Instance.Speed.z * (endTime - Time.time);
	}

    public IEnumerator StartGeneration()
    {
        Generator.position = new Vector3(Generator.position.x, Generator.position.y, PlayerController.Instance.transform.position.z - GameController.Instance.Speed.z * 1.5f);

        float startTime = Time.time;
		float endTime = startTime + GameController.Instance.RocketTime;

		while (Generator.position.z < ZDIstance && EnoughTime(endTime))
        {
            int coinsNumber = Random.Range(MinLine, MaxLine + 1);            

			for (; coinsNumber >= 0 && Generator.position.z < ZDIstance && EnoughTime(endTime); coinsNumber--)
            {
                yield return NextCoin();
                if (coinsNumber > 0)
                {
                    Generator.position += Vector3.forward * CoinDistance;
                }
                yield return CoroutineManager.Frame;
            }

			if (Generator.position.z < ZDIstance && EnoughTime(endTime))
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
            yield return CoroutineManager.Frame;
        }

        var coin = _avaliableCoins[0];

        coin.transform.position = Generator.position;
		coin.transform.rotation = Quaternion.Euler(PreviousRot);
        coin.gameObject.SetActive(true);

        _totalCoins += 1;
		PreviousRot -= Vector3.up * IceCreamRotator.Instance.AngleDelta;

        _avaliableCoins.Remove(coin);
    }

    public void ResetCoin(Coin coin)
    {
        _avaliableCoins.Add(coin);
    }

    IEnumerator Generation()
    {
        while (_generationStarted && _totalCoins < _coinsNumber)
        {
            int coinsNumber = Random.Range(MinLine, MaxLine + 1);            

			for (; coinsNumber >= 0 && _generationStarted && _totalCoins < _coinsNumber; coinsNumber--)
            {
                yield return NextCoin();
                if (coinsNumber > 0)
                {
                    yield return new WaitForSeconds(_deltaTime);
                }
            }

			if (_generationStarted && _totalCoins < _coinsNumber)
            yield return ChangeLine(false);
        }
    }

    IEnumerator Stop(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        _generationStarted = false;
    }

}

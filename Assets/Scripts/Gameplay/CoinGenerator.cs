using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class CoinGenerator : Singleton<CoinGenerator>
    {
        public Transform Generator;

        public float CoinDistance = 0.2f;
        public byte MinLine = 5;
        public byte MaxLine = 15;
        public short ZDistance = 150;

        private bool _generationStarted = false;
        private Coin[] _coins;
        private List<Coin> _availableCoins;
        private float _deltaTime = 0;
        private sbyte _currentLine = 0;
        private float _playerDistance;
        private short _coinsNumber = 0;
        private short _totalCoins = 0;
        private Vector3 _previousRot = Vector3.zero;

        private void Start()
        {
            _coins = GetComponentsInChildren<Coin>();
            TurnOffCoins();
        }

        public void TurnOffCoins()
        {
            _availableCoins = new List<Coin>(_coins);

            for (int i = 0; i < _coins.Length; i++)
            {
                _coins[i].gameObject.SetActive(false);
            }
        }

        public void BeginGeneration(float startTime)
        {
            _deltaTime = Mathf.Abs(CoinDistance / SpeedController.Speed.z);
            _playerDistance = Mathf.Abs(Generator.position.z - PlayerController.Instance.transform.position.z);

            float totalTime = GameController.Instance.RocketTime - (Mathf.Abs(_playerDistance / SpeedController.Speed.z) + (Time.time - startTime));

            if (!(totalTime > 0)) return;

            _totalCoins = 0;
            _coinsNumber = (short)(-totalTime * SpeedController.Speed.z / CoinDistance);
            _generationStarted = true;
            StartCoroutine(Generation());
            StartCoroutine(Stop(totalTime));
        }

        private bool EnoughTime(float endTime)
        {
            return (Generator.position.z - PlayerController.Instance.transform.position.z) <=
                   -SpeedController.Speed.z * (endTime - Time.time);
        }

        public IEnumerator StartGeneration()
        {
            Generator.position = new Vector3(Generator.position.x, Generator.position.y, PlayerController.Instance.transform.position.z - SpeedController.Speed.z * 1.5f);

            float startTime = Time.time;
            float endTime = startTime + GameController.Instance.RocketTime;

            while (Generator.position.z < ZDistance && EnoughTime(endTime))
            {
                sbyte coinsNumber = (sbyte)Random.Range(MinLine, MaxLine + 1);

                for (; coinsNumber >= 0 && Generator.position.z < ZDistance && EnoughTime(endTime); coinsNumber--)
                {
                    yield return NextCoin();
                    if (coinsNumber > 0)
                    {
                        Generator.position += Vector3.forward * CoinDistance;
                    }
                    yield return CoroutineManager.Frame;
                }

                if (Generator.position.z < ZDistance && EnoughTime(endTime))
                {
                    yield return ChangeLine(true);
                }
            }

            BeginGeneration(startTime);
        }

        public IEnumerator ChangeLine(bool move)
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

        private IEnumerator NextCoin()
        {
            while (_availableCoins.Count < 1)
            {
                yield return CoroutineManager.Frame;
            }

            var coin = _availableCoins[0];

            coin.transform.position = Generator.position;
            coin.transform.rotation = Quaternion.Euler(_previousRot);
            coin.gameObject.SetActive(true);

            _totalCoins += 1;
            _previousRot -= Vector3.up * IceCreamRotator.Instance.AngleDelta;

            _availableCoins.Remove(coin);
        }

        public void ResetCoin(Coin coin)
        {
            _availableCoins.Add(coin);
        }

        private IEnumerator Generation()
        {
            while (_generationStarted && _totalCoins < _coinsNumber)
            {
                byte coinsNumber = (byte)Random.Range(MinLine, MaxLine + 1);

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

        private IEnumerator Stop(float totalTime)
        {
            yield return new WaitForSeconds(totalTime);
            _generationStarted = false;
        }

    }
}

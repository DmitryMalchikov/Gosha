using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoxPrize : MonoBehaviour
    {
        public GameObject IceCream;
        public GameObject Freeze;
        public GameObject Magnet;
        public GameObject Shield;
        public PrizeCard Card;

        public Animator anim;

        private GameObject _activeObj;

        private string _currentName;

        public void SetPrize(string name)
        {
            _currentName = name;
            switch (name)
            {
                case "IceCream":
                case null:
                    _activeObj = IceCream;
                    break;
                case "Shield":
                    _activeObj = Shield;
                    break;
                case "Freeze":
                    _activeObj = Freeze;
                    break;
                case "Magnet":
                    _activeObj = Magnet;
                    break;
                default:
                    Card.SetCard(name.AddBrackets());
                    _activeObj = Card.gameObject;
                    break;
            }

            _activeObj.SetActive(true);
        }

        public void PrizeOut(bool toOut)
        {
            anim.SetBool("Out", toOut);
            if (toOut && !string.IsNullOrEmpty(_currentName) && _currentName.Contains("Card"))
            {
                AudioManager.PlayCardGet();
            }
        }

        public void TurnOffPrizes()
        {
            if (_activeObj)
                _activeObj.SetActive(false);
        }
    }
}

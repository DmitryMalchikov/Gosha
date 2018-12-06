using System;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public abstract class TimeCheck : MonoBehaviour
    {
        private readonly WaitForSeconds _minute = new WaitForSeconds(60);
        public Text Time;

        public abstract IExpirable Info { get; }

        private void OnEnable()
        {
            StartCoroutine(CheckTime());
        }

        protected IEnumerator CheckTime()
        {
            yield return new WaitUntil(() => Info != null);

            while (true)
            {
                var time = (Info.ExpireDate - DateTime.Now.ToUniversalTime());
                Time.text = time.TimeSpanToLocalizedString();
                yield return _minute;
            }
        }
    }
}

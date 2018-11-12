using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TimeCheck : MonoBehaviour
{
    private WaitForSeconds minute = new WaitForSeconds(60);
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
            yield return minute;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyTask : MonoBehaviour {

    protected WaitForSeconds minute = new WaitForSeconds(60);

    public Text Task;
    public Text TimeLeft;
    public Image Prize;
    PlayerTaskModel info;
    public GameObject box;
    public Transform CasesContent;

    public GameObject IsDone;

    public void SetTask(int number, PlayerTaskModel model)
    {
        info = model;
        Task.text = model.GenerateDescription();
        for (int i = 0; i < model.Reward; i++)
        {
            Instantiate(box, CasesContent);
        }

        if(info.PlayerProgress >= info.ActionsCount)
        {
            IsDone.SetActive(true);
        }

        StartCoroutine(CheckTime());
    }

    protected IEnumerator CheckTime()
    {
        while (true)
        {
            var time = (info.ExpireDate - DateTime.Now);

            TimeLeft.text = time.IntervalToString();

            yield return minute;
        }
    }
}

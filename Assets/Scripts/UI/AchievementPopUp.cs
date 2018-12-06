﻿using System.Collections;
using Assets.Scripts.DTO;
using Assets.Scripts.Managers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class AchievementPopUp : MonoBehaviour {

        public Image Background;
        public Text ActionType;
        public Text Description;
        public Color AchievementColor;
        public Color TaskColor;

        public void ShowAchievement(PlayerTasksAnswer newModel)
        {
            Background.color = AchievementColor;
            ActionType.text = LocalizationManager.GetLocalizedValue("achievementunlocked");
            Description.text = newModel.Name;
            gameObject.SetActive(true);
            StartCoroutine(Show());
        }

        public void ShowTask(string description)
        {
            Background.color = TaskColor;
            ActionType.text = LocalizationManager.GetLocalizedValue("taskdone");
            Description.text = description;
            gameObject.SetActive(true);
            StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(2));
            gameObject.SetActive(false);
        }
    }
}

using Assets.Scripts.DTO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class AchievementObject : MonoBehaviour {

        public Text Title;

        public PlayerAchievementModel Info;

        public void SetAchievement(PlayerAchievementModel model)
        {
            Info = model;
            Title.text = model.Name;
        }

        public void OpenAchievementInfo()
        {
            Canvaser.Instance.AchievementInfo.SetAchievementInfo(Info);
        }
    }
}

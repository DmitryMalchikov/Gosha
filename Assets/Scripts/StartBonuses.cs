using System.Collections.Generic;
using Assets.Scripts.DTO;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class StartBonuses : MonoBehaviour
    {
        public BonusPanel[] BPanels;

        public void SetStartBonuses(List<Bonus> bonuses)
        {
            if (bonuses == null || bonuses.Count == 0)
            {
                for (int i = 0; i < BPanels.Length; i++)
                {
                    BPanels[i].SetInfo(null);
                }
                return;
            }
            for (int i = 0; i < BPanels.Length; i++)
            {
                var bonus = bonuses.Find(p => p.Name.Name == BPanels[i].Name);
                BPanels[i].SetInfo(bonus);
            }
        }

        public void ResetStartBonuses()
        {
            for (int i = 0; i < BPanels.Length; i++)
            {
                BPanels[i].Active.isOn = false;
                var bonus = BPanels[i].Bonus;
                if (bonus == null || bonus.CurrentBonus == null) continue;
                bonus.CurrentBonus.Amount = 0;
                BPanels[i].SetInfo(bonus.CurrentBonus);
            }
        }
    }
}

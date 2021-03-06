﻿using System.Collections.Generic;
using UnityEngine;

public class StartBonuses : MonoBehaviour
{

    public List<BonusPanel> BPanels;

    public void SetStartBonuses(List<Bonus> bonuses)
    {
        for (int i = 0; i < BPanels.Count; i++)
        {
            var bonus = bonuses.Find(p => p.Name.Name == BPanels[i].Name);

            BPanels[i].SetInfo(bonus);
        }
    }

    public void ResetStartBonuses()
    {
        for (int i = 0; i < BPanels.Count; i++)
        {
            BPanels[i].Active.isOn = false;
            var bonus = BPanels[i].Bonus;
            if (bonus != null)
            {
                bonus.Amount = 0;
                BPanels[i].SetInfo(bonus);
            }
        }
    }
}

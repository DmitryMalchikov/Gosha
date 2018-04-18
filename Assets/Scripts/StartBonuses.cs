using System.Collections.Generic;
using UnityEngine;

public class StartBonuses : MonoBehaviour
{

    public List<BonusPanel> BPanels;

    public void SetStartBonuses(List<Bonus> bonuses)
    {
        for (int i = 0; i < BPanels.Count; i++)
        {
            var bonus = bonuses.Find(p => p.Name == BPanels[i].Name);

            BPanels[i].SetInfo(bonus);

        }
    }
}

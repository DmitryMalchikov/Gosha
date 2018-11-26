using System.Collections;
using UnityEngine;

public class BonusCooldown : MonoBehaviour
{
    public Animator anim;
    public float Cooldown;
    public GameObject panel;

    public void Activate(bool toActivate)
    {
        panel.SetActive(toActivate);
    }

    public void SetTimer(float cooldown)
    {
        Cooldown = cooldown;
        anim.speed = 1 / cooldown;
    }

    public void OpenCooldownPanel()
    {
        Activate(true);
        StartCoroutine(ShowCooldown());
    }

    IEnumerator ShowCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        GameController.Instance.CanUseCurrentBonus = true;
        Activate(false);
    }
}

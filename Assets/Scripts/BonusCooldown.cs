using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusCooldown : MonoBehaviour {

    public Animator anim;
    public float Cooldown;

    public void SetTimer(float cooldown)
    {
        Cooldown = cooldown;
        anim.speed = 1 / cooldown;
    }

    public void OpenCooldownPanel()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowCooldown());
    }

    IEnumerator ShowCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        gameObject.SetActive(false);
    }
}

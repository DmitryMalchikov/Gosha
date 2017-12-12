using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeState : MonoBehaviour {

   public Animator animator;
   public  Toggle toggle;
    
	// Use this for initialization
	void Awake () {
        animator = GetComponent<Animator>();
        toggle = GetComponent<Toggle>();

        animator.SetBool("IsOn", Canvaser.CurrentVolume == -80);
        toggle.isOn = Canvaser.CurrentVolume == -80;
    }
	
    public void Switch()
    {
        animator.SetBool("IsOn", toggle.isOn);
        Canvaser.Instance.Mute(toggle.isOn);
    }

    public void SwitchOn()
    {
        toggle.isOn = true;
        animator.SetBool("IsOn",true);
    }

    public void SwitchOff()
    {
        toggle.isOn = false;
        animator.SetBool("IsOn", false);
    }
}

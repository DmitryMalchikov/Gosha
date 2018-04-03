using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceCreamChanger : MonoBehaviour {

    public Text IceCream;
    public int CurrentCount;
    public float Duration = 2;

    public void ChangeIceCream(int count)
    {
        if (CurrentCount != count)
        {
            if ((!gameObject.activeInHierarchy) || CurrentCount == 0)
            {
                SetIceCream(count);
            }
            else
            {
                UpdateIceCream(count);
            }
        }
    }

    public void SetIceCream(int count)
    {
        CurrentCount = count;
        IceCream.text = count.ToString();
    }

    public void UpdateIceCream(int count)
    {
        StartCoroutine(Transition(count));
    }

    IEnumerator Transition(int endVal)
    {
        float lerp = Time.deltaTime / Duration;
        while (Mathf.Abs(endVal - CurrentCount)>0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lerp += Time.deltaTime / Duration;
            CurrentCount = (int)Mathf.Lerp(CurrentCount, endVal, lerp);
            IceCream.text = CurrentCount.ToString();
        }
        //CurrentCount++;
        //IceCream.text = CurrentCount.ToString();
    }
}

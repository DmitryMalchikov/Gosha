using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCoins : MonoBehaviour
{
    public Vector3 StartPosition;
    
    void Update()
    {
        transform.Translate(GameController.Instance.Speed * Time.deltaTime);
    }

    public void Activate()
    {
        transform.position = new Vector3(StartPosition.x, StartPosition.y, StartPosition.z * (GameController.Instance.Speed.z / -10f));
        gameObject.SetActive(true);
        StartCoroutine(TurnOffCoins());
    }

    IEnumerator TurnOffCoins()
    {
        yield return new WaitForSeconds(8);

        gameObject.SetActive(false);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}

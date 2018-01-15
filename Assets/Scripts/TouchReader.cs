using System.Collections;
using UnityEngine;

public class TouchReader : MonoBehaviour {

    public bool sas;
    public PlayerController PC;
    bool wasMove = false;

    public float DoubleTapTime = 0.1f;
    public float sqrMag = 1;

    Vector3 pos;
    bool wasClick = false;

    public Collider col;

    void Start()
    {
        col = GetComponent<Collider>();

		if (!PC) {
			StartCoroutine (GetPC ());
		}

    }

	IEnumerator GetPC(){
		yield return new WaitUntil (() => PlayerController.Instance != null);

		PC = PlayerController.Instance;
	}

    void OnMouseDown()
    {
        if (GameController.Instance.Started)
        {
            pos = Input.mousePosition;
            sas = true;
            if (!wasClick)
            {
                wasMove = false;
                wasClick = true;
                StartCoroutine(CheckClick());
            }
            else if (!wasMove)
            {
                Debug.Log("Double tap");
                GameController.Instance.UseBonus();
            }
        }
    }

    void OnMouseDrag()
    {

        if (GameController.Instance.Started && sas)
        {
            //Debug.Log(Input.mousePosition.x - pos.x);
            if (Mathf.Abs(Input.mousePosition.y - pos.y) > Mathf.Abs(Input.mousePosition.x - pos.x))
            {
                if (Input.mousePosition.y - pos.y > sqrMag)
                {
                    PC.Jump();
                    sas = false;
                    wasMove = true;
                }
                else if (Input.mousePosition.y - pos.y < -sqrMag)
                {
                    PC.Crouch();
                    sas = false;
                    wasMove = true;
                }
            }
            else if (Mathf.Abs(Input.mousePosition.x - pos.x) > sqrMag)
            {
                PC.Move((Input.mousePosition.x - pos.x) > 0);
                sas = false;
                wasMove = true;
            }
        }

    }

    IEnumerator CheckClick()
    {
        yield return new WaitForSeconds(DoubleTapTime);

        if (wasClick)
        {
            wasClick = false;
        }
    }

    //private void Update()
    //{
    //}

    //void OnMouseUp()
    //{
    //    Debug.Log("Up");
    //    if (sas)
    //    {
    //        if (Mathf.Abs(Input.mousePosition.y - pos.y) > Mathf.Abs(Input.mousePosition.x - pos.x))
    //        {
    //            if ((Input.mousePosition.y - pos.y) > sqrMag)
    //            {
    //                PC.Jump();
    //            }
    //            else if (Input.mousePosition.y - pos.y < sqrMag)
    //            {
    //                PC.Crouch();
    //            }
    //        }
    //        else if (Mathf.Abs(Input.mousePosition.x - pos.x) > sqrMag)
    //        {
    //            PC.Move((Input.mousePosition.x - pos.x) > 0);
    //        }
    //        sas = false;
    //    }
    //}

}

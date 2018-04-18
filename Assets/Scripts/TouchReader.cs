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
        if (GameController.Instance.Started && Time.timeScale > 0)
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

        if (GameController.Instance.Started && sas && Time.timeScale > 0)
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

	#if UNITY_EDITOR
	void Update(){
		if (!GameController.Instance.Started)
			return;

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			PC.Move (true);
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			PC.Move (false);
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			PC.Crouch ();
		}
		else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			PC.Jump ();
		}
	}
	#endif
}

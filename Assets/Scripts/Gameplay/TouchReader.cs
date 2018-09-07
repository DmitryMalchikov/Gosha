using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchReader : MonoBehaviour
{

    public bool sas;
    public PlayerController PC;
    bool wasMove = false;

    public float DoubleTapTime = 0.1f;
    public float sqrMag = 1;

    Vector3 pos;
    bool wasClick = false;

    public Collider col;

    private static Queue<bool> _moveInputs = new Queue<bool>();
    private bool _lastInput = false;

    void Start()
    {
        col = GetComponent<Collider>();

        if (!PC)
        {
            StartCoroutine(GetPC());
        }

    }

    IEnumerator GetPC()
    {
        yield return new WaitUntil(() => PlayerController.Instance != null);

        PC = PlayerController.Instance;
    }

    void OnMouseDown()
    {
        if (!Canvaser.Instance.MainMenu.activeInHierarchy && GameController.Instance.Started && Time.timeScale > 0)
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
                if (GameController.Instance.CanUseCurrentBonus)
                {
                    GameController.Instance.UseBonus();
                }
            }
        }
    }

    void OnMouseDrag()
    {

        if (GameController.Instance.Started && sas && Time.timeScale > 0)
        {
            if (DistanceAbsX > sqrMag || DistanceAbsY > sqrMag)
            {
                if (DistanceAbsX > DistanceAbsY)
                {
                    _moveInputs.Enqueue(DistanceX > 0);
                    //PC.Move(DistanceX > 0);
                    sas = false;
                    wasMove = true;
                }
                else
                {
                    if (DistanceY > sqrMag)
                    {
                        PC.Jump();
                        sas = false;
                        wasMove = true;
                    }
                    else if (DistanceY < -sqrMag)
                    {
                        PC.Crouch();
                        sas = false;
                        wasMove = true;
                    }
                }
            }
        }
    }

    private float DistanceX { get { return Input.mousePosition.x - pos.x; } }
    private float DistanceY { get { return Input.mousePosition.y - pos.y; } }
    private float DistanceAbsX { get { return Mathf.Abs(DistanceX); } }
    private float DistanceAbsY { get { return Mathf.Abs(DistanceY); } }

    IEnumerator CheckClick()
    {
        yield return new WaitForSeconds(DoubleTapTime);

        if (wasClick)
        {
            wasClick = false;
        }
    }

    public static void ClearInputs()
    {
        _moveInputs.Clear();
    }

    void Update()
    {
        if (!GameController.Instance.Started)
            return;

        if (_moveInputs.Count > 0 && (!PC.isMoving || _lastInput != _moveInputs.Peek()))
        {
            bool move = _moveInputs.Dequeue();
            PC.Move(move);
            _lastInput = move;
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _moveInputs.Enqueue(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _moveInputs.Enqueue(false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PC.Crouch();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PC.Jump();
        }
#endif
    }

}

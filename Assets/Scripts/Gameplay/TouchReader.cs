using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class TouchReader : MonoBehaviour
    {
        public bool sas;
        public PlayerController PC;
        private bool _wasMove = false;

        public float DoubleTapTime = 0.1f;
        public float SqrMag = 1;

        private Vector3 _pos;
        private bool _wasClick = false;

        public Collider Col;

        private static Queue<bool> _moveInputs = new Queue<bool>();
        private bool _lastInput = false;

        void OnMouseDown()
        {
            if (Canvaser.Instance.MainMenu.activeInHierarchy || !GameController.Started ||
                !(Time.timeScale > 0)) return;

            _pos = Input.mousePosition;
            sas = true;
            if (!_wasClick)
            {
                _wasMove = false;
                _wasClick = true;
                StartCoroutine(CheckClick());
            }
            else if (!_wasMove)
            {
                if (GameController.Instance.CanUseCurrentBonus)
                {
                    GameController.Instance.UseBonus();
                }
            }
        }

        void OnMouseDrag()
        {
            if (GameController.Started && sas && Time.timeScale > 0)
            {
                if (!(DistanceAbsX > SqrMag) && !(DistanceAbsY > SqrMag)) return;

                if (DistanceAbsX > DistanceAbsY)
                {
                    _moveInputs.Enqueue(DistanceX > 0);
                    //PC.Move(DistanceX > 0);
                    sas = false;
                    _wasMove = true;
                }
                else
                {
                    if (DistanceY > SqrMag)
                    {
                        PC.Jump();
                        sas = false;
                        _wasMove = true;
                    }
                    else if (DistanceY < -SqrMag)
                    {
                        PC.Crouch();
                        sas = false;
                        _wasMove = true;
                    }
                }
            }
        }

        private float DistanceX { get { return Input.mousePosition.x - _pos.x; } }
        private float DistanceY { get { return Input.mousePosition.y - _pos.y; } }
        private float DistanceAbsX { get { return Mathf.Abs(DistanceX); } }
        private float DistanceAbsY { get { return Mathf.Abs(DistanceY); } }

        private IEnumerator CheckClick()
        {
            yield return new WaitForSeconds(DoubleTapTime);

            if (_wasClick)
            {
                _wasClick = false;
            }
        }

        public static void ClearInputs()
        {
            _moveInputs.Clear();
        }

        void Update()
        {
            if (!GameController.Started)
                return;

            if (_moveInputs.Count > 0 && (!PC.IsMoving || _lastInput != _moveInputs.Peek()))
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
}

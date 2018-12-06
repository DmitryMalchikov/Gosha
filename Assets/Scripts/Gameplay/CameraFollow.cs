using System.Collections;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class CameraFollow : Singleton<CameraFollow>
    {
        private Transform _target;
        private static Vector3 _offset;
        private static float _zOffset;
        public float SpeedY = 12;
        public float SpeedX = 20;
        private float _y;
        private float _x;
        private float _z;
        public Animator Animator;

        public float ShakeDuration = 1;
        public float ShakeAmount = .5f;

        private float _shakeDuration;
        private bool _shaking = false;
        private Vector3 _originalPos;

        private void OnEnable()
        {
            if (_target == null)
            {
                _target = PlayerController.Instance.transform;
            }
            _offset = transform.position - _target.position;
            _zOffset = _offset.z;
            _y = _target.position.y;
            //x = transform.position.x;
            Animator.enabled = false;

            PlayerRigidbody.FreezeExceptJump();
            Canvaser.Instance.GamePanel.gameObject.SetActive(true);
            ScoreManager.StartRun();
            GameController.Instance.ResetScores();
        }

        void LateUpdate()
        {
            _y = Mathf.Lerp(transform.position.y, PlayerController.Instance.LastGroundY + _offset.y, SpeedY * Time.deltaTime);
            _x = Mathf.Lerp(transform.position.x, _target.position.x / 1.15f + _offset.x, SpeedX * Time.deltaTime);
            _z = Mathf.Lerp(transform.position.z, _target.position.z + _offset.z, SpeedY * Time.deltaTime);
            if (!_shaking)
            {
                transform.position = new Vector3(_x, _y, _z);
            }
            else
            {
                _originalPos = new Vector3(_x, _y, _z);
            }
        }

        public void ChangeCamera()
        {
            if (!Animator.enabled)
            {
                Animator.enabled = true;
            }
            Animator.SetTrigger("Change");
        }

        public void ShakeCamera()
        {
            _shakeDuration = ShakeDuration;
            StartCoroutine(Shake());
        }

        public static void OnHit()
        {
            _offset.z = -3.2f;
        }

        public static void ResetOffset()
        {
            _offset.z = _zOffset;
        }

        IEnumerator Shake()
        {
            _shaking = true;

            while (_shakeDuration > 0)
            {
                yield return new WaitForEndOfFrame();
                transform.position = _originalPos + Random.insideUnitSphere * ShakeAmount;
                _shakeDuration -= Time.deltaTime;
            }

            _shaking = false;
        }
    }
}

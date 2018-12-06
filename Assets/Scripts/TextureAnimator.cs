using Assets.Scripts.Gameplay;
using UnityEngine;

namespace Assets.Scripts
{
    public class TextureAnimator : MonoBehaviour
    {
        public float SpeedCoef = .5f;
        private Material mat;
        private float _offset = 0;

        void Start()
        {
            mat = GetComponent<Renderer>().material;
        }
    
        void Update()
        {
            if (!GameController.Started)
                return;

            _offset += Time.deltaTime * SpeedController.Speed.z * SpeedCoef;
            _offset %= 1;
            mat.SetTextureOffset("_MainTex", new Vector2(0, _offset));
        }
    }
}

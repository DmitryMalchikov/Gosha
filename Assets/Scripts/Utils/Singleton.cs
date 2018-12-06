using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
            private set
            {
                if (!_instance)
                {
                    _instance = value;
                }
            }
        }

        private static T _instance;

        private void Awake()
        {
            Instance = this as T;
        }
    }
}

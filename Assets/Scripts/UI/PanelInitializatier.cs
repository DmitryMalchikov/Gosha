using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PanelInitializatier : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _panels;

        private void Start()
        {
            for (int i  = 0; i < _panels.Length; i++)
            {
                _panels[i].SetActive(false);
            }
        }
    }
}

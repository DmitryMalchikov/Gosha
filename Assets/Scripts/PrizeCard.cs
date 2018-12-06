using UnityEngine;

namespace Assets.Scripts
{
    public class PrizeCard : MonoBehaviour
    {
        public Material mat;

        public void SetCard(string name = "")
        {
            mat.mainTexture = Resources.Load<Texture2D>(name);
        }
    }
}

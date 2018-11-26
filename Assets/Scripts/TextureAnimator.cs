using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    public float SpeedCoef = .5f;
    private Material mat;
    private float offset = 0;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        if (!GameController.Started)
            return;

        offset += Time.deltaTime * SpeedController.Speed.z * SpeedCoef;
        offset %= 1;
        mat.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }
}

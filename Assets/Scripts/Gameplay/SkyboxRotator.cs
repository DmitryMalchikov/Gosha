using UnityEngine;
using VacuumShaders.CurvedWorld;

public class SkyboxRotator : Singleton<SkyboxRotator>
{
    private static float _skyboxRotation = 0f;

    [SerializeField]
    private Material _skybox;
    [SerializeField]
    private float _rotationCoef;

    public static void Rotate()
    {
        _skyboxRotation = (_skyboxRotation + CurvedWorld_Controller.get._V_CW_Bend_Y * Instance._rotationCoef * Time.deltaTime) % 360;
        Instance._skybox.SetFloat("_Rotation", _skyboxRotation);
    }
}

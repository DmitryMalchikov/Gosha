using UnityEngine;

public class Masks : MonoBehaviour
{
    public static int EnvironmentMask { get; private set; }
    public static int DefaultLayer { get; private set; }
    public static int GroundLayer { get; private set; }
    public static int ObstaclesAndPickables { get; private set; }

    private void Awake()
    {
        EnvironmentMask = LayerMask.GetMask("Ground", "Default");
        DefaultLayer = LayerMask.NameToLayer("Default");
        GroundLayer = LayerMask.NameToLayer("Ground");
        ObstaclesAndPickables = LayerMask.GetMask("Default", "Pickable");
    }
}

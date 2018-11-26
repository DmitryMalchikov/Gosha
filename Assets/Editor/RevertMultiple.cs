using UnityEditor;
using UnityEngine;

public class RevertMultiple
{
    [MenuItem("Tools/Revert to Prefab %r")]
    static void Revert()
    {
        var selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++) {              
                PrefabUtility.ReconnectToLastPrefab(selection[i]);
                //PrefabUtility.RevertPrefabInstance(selection[i]);
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }
}

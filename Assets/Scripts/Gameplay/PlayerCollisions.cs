using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : Singleton<PlayerCollisions>
{
    private static HashSet<GameObject> _collisions = new HashSet<GameObject>();

    public static bool AnyCollisions
    {
        get
        {
            return _collisions.Count > 0;
        }
    }

    public static void ClearCollisions()
    {
        _collisions.Clear();
    }

    public static void ClearCollisionsExceptGround()
    {
        _collisions.RemoveWhere(col => col.name != "Ground");
    }

    public static bool AddCollision(GameObject collision)
    {
        return _collisions.Add(collision);
    }

    public static bool HaveCollision(GameObject collision)
    {
        return _collisions.Contains(collision);
    }

    public static bool RemoveCollision(GameObject collision)
    {
        var toRemove = _collisions.RemoveWhere(col => col == collision);
        return toRemove > 0;
    }
}

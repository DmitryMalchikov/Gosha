using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class IceCremChecker : MonoBehaviour
    {
        void Start()
        {
            List<Transform> ic = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                ic.Add(transform.GetChild(i));
            }

            var list = CheckDuplicatePos(ic);
            for (int i = 0; i < ic.Count; i++)
            {
                if (!list.Contains(ic[i]))
                {
                    DestroyImmediate(ic[i].gameObject);
                }
            }
        }

        private List<Transform> CheckDuplicatePos(List<Transform> obj)
        {
            return obj.Distinct(new TransformComparer()).ToList();
        }

    }

    public class TransformComparer : IEqualityComparer<Transform>
    {
        public bool Equals(Transform x, Transform y)
        {
            return Vector3.Distance(x.localPosition, y.localPosition) < 0.03f;
        }

        public int GetHashCode(Transform obj)
        {
            return 1;
        }
    }
}
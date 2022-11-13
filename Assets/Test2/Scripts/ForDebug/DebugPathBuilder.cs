using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class DebugPathBuilder : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    private void OnDrawGizmos()
    {

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Transform child = transform.GetChild(i);
            Transform nextChild = transform.GetChild(i + 1);
            DebugRect rect = child.GetComponent<DebugRect>();
            DebugRect nextRect = nextChild.GetComponent<DebugRect>();
            /*if (rect.Intersect(nextRect))
                if (rect.Min.x == nextRect.Max.x || rect.Max.x == nextRect.Min.x)
                    Gizmos.color = Color.red;
                else*/
            Gizmos.color = Color.white * .5f;
            nextRect.Parent = rect;
            Gizmos.DrawLine(child.position, nextChild.position);

        }
    }

    private IEnumerable<Edge> GetEdges()
    {
        List<Edge> result = new List<Edge>();
        for (int i = 1; i < transform.childCount; i++)
        {
            result.Add(transform.GetChild(i).GetComponent<DebugRect>().GetEdge());
        }
        return result;
    }
}

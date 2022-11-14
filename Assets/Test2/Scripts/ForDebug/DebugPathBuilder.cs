using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class DebugPathBuilder : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private List<Vector2> _path;
    private void OnDrawGizmos()
    {

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Transform child = transform.GetChild(i);
            Transform nextChild = transform.GetChild(i + 1);
            DebugRect rect = child.GetComponent<DebugRect>();
            DebugRect nextRect = nextChild.GetComponent<DebugRect>();
            Gizmos.color = Color.white * .5f;
            nextRect.Parent = rect;
            Gizmos.DrawLine(child.position, nextChild.position);
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Gizmos.DrawLine(_path[i], _path[i + 1]);
        }
    }

    [ContextMenu("TestPathFind")]
    private void TestPathFind()
    {
        _path = new PathFinder().GetPath(_startPoint.position, _endPoint.position, GetEdges()).ToList();

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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;


public class ConeCheck : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end1;
    [SerializeField] private Transform end2;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    private Vector3 Center => (end1.position + end2.position) * .5f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(start.position, end1.position);
        Gizmos.DrawLine(start.position, end2.position);
    }
    [ContextMenu("Check")]
    private void Check()
    {
        SetRightSide();
        var result1 = CheckPoint(point1.position);
        Debug.Log(result1);
        var result2 = CheckPoint(point2.position);
        Debug.Log(result2);
        if (result1 == PointSide.InBound && result2 == PointSide.InBound)
        {
            end1.position = point1.position;
            end2.position = point2.position;
            SetRightSide();
        }
        if (result1 == PointSide.OutLeft && result2 == PointSide.OutRight ||
            result1 == PointSide.OutRight && result2 == PointSide.OutLeft)
        {
           end1.position = LineIntersect(start.position, end1.position, point1.position, point2.position);
           end2.position = LineIntersect(start.position, end2.position, point1.position, point2.position);
        }
        if (result1 == PointSide.OutLeft && result2 == PointSide.InBound)
        {
            end1.position = point1.position;
        }
        if(result1 == PointSide.InBound && result2 == PointSide.OutLeft)
        {
            end1.position = point2.position;
        }

        if (result1 == PointSide.OutRight && result2 == PointSide.InBound)
        {
            end2.position = point1.position;
        }
        if (result1 == PointSide.InBound && result2 == PointSide.OutRight)
        {
            end2.position = point2.position;
        }
    }

    private void SetRightSide()
    {
        if (CheckSide(start.position, Center, end1.position))
        {
            var temp = end1.position;
            end1.position = end2.position;
            end2.position = temp;
        }
    }

    private PointSide CheckPoint(Vector3 point)
    {
        if (Vector3.Dot(Center - start.position, point - start.position) < 0)
            return PointSide.FullOut;
        bool leftSide = CheckSide(start.position, end1.position, point);
        bool rightSide = CheckSide(end2.position, start.position, point);
        if (leftSide && rightSide)
            return PointSide.InBound;
        if (!leftSide && rightSide)
            return PointSide.OutLeft;
        if (leftSide && !rightSide)
            return PointSide.OutRight;
        return PointSide.FullOut;
    }
    private static (float a, float b, float c) ConvertToABC(Vector2 p1, Vector2 p2)
    {
        return (p1.y - p2.y, p2.x - p1.x, p1.x * p2.y - p2.x * p1.y);
    }
    public static Vector2 LineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var (a1, b1, c1) = ConvertToABC(p1, p2);
        var (a2, b2, c2) = ConvertToABC(p3, p4);
        Vector2 result = Vector3.zero;
        float num = a1 * b2 - a2 * b1;
        result.x = (-(c1 * b2 - c2 * b1)) / num;
        result.y = (-(a1 * c2 - a2 * c1)) / num;
        return result;
    }

    private bool CheckSide(Vector3 start, Vector3 end, Vector3 point)
    {
        return (start.x - point.x) * (end.y - start.y) - (end.x - start.x) * (start.y - point.y) < 0;
    }
}

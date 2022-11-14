using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
public class RaysCone
{
    private struct LineData
    {
        public float A;
        public float B;
        public float C;
        public void Deconstruct(out float a, out float b, out float c)
        {
            a = A;
            b = B;
            c = C;
        }
    }
    private Vector2 _start;
    private Vector2 _end1;
    private Vector2 _end2;
    private Vector2 _endCenter;

    public RaysCone(Vector2 start, Vector2 end1, Vector2 end2)
    {
        _start = start;
        _end1 = end1;
        _end2 = end2;
        _endCenter = (_end1 + _end2) * .5f;
        SetRightSide();
    }

    public Vector2 GetEnd()
    {
        return _endCenter;
    }

    public bool CheckEdge(Vector2 point1, Vector2 point2)
    {
        var result1 = CheckPoint(point1);
        var result2 = CheckPoint(point2);
        if (result1 == PointSide.InBound && result2 == PointSide.InBound)
        {
            _end1 = point1;
            _end2 = point2;
        }
        else if (result1 == PointSide.OutLeft && result2 == PointSide.OutRight ||
            result1 == PointSide.OutRight && result2 == PointSide.OutLeft)
        {
            _end1 = LineIntersect(_start, _end1, point1, point2);
            _end2 = LineIntersect(_start, _end2, point1, point2);
        }
        else if (result1 == PointSide.OutLeft && result2 == PointSide.InBound)
        {
            _end1 = LineIntersect(_start, _end1, point1, point2);
            _end2 = point2;
        }
        else if (result1 == PointSide.InBound && result2 == PointSide.OutLeft)
        {
            _end1 = LineIntersect(_start, _end1, point1, point2);
            _end1 = point1;
        }
        else if (result1 == PointSide.OutRight && result2 == PointSide.InBound)
        {
            _end1 = point2;
            _end2 = LineIntersect(_start, _end2, point1, point2);
        }
        else if (result1 == PointSide.InBound && result2 == PointSide.OutRight)
        {
            _end1 = point1;
            _end2 = LineIntersect(_start, _end2, point1, point2);
        }
        else
        {
            return false;
        }
        _endCenter = (_end1 + _end2) * .5f;
        SetRightSide();
        return true;
    }
    public PointSide CheckPoint(Vector2 point)
    {
        if (Vector3.Dot(_endCenter - _start, point - _start) < 0)
            return PointSide.FullOut;
        bool leftSide = CheckSide(_start, _end1, point);
        bool rightSide = CheckSide(_end2, _start, point);
        if (leftSide && rightSide)
            return PointSide.InBound;
        if (!leftSide && rightSide)
            return PointSide.OutLeft;
        if (leftSide && !rightSide)
            return PointSide.OutRight;
        return PointSide.FullOut;
    }
    public bool TryGetIntersectsWithEdges(Edge edge1, Edge edge2, out Vector2 result)
    {
        result = edge1.First.Center;
        var lines = new LineData[4];
        lines[0] = ConvertToABC(edge1.Start, edge2.Start);
        lines[1] = ConvertToABC(edge1.Start, edge2.End);
        lines[2] = ConvertToABC(edge1.End, edge2.Start);
        lines[3] = ConvertToABC(edge1.End, edge2.End);

        var thisLines = new LineData[2];
        thisLines[0] = ConvertToABC(_start, _end1);
        thisLines[1] = ConvertToABC(_start, _end2);
        var intersects = new List<Vector2>();
        foreach (var thisLine in thisLines)
        {
            foreach (var line in lines)
            {
                Vector2 intersection = LineIntersect(thisLine, line);
                if (edge1.First.Contains(intersection))
                {
                    intersects.Add(intersection);
                    Debug.Log("пересечение");
                }
            }
        }

        if (intersects.Count > 0)
        {
            Vector2 summ = Vector2.zero;
            foreach (var intersect in intersects)
            {
                summ += intersect;
            }
            summ /= intersects.Count;
            result = summ;
            return true;
        }
        /*if (intersects.Count > 1)
        {
            var edgeCenter = (Vector2)(edge1.Start + edge1.End) * .5f;
            float minDistance = float.PositiveInfinity;
            for (int i = 0; i < intersects.Count; i++)
            {
                float distance = (edgeCenter - intersects[i]).SqrMagnitude();
                if (distance < minDistance)
                {
                    result = intersects[i];
                    minDistance = distance;
                }
            }
            return true;
        }

        if (intersects.Count == 1)
        {
            result = intersects[0];
            return true;
        }*/

        return false;
    }
    private void SetRightSide()
    {
        if (CheckSide(_start, _endCenter, _end1))
        {
            var temp = _end1;
            _end1 = _end2;
            _end2 = temp;
        }
    }
    private bool CheckSide(Vector2 start, Vector2 end, Vector2 point)
    {
        return (start.x - point.x) * (end.y - start.y) - (end.x - start.x) * (start.y - point.y) < 0;
    }

    private LineData ConvertToABC(Vector2 p1, Vector2 p2)
    {
        return new LineData() { A = p1.y - p2.y, B = p2.x - p1.x, C = p1.x * p2.y - p2.x * p1.y };
    }
    private bool SegmentIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 result)
    {
        var (a1, b1, c1) = ConvertToABC(p1, p2);
        var (a2, b2, c2) = ConvertToABC(p3, p4);
        result = Vector3.zero;
        float num = a1 * b2 - a2 * b1;
        if (Mathf.Abs(num) < float.Epsilon)
        {
            return false;
        }
        result.x = (0f - (c1 * b2 - c2 * b1)) / num;
        result.y = (0f - (a1 * c2 - a2 * c1)) / num;
        return true;
    }

    private Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        var (a1, b1, c1) = ConvertToABC(p1, p2);
        var (a2, b2, c2) = ConvertToABC(p3, p4);
        Vector2 result = Vector3.zero;
        float num = a1 * b2 - a2 * b1;
        result.x = (-(c1 * b2 - c2 * b1)) / num;
        result.y = (-(a1 * c2 - a2 * c1)) / num;
        return result;
    }
    private Vector2 LineIntersect(LineData l1, LineData l2)
    {
        var (a1, b1, c1) = l1;
        var (a2, b2, c2) = l2;
        Vector2 result = Vector3.zero;
        float num = a1 * b2 - a2 * b1;
        result.x = (-(c1 * b2 - c2 * b1)) / num;
        result.y = (-(a1 * c2 - a2 * c1)) / num;
        return result;
    }
}

public class PathFinder : IPathFinder
{
    public IEnumerable<Vector2> GetPath(Vector2 A, Vector2 C, IEnumerable<Edge> edges)
    {
        var path = new List<Vector2>();
        if (!DataValidation(A, C, edges))
            return path;

        path.Add(A);
        var first = edges.First();
        RaysCone raysCone = new RaysCone(A, first.Start, first.End);
        bool rayIsEnd = false;
        Edge prevEdge = new Edge();
        foreach (var edge in edges.Skip(1))
        {
            if (rayIsEnd)
            {
                Vector2 point;
                if(!raysCone.TryGetIntersectsWithEdges(prevEdge, edge, out point))
                    path.Add(raysCone.GetEnd());
                path.Add(point);
                raysCone = new RaysCone(point, prevEdge.Start, prevEdge.End);
                rayIsEnd = false;
            }
            if (!raysCone.CheckEdge(edge.Start, edge.End))
            {
                rayIsEnd = true;
            }
            prevEdge = edge;
        }
        if (raysCone != null)
        {
            if (raysCone.CheckPoint(C) != PointSide.InBound)
                path.Add(raysCone.GetEnd());
            path.Add(C);
        }



        Debug.Log("Путь построен успешно");
        return path;
    }

    private bool DataValidation(Vector2 startPoint, Vector2 endPoint, IEnumerable<Edge> edges)
    {
        var firstRect = edges.First().First;
        if (!firstRect.Contains(startPoint))
        {
            Debug.LogWarning($"Стартовая точка {startPoint} запределами первого прямоугольника {firstRect}");
            return false;
        }
        var lastRect = edges.Last().Second;
        if (!lastRect.Contains(endPoint))
        {
            Debug.LogWarning($"Конечная точка {endPoint} запределами последнего прямоугольника {lastRect}");
            return false;
        }

        Edge? lastEdge = null;
        foreach (var edge in edges)
        {
            if (edge.Start == edge.End)
            {
                Debug.LogWarning("Ребро с нулевой длинной");
                return false;
            }
            if (lastEdge != null)
            {
                if (lastEdge.Value.Second != edge.First)
                {
                    Debug.LogWarning($"Неверная последовательность рёбер{lastEdge.Value.Second} {edge.First}");
                    return false;
                }
            }
            lastEdge = edge;
        }
        return true;
    }


}

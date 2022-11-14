using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;

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
                if (!raysCone.TryGetIntersectsWithEdges(prevEdge, edge, out point))
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
            if (raysCone.CheckPoint(C) == PointSide.InBound)
            {
                path.Add(C);
            }
            else
            {
                Vector2 point;
                if (!raysCone.TryGetIntersectsWithEdges(prevEdge, C, out point))
                    path.Add(raysCone.GetEnd());
                path.Add(point);
                path.Add(C);
            }
        }
        if (path.Count > 0)
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

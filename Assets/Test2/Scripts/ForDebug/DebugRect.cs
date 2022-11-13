using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRect : MonoBehaviour
{
    [SerializeField] private Vector2 _size = Vector2.one;
    [SerializeField] private bool _autoSize = true;

    [HideInInspector]
    public DebugRect Parent;
    public Vector2 Min => ((Vector2)transform.position - _size * .5f).Round(2);
    public Vector2 Max => ((Vector2)transform.position + _size * .5f).Round(2);
    public bool Intersect(DebugRect rect)
    {
        if (Max.x < rect.Min.x || Min.x > rect.Max.x) return false;
        if (Max.y < rect.Min.y || Min.y > rect.Max.y) return false;
        return true;
    }

    public Edge GetEdge()
    {
        Edge edge = new Edge();
        if (Parent != null)
        {
            if (TryGetIntersectEdge(Parent, out var start, out var end))
            {
                edge.Start = start;
                edge.End = end;
            }
            edge.First = new Rectangle() { Min = Parent.Min, Max = Parent.Max };
            edge.Second = new Rectangle() { Min = Min, Max = Max };
        }
        return edge;
    }

    private void CorrectSize()
    {
        if (Parent != null)
        {
            float xDistance1 = Mathf.Abs(Parent.Min.x - Max.x);
            float xDistance2 = Mathf.Abs(Parent.Max.x - Min.x);
            float minXDistance = xDistance1 < xDistance2 ? xDistance1 : xDistance2;

            float yDistance1 = Mathf.Abs(Parent.Min.y - Max.y);
            float yDistance2 = Mathf.Abs(Parent.Max.y - Min.y);
            float minYDistance = yDistance1 < yDistance2 ? yDistance1 : yDistance2;

            if (minXDistance < minYDistance)
            {
                if (Intersect(Parent))
                {
                    _size.x -= minXDistance;
                    if (_size.x < 0)
                        _size.x = .1f;
                }
                else
                    _size.x += minXDistance;
            }
            else
            {
                if (Intersect(Parent))
                {
                    _size.y -= minYDistance;
                    if (_size.y < 0)
                        _size.y = .1f;
                }
                else
                    _size.y += minYDistance;
            }
        }
    }
    private bool TryGetIntersectEdge(DebugRect rect, out Vector3 start, out Vector3 end)
    {
        start = Vector3.zero;
        end = Vector3.zero;
        if (!Intersect(Parent))
            return false;
        float z = transform.position.z - 1;
        if (Parent.Max.x == Min.x)
        {
            float x = Min.x;
            float y1 = Parent.Max.y < Max.y ? Parent.Max.y : Max.y;
            float y2 = Parent.Min.y > Min.y ? Parent.Min.y : Min.y;
            start = new Vector3(x, y1, z);
            end = new Vector3(x, y2, z);
            return true;
        }
        if (Parent.Min.x == Max.x)
        {
            float x = Max.x;
            float y1 = Parent.Max.y < Max.y ? Parent.Max.y : Max.y;
            float y2 = Parent.Min.y > Min.y ? Parent.Min.y : Min.y;
            start = new Vector3(x, y1, z);
            end = new Vector3(x, y2, z);
            return true;
        }
        if (Parent.Max.y == Min.y)
        {
            float y = Min.y;
            float x1 = Parent.Max.x < Max.x ? Parent.Max.x : Max.x;
            float x2 = Parent.Min.x > Min.x ? Parent.Min.x : Min.x;
            start = new Vector3(x1, y, z);
            end = new Vector3(x2, y, z);
            return true;
        }
        if (Parent.Min.y == Max.y)
        {
            float y = Max.y;
            float x1 = Parent.Max.x < Max.x ? Parent.Max.x : Max.x;
            float x2 = Parent.Min.x > Min.x ? Parent.Min.x : Min.x;
            start = new Vector3(x1, y, z);
            end = new Vector3(x2, y, z);
            return true;
        }
        return false;
    }
    private void DrawIntersect()
    {
        if (Parent == null)
            return;
        if (TryGetIntersectEdge(Parent, out Vector3 start, out Vector3 end))
        {
            Gizmos.DrawSphere(start, .05f);
            Gizmos.DrawSphere(end, .05f);
            Gizmos.DrawLine(start, end);
        }
    }

    private void OnDrawGizmos()
    {
        if (_autoSize)
            CorrectSize();
        Gizmos.color = Color.white * .8f;
        Gizmos.DrawWireCube(transform.position, _size);
        Gizmos.color = Color.yellow * .5f;
        Gizmos.DrawCube(transform.position, _size);
        Gizmos.color = Color.red;
        DrawIntersect();
    }
}

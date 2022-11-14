using System;
using System.Drawing;
using UnityEngine;

public struct Rectangle
{
    public Vector2 Min;
    public Vector2 Max;

    public Vector2 Center => (Min + Max) * .5f;
    public bool Contains(Vector2 point)
    {
        if (point.x >= Min.x && point.x <= Max.x && point.y >= Min.y && point.y <= Max.y)
            return true;
        return false;
    }
    public static bool operator ==(Rectangle a, Rectangle b)
    {
        return a.Min.Equals(b.Min) &&
               a.Max.Equals(b.Max);
    }
    public static bool operator !=(Rectangle a, Rectangle b)
    {
        return !a.Min.Equals(b.Min) ||
              !a.Max.Equals(b.Max);
    }

    public override string ToString()
    {
        return $"min:{Min},max:{Max}";
    }
    public override bool Equals(object obj)
    {
        return obj is Rectangle rectangle &&
               Min.Equals(rectangle.Min) &&
               Max.Equals(rectangle.Max);
    }

    public override int GetHashCode()
    {
        int hashCode = 1537547080;
        hashCode = hashCode * -1521134295 + Min.GetHashCode();
        hashCode = hashCode * -1521134295 + Max.GetHashCode();
        return hashCode;
    }
}

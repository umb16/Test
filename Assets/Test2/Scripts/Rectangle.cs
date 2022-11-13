using UnityEngine;

public struct Rectangle
{
    public Vector2 Min;
    public Vector2 Max;

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

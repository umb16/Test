using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Round(this Vector2 v, int digits)
    {
        return new Vector2((float)Math.Round(v.x, 2), (float)Math.Round(v.y, 2));
    }
}

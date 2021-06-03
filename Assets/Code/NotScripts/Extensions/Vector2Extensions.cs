using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 GetCopy(this Vector2 original)
    {
        return new Vector2(original.x, original.y);
    }
}

using System.Collections.Generic;
using UnityEngine;

public static class GeometryHelper
{
    public static bool LineIntersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
        if (Mathf.Abs(d) < 0.0001f) return false;
        float ua = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
        float ub = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;
        return ua > 0f && ua < 1f && ub > 0f && ub < 1f;
    }

    public static float PolygonArea(IReadOnlyList<Vector2> poly)
    {
        float a = 0f;
        for (int i = 0; i < poly.Count; i++)
        {
            int j = (i + 1) % poly.Count;
            a += poly[i].x * poly[j].y - poly[j].x * poly[i].y;
        }
        return Mathf.Abs(a * 0.5f);
    }
}

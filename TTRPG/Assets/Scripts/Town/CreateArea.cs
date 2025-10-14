using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateArea : MonoBehaviour
{
    [Header("References")]
    public TownManager town;

    [Header("Area Settings")]
    public float minAreaSize = 1f;
    public float maxAreaSize = 20f;
    public bool debugDraw = true;

    [Header("Area Data")]
    public List<AreaWrapper> areas = new();
    public List<List<Vector2>> pointsTaken = new();

    [Header("Area Type Rules")]
    public float farmlandThreshold = 12f;
    public float industrialThreshold = 8f;
    public float marketThreshold = 5f;
    public float commonThreshold = 3f;

    public enum AreaType
    {
        Farmland,
        Industrial,
        Temple,
        Market,
        CommonHousing,
        NobleHousing
    }

    [System.Serializable]
    public class AreaWrapper
    {
        public AreaType areaType;
        public List<Vector2> points;
        public float areaSize;
    }

    private void Start()
    {
        if (town == null) town = TownManager.instance;
        if (town == null)
        {
            Debug.LogError("CreateArea: TownManager not found.");
            enabled = false;
            return;
        }

        // hook into new node creation event
        HexGraph.instance.OnNodeCreate += OnNodeCreated;
    }

    private void OnNodeCreated(TownManager.Node node, bool newRing)
    {
        if (node == null) return;
        TryDetectNewAreas(node);
    }

    private void TryDetectNewAreas(TownManager.Node node)
    {
        // go through all pairs of neighbors to detect closed loops
        var neighbors = node.neighbors;
        if (neighbors.Count < 2) return;

        for (int i = 0; i < neighbors.Count; i++)
        {
            for (int j = i + 1; j < neighbors.Count; j++)
            {
                var a = neighbors[i];
                var b = neighbors[j];
                if (!AreConnected(a, b)) continue;

                // possible triangle (node, a, b)
                var polygonPoints = new List<Vector2> { node.Pos, a.Pos, b.Pos };
                string key = GetPolygonKey(polygonPoints);
                if (pointsTaken.Any(p => GetPolygonKey(p) == key))
                    continue;

                float area = ComputePolygonArea(polygonPoints);

                // register
                pointsTaken.Add(polygonPoints);
                var wrapper = new AreaWrapper
                {
                    areaType = ClassifyArea(area),
                    points = polygonPoints,
                    areaSize = area
                };
                areas.Add(wrapper);

                // increment shared edge counts
                IncrementEdgeSidesShared(node, a);
                IncrementEdgeSidesShared(a, b);
                IncrementEdgeSidesShared(b, node);

                Debug.Log($"Created new area between {node.name}, {a.name}, {b.name} — {wrapper.areaType}");
            }
        }
    }

    private bool AreConnected(TownManager.Node a, TownManager.Node b)
    {
        return a.neighbors.Contains(b);
    }

    private void IncrementEdgeSidesShared(TownManager.Node a, TownManager.Node b)
    {
        var edge = town.GetType()
            .GetField("edges", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(town) as List<TownManager.Edge>;

        var e = edge.FirstOrDefault(x =>
            (x.a == a && x.b == b) ||
            (x.a == b && x.b == a));

        if (e != null)
            e.sidesShared++;
    }

    private float ComputePolygonArea(List<Vector2> points)
    {
        float area = 0;
        for (int i = 0; i < points.Count; i++)
        {
            int j = (i + 1) % points.Count;
            area += points[i].x * points[j].y - points[j].x * points[i].y;
        }
        return Mathf.Abs(area / 2f);
    }

    private AreaType ClassifyArea(float area)
    {
        if (area >= farmlandThreshold) return AreaType.Farmland;
        if (area >= industrialThreshold) return AreaType.Industrial;
        if (area >= marketThreshold) return AreaType.Market;
        if (area >= commonThreshold) return AreaType.CommonHousing;
        return AreaType.NobleHousing;
    }

    private string GetPolygonKey(List<Vector2> pts)
    {
        var sorted = pts
            .OrderBy(p => p.x)
            .ThenBy(p => p.y)
            .Select(p => $"{p.x:F2},{p.y:F2}");
        return string.Join("_", sorted);
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw || pointsTaken == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.2f);
        foreach (var poly in pointsTaken)
        {
            if (poly.Count < 3) continue;
            for (int i = 0; i < poly.Count; i++)
            {
                Gizmos.DrawLine(poly[i], poly[(i + 1) % poly.Count]);
            }
        }
    }
}

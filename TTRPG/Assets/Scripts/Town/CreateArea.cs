//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using System;

//public class CreateArea : MonoBehaviour
//{
//    [Header("References")]
//    public TownManager town;

//    [Header("Area Settings")]
//    public float minAreaSize = 1f;
//    public float maxAreaSize = 20f;
//    public bool debugDraw = true;

//    [Header("Area Data")]
//    public List<AreaWrapper> areas = new();
//    public List<List<Vector2>> pointsTaken = new();

//    [Header("Area Type Rules")]
//    public float incrementAreaThreshold = 1.1f;
//    public float farmlandThreshold = 12f;
//    public float industrialThreshold = 8f;
//    public float marketThreshold = 5f;
//    public float commonThreshold = 3f;

//    private int ringNum = 0;
//    private int ringNum2 = 5;

//    public event Action<AreaWrapper> OnAreaCreated;

//    private void Start()
//    {
//        HexGraph.instance.OnNewRingCreate += ExpandCriteria;
//        HexGraph.instance.OnNodeCreate += OnNodeCreated;
//    }

//    void ExpandCriteria()
//    {
//        ringNum++;
//        if (ringNum < ringNum2) return;
//        commonThreshold *= incrementAreaThreshold;
//        marketThreshold *= incrementAreaThreshold;
//        farmlandThreshold *= incrementAreaThreshold;
//        industrialThreshold *= incrementAreaThreshold;
//    }

//    private void OnNodeCreated(TownManager.Node node, bool newRing)
//    {
//        if (node == null) return;
//        TryDetectNewAreas(node);
//    }

//    private void TryDetectNewAreas(TownManager.Node start)
//    {
//        var allCycles = new HashSet<string>();
//        foreach (var neighbor in start.neighbors)
//            DFSFindLoops(start, neighbor, new List<TownManager.Node> { start }, allCycles);

//        foreach (var key in allCycles)
//        {
//            var polygonPoints = ParsePolygonKey(key);
//            if (polygonPoints.Count < 3) continue;
//            if (pointsTaken.Any(p => GetPolygonKey(p) == key)) continue;
//            if (IsInsideExistingPolygon(polygonPoints)) continue;
//            if (HasInternalEdges(polygonPoints)) continue;
//            float area = ComputePolygonArea(polygonPoints);
//            pointsTaken.Add(polygonPoints);
//            var wrapper = new AreaWrapper
//            {
//                areaType = ClassifyArea(area),
//                points = polygonPoints,
//                areaSize = area
//            };
//            areas.Add(wrapper);
//            OnAreaCreated?.Invoke(wrapper);
//        }
//    }

//    private void DFSFindLoops(TownManager.Node start, TownManager.Node current, List<TownManager.Node> path, HashSet<string> allCycles)
//    {
//        if (path.Count > 15) return;
//        foreach (var next in current.neighbors)
//        {
//            if (next == start && path.Count >= 3)
//            {
//                var loop = path.Select(n => n.Pos).ToList();
//                var key = GetPolygonKey(loop);
//                allCycles.Add(key);
//                continue;
//            }
//            if (path.Contains(next)) continue;
//            var newPath = new List<TownManager.Node>(path) { next };
//            DFSFindLoops(start, next, newPath, allCycles);
//        }
//    }

//    private bool HasInternalEdges(List<Vector2> polygon)
//    {
//        var allEdges = town.GetType()
//            .GetField("edges", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//            .GetValue(town) as List<TownManager.Edge>;

//        foreach (var edge in allEdges)
//        {
//            if (polygon.Contains(edge.a.Pos) && polygon.Contains(edge.b.Pos)) continue;
//            var mid = (edge.a.Pos + edge.b.Pos) * 0.5f;
//            if (PointInPolygon(mid, polygon))
//                return true;
//        }
//        return false;
//    }

//    private bool IsInsideExistingPolygon(List<Vector2> polygon)
//    {
//        foreach (var existing in pointsTaken)
//        {
//            var mid = GetPolygonCentroid(polygon);
//            if (PointInPolygon(mid, existing))
//                return true;
//        }
//        return false;
//    }

//    private Vector2 GetPolygonCentroid(List<Vector2> poly)
//    {
//        float x = 0, y = 0;
//        foreach (var p in poly)
//        {
//            x += p.x;
//            y += p.y;
//        }
//        return new Vector2(x / poly.Count, y / poly.Count);
//    }

//    private bool PointInPolygon(Vector2 point, List<Vector2> poly)
//    {
//        bool inside = false;
//        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
//        {
//            if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
//                (point.x < (poly[j].x - poly[i].x) * (point.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
//                inside = !inside;
//        }
//        return inside;
//    }

//    private List<Vector2> ParsePolygonKey(string key)
//    {
//        return key.Split('_').Select(s =>
//        {
//            var parts = s.Split(',');
//            return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
//        }).ToList();
//    }

//    private float ComputePolygonArea(List<Vector2> points)
//    {
//        float area = 0;
//        for (int i = 0; i < points.Count; i++)
//        {
//            int j = (i + 1) % points.Count;
//            area += points[i].x * points[j].y - points[j].x * points[i].y;
//        }
//        return Mathf.Abs(area / 2f);
//    }

//    private AreaType ClassifyArea(float area)
//    {
//        int rand = UnityEngine.Random.Range(0, 10);
//        if (area >= farmlandThreshold) return AreaType.Farmland;
//        if (area >= industrialThreshold)
//        {
//            if (rand < 3) return AreaType.Industrial;
//            else return AreaType.CommonHousing;
//        }
//        if (area >= marketThreshold)
//        {
//            if (rand < 3) return AreaType.Market;
//            else return AreaType.CommonHousing;
//        }
//        if (area >= commonThreshold) return AreaType.CommonHousing;
//        if (rand < 5) return AreaType.Temple;
//        else return AreaType.NobleHousing;
//    }

//    private string GetPolygonKey(List<Vector2> pts)
//    {
//        var sorted = pts
//            .OrderBy(p => p.x)
//            .ThenBy(p => p.y)
//            .Select(p => $"{p.x:F2},{p.y:F2}");
//        return string.Join("_", sorted);
//    }

//    private void OnDrawGizmos()
//    {
//        if (!debugDraw || pointsTaken == null) return;
//        Gizmos.color = new Color(0, 1, 0, 0.2f);
//        foreach (var poly in pointsTaken)
//        {
//            if (poly.Count < 3) continue;
//            for (int i = 0; i < poly.Count; i++)
//            {
//                Gizmos.DrawLine(poly[i], poly[(i + 1) % poly.Count]);
//            }
//        }
//    }
//}

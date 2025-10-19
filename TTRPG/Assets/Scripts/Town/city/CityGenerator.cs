using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Header("References")]
    public AreaWrapper areaWrapper;

    [Header("City Layout")]
    public bool straightRoadsFromPrime = true;
    public bool debugDraw = true;
    public Vector2 primePosition = Vector2.zero;
    public int initialRingNodes = 8;
    public int totalRings = 6;
    public float baseRingDistance = 5f;
    public float ringDistanceMultiplier = 1.15f;
    public float nodeJitter = 0.6f;
    public float ringAngleOffset = 0.4f;
    public int minConnectionsPerNode = 1;
    public int maxConnectionsPerNode = 3;

    [Header("Area Rules")]
    public float incrementAreaThreshold = 1.1f;
    public float farmlandThreshold = 12f;
    public float industrialThreshold = 8f;
    public float marketThreshold = 5f;
    public float commonThreshold = 3f;

    public event Action<List<float>, CityAreaType> OnAreaDetected;

    [Header("Generation Info")]
    public List<CityNodeData> nodes = new List<CityNodeData>();
    public List<(Vector2, Vector2)> edges = new List<(Vector2, Vector2)>();
    public int nextId;
    public System.Random rng = new System.Random();
    public int currentRing;
    public static CityGenerator instance;
    public bool generate = false;

    private void Awake()
    {
        instance = this;
    }

    void Start() => GenerateCity(generate);

    private void Update()
    {
        if(generate)
        {
            GenerateCity(generate);
            generate = false;
        }
    }

    public void GenerateCity(bool genFullCity)
    {
        nodes.Clear();
        edges.Clear();
        areaWrapper?.ClearAreas();
        nextId = 0;
        currentRing = 0;

        CreatePrimeNode();
        CreatePrimaryRing();

        if (genFullCity)
        {
            for (int r = 1; r < totalRings; r++)
            {
                CreateRing(r);
                ExpandCriteria();
            }
        }

        if (straightRoadsFromPrime)
            ConnectPrimeToRings();
    }

    public void AddSingleNode(Vector2 position, int ringIndex)
    {
        var n = new CityNode(nextId++, position);
        var data = new CityNodeData(n, ringIndex);
        nodes.Add(data);
    }

    void CreatePrimeNode()
    {
        nodes.Add(new CityNodeData(new CityNode(nextId++, primePosition), 0));
    }

    void CreatePrimaryRing()
    {
        float radius = baseRingDistance;
        for (int i = 0; i < initialRingNodes; i++)
        {
            float a = (2 * Mathf.PI * i / initialRingNodes) + RandomRange(-ringAngleOffset, ringAngleOffset);
            Vector2 p = primePosition + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (radius + RandomRange(-nodeJitter, nodeJitter));
            nodes.Add(new CityNodeData(new CityNode(nextId++, p), 1));
        }

        ConnectRingNodes(1, initialRingNodes);
    }

    void CreateRing(int ringIndex)
    {
        float radius = baseRingDistance * Mathf.Pow(ringDistanceMultiplier, ringIndex);
        int nodeCount = Mathf.RoundToInt(initialRingNodes * Mathf.Pow(1.05f, ringIndex));
        float angleOffset = (float)rng.NextDouble() * Mathf.PI * 2f;

        int startIdx = nodes.Count;
        for (int i = 0; i < nodeCount; i++)
        {
            float a = angleOffset + (2 * Mathf.PI * i / nodeCount) + RandomRange(-ringAngleOffset, ringAngleOffset);
            Vector2 p = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (radius + RandomRange(-nodeJitter, nodeJitter));
            nodes.Add(new CityNodeData(new CityNode(nextId++, p), ringIndex + 1));
        }

        ConnectRingNodes(ringIndex + 1, nodeCount);
        ConnectBetweenRings(ringIndex, ringIndex + 1);
        DetectPolygonsBetweenRings(startIdx, nodeCount, ringIndex);
    }

    void ConnectRingNodes(int ringIndex, int nodeCount)
    {
        int start = nodes.FindIndex(n => n.ringIndex == ringIndex);
        if (start < 0) return;
        for (int i = 0; i < nodeCount; i++)
        {
            int a = start + i;
            int b = start + ((i + 1) % nodeCount);
            ConnectNodes(a, b);
        }
    }

    void ConnectBetweenRings(int lowerRing, int upperRing)
    {
        var lower = nodes.Where(n => n.ringIndex == lowerRing).ToList();
        var upper = nodes.Where(n => n.ringIndex == upperRing).ToList();
        foreach (var u in upper)
        {
            var bottomCandidates = lower.Select(n => n.node).ToList();
            var picks = PickClosestIndices(u.node.pos, bottomCandidates, rng.Next(minConnectionsPerNode, maxConnectionsPerNode + 1));
            foreach (var pick in picks)
            {
                int lowerIdx = nodes.FindIndex(n => n.node.id == pick.id);
                int upperIdx = nodes.FindIndex(n => n.node.id == u.node.id);
                if (lowerIdx >= 0 && upperIdx >= 0)
                    ConnectNodes(upperIdx, lowerIdx);
            }
        }
    }

    void ConnectNodes(int a, int b)
    {
        var A = nodes[a].node.pos;
        var B = nodes[b].node.pos;
        foreach (var e in edges)
        {
            if (GeometryHelper.LineIntersects(A, B, e.Item1, e.Item2))
                return;
        }
        nodes[a].ConnectTo(nodes[b].node.id);
        nodes[b].ConnectTo(nodes[a].node.id);
        edges.Add((A, B));
    }

    void DetectPolygonsBetweenRings(int topStart, int topCount, int bottomRingIndex)
    {
        var bottom = nodes.Select((n, i) => new { n, i }).Where(x => x.n.ringIndex == bottomRingIndex).Select(x => x.i).ToList();
        var top = Enumerable.Range(topStart, topCount).ToList();
        var ordered = bottom.OrderBy(i => Mathf.Atan2(nodes[i].node.pos.y, nodes[i].node.pos.x)).ToList();

        for (int b = 0; b < ordered.Count; b++)
        {
            int b1 = ordered[b];
            int b2 = ordered[(b + 1) % ordered.Count];
            foreach (var tA in top)
            {
                foreach (var tB in top)
                {
                    if (tA == tB) continue;
                    if (nodes[tA].neighbors.Contains(nodes[tB].node.id)
                        && nodes[b1].neighbors.Contains(nodes[tA].node.id)
                        && nodes[b2].neighbors.Contains(nodes[tB].node.id))
                    {
                        var corners = new List<Vector2> { nodes[b1].node.pos, nodes[b2].node.pos, nodes[tB].node.pos, nodes[tA].node.pos };
                        AddDetectedArea(corners);
                    }
                }
            }
        }
    }

    void AddDetectedArea(IReadOnlyList<Vector2> corners)
    {
        var unique = corners.Select(v => new Vector2((float)Math.Round(v.x, 3), (float)Math.Round(v.y, 3))).Distinct().ToList();
        if (unique.Count < 3) return;
        float area = GeometryHelper.PolygonArea(unique);
        var type = ClassifyArea(area);
        areaWrapper?.AddArea(unique, type);
        var flat = new List<float>(unique.Count * 2);
        foreach (var v in unique) { flat.Add(v.x); flat.Add(v.y); }
        OnAreaDetected?.Invoke(flat, type);
    }

    CityAreaType ClassifyArea(float a)
    {
        if (a >= farmlandThreshold) return CityAreaType.Farmland;
        if (a >= industrialThreshold) return CityAreaType.Industrial;
        if (a >= marketThreshold) return CityAreaType.Market;
        if (a >= commonThreshold) return CityAreaType.CommonHousing;
        return CityAreaType.NobleHousing;
    }

    void ExpandCriteria()
    {
        currentRing++;
        commonThreshold *= incrementAreaThreshold;
        marketThreshold *= incrementAreaThreshold;
        farmlandThreshold *= incrementAreaThreshold;
        industrialThreshold *= incrementAreaThreshold;
    }

    void ConnectPrimeToRings()
    {
        var prime = nodes.First();
        for (int ring = 1; ring <= totalRings; ring++)
        {
            var ringNodes = nodes.Where(n => n.ringIndex == ring).ToList();
            if (ringNodes.Count == 0) continue;
            var closest = ringNodes.OrderBy(n => (n.node.pos - prime.node.pos).sqrMagnitude).First();
            int idxPrime = nodes.FindIndex(n => n.node.id == prime.node.id);
            int idxTarget = nodes.FindIndex(n => n.node.id == closest.node.id);
            ConnectNodes(idxPrime, idxTarget);
        }
    }

    List<CityNode> PickClosestIndices(Vector2 pos, List<CityNode> candidates, int pickCount)
    {
        return candidates.OrderBy(c => (c.pos - pos).sqrMagnitude).Take(Mathf.Min(pickCount, candidates.Count)).ToList();
    }

    float RandomRange(float a, float b) => a + (float)rng.NextDouble() * (b - a);

    void OnDrawGizmos()
    {
        if (!debugDraw || nodes == null) return;
        Gizmos.color = Color.green;
        foreach (var n in nodes)
            Gizmos.DrawSphere(new Vector3(n.node.pos.x, n.node.pos.y, 0f), 0.1f);

        Gizmos.color = Color.white;
        foreach (var e in edges)
            Gizmos.DrawLine(new Vector3(e.Item1.x, e.Item1.y, 0f), new Vector3(e.Item2.x, e.Item2.y, 0f));
    }
}

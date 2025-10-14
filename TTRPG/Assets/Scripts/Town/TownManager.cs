using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using System;

public class TownManager : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    public GameObject nodePrefab;

    [Header("Hex Settings")]
    public int hexSides = 6;
    public float ringRadius = 2f;
    public float nodeScale = 0.5f;

    [Header("Expansion Settings")]
    public float minSpacing = 0.8f;
    public float edgeOffsetRandomnessDeg = 25f;
    public int maxPlacementAttempts = 20;
    public float redNodeAreaThreshold = 4f;
    public int maxConnections = 3;
    public int hexNodeConnection = 2;

    [Header("Graphics")]
    public GameObject roadPrefab;
    public int roadWidth = 4;

    [Header("Graph Data (runtime view only)")]
    private List<Node> nodes = new List<Node>();
    private List<Edge> edges = new List<Edge>();

    [Header("Hierarchy Organization")]
    public Transform graphParent;

    public static TownManager instance;

    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class Node
    {
        public string name;
        public Transform transform;
        public float area;
        public int expansionSlots;
        public bool isPrime;
        public bool isLocked;
        public List<Node> neighbors = new List<Node>();
        public Node(string n) { name = n; }
        public Vector2 Pos => transform.position;
    }

    public class Edge
    {
        public Node a;
        public Node b;
        public int sidesShared = 0;
        public Edge(Node a, Node b)
        {
            this.a = a;
            this.b = b;
        }
    }

    void Start()
    {
        if (nodePrefab == null) return;

        if (graphParent == null)
        {
            GameObject parentGO = new GameObject("GraphNodes");
            graphParent = parentGO.transform;
        }

        //GenerateHexBase();
    }

    void OnDrawGizmos()
    {
        if (edges == null) return;
        Gizmos.color = Color.yellow;

        foreach (var e in edges)
        {
            if (e?.a?.transform == null || e?.b?.transform == null) continue;
            Gizmos.DrawLine(e.a.Pos, e.b.Pos);
        }
    }

    void GenerateHexBase()
    {
        Node prime = CreateNode(Vector2.zero, 0.5f, Color.green, 0, true, false);

        float angleStep = 360f / hexSides;
        for (int i = 0; i < hexSides; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;

            Node n = CreateNode(pos, 1f, Color.white, hexNodeConnection, false, false);

            CreateEdge(prime, n);
            if (i > 0) CreateEdge(n, nodes[nodes.Count - 2]);
            if (i == hexSides - 1) CreateEdge(n, nodes[1]);
        }
    }

    public Node CreateNode(Vector2 pos, float area, Color color, int expansionSlots, bool isPrime, bool isLocked)
    {
        GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * nodeScale;
        go.transform.SetParent(graphParent, true);

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;

        Node node = new Node($"Node_{nodes.Count}")
        {
            transform = go.transform,
            area = area,
            expansionSlots = expansionSlots,
            isPrime = isPrime,
            isLocked = isLocked
        };

        nodes.Add(node);
        return node;
    }

    public void CreateEdge(Node a, Node b)
    {
        if (a == null || b == null || EdgeExists(a, b)) return;

        edges.Add(new Edge(a, b));
        a.neighbors.Add(b);
        b.neighbors.Add(a);

        if (roadPrefab != null)
        {
            Vector2 mid = (a.Pos + b.Pos) * 0.5f;
            Vector2 dir = (b.Pos - a.Pos);
            float length = dir.magnitude;

            GameObject road = Instantiate(roadPrefab, mid, Quaternion.identity, graphParent);

            // Rotate to face connection direction (+90° for vertical-aligned sprites)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            road.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

            // Scale the road along its Y-axis instead of X
            var sr = road.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float spriteLength = sr.sprite.bounds.size.y; // note: Y now used
                float spriteWidth = sr.sprite.bounds.size.x;
                road.transform.localScale = new Vector3(roadWidth / spriteWidth, length / spriteLength, 1f);
            }
            else
            {
                // fallback scaling
                road.transform.localScale = new Vector3(roadWidth, length, 1f);
            }
        }
    }


    bool EdgeExists(Node a, Node b)
    {
        return edges.Any(e => (e.a == a && e.b == b) || (e.a == b && e.b == a));
    }

    // ------------------- EXPANSION LOGIC -------------------
    List<Edge> GetCandidateEdges()
    {
        return edges.Where(e =>
            !e.a.isLocked && !e.b.isLocked &&
            e.a.expansionSlots > 0 && e.b.expansionSlots > 0).ToList();
    }

    Vector2 EdgeMidpoint(Edge e) => (e.a.Pos + e.b.Pos) * 0.5f;

    Vector2 OutwardNormal(Edge e)
    {
        Vector2 dir = (e.b.Pos - e.a.Pos).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);
        Vector2 centerToMid = (EdgeMidpoint(e) - Vector2.zero).normalized;
        if (centerToMid == Vector2.zero) return perp;
        if (Vector2.Dot(perp, centerToMid) < 0) perp = -perp;
        return perp;
    }

    bool OverlapsAnyNode(Vector2 pos)
    {
        foreach (var n in nodes)
            if (Vector2.Distance(pos, n.Pos) < minSpacing) return true;
        return false;
    }

    bool WouldCrossExistingEdges(Vector2 newPos, Node a, Node b)
    {
        foreach (var e in edges)
        {
            if (e.a == a || e.b == a || e.a == b || e.b == b) continue;
            if (SegmentsIntersect(newPos, a.Pos, e.a.Pos, e.b.Pos)) return true;
            if (SegmentsIntersect(newPos, b.Pos, e.a.Pos, e.b.Pos)) return true;
        }
        return false;
    }

    bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Cross(p4 - p3, p1 - p3);
        float d2 = Cross(p4 - p3, p2 - p3);
        float d3 = Cross(p2 - p1, p3 - p1);
        float d4 = Cross(p2 - p1, p4 - p1);

        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0))) return true;

        if (Mathf.Approximately(d1, 0) && OnSegment(p3, p4, p1)) return true;
        if (Mathf.Approximately(d2, 0) && OnSegment(p3, p4, p2)) return true;
        if (Mathf.Approximately(d3, 0) && OnSegment(p1, p2, p3)) return true;
        if (Mathf.Approximately(d4, 0) && OnSegment(p1, p2, p4)) return true;

        return false;
    }

    float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        return p.x >= Mathf.Min(a.x, b.x) - Mathf.Epsilon && p.x <= Mathf.Max(a.x, b.x) + Mathf.Epsilon &&
               p.y >= Mathf.Min(a.y, b.y) - Mathf.Epsilon && p.y <= Mathf.Max(a.y, b.y) + Mathf.Epsilon;
    }

    public bool AddRandomExpansionNode(float sizeX)
    {
        var candidates = GetCandidateEdges()
            .OrderBy(e => UnityEngine.Random.value * (1f / (1f + EdgeMidpoint(e).magnitude)))
            .ToList();

        if (candidates.Count == 0) return false;

        int startIndex = UnityEngine.Random.Range(0, candidates.Count);
        for (int i = 0; i < candidates.Count; i++)
        {
            Edge chosen = candidates[(startIndex + i) % candidates.Count];
            Vector2 mid = EdgeMidpoint(chosen);
            Vector2 baseNormal = OutwardNormal(chosen);

            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                float rndDeg = UnityEngine.Random.Range(-edgeOffsetRandomnessDeg, edgeOffsetRandomnessDeg);
                Vector2 rotated = Rotate(baseNormal, rndDeg);
                Vector2 pos = mid + rotated * sizeX;

                if (OverlapsAnyNode(pos)) continue;
                if (WouldCrossExistingEdges(pos, chosen.a, chosen.b)) continue;

                Node newNode = CreateNode(pos, sizeX, new Color(1f, 0.55f, 0f), maxConnections, false, false);
                CreateEdge(newNode, chosen.a);
                CreateEdge(newNode, chosen.b);

                chosen.a.expansionSlots = Mathf.Max(0, chosen.a.expansionSlots - 1);
                chosen.b.expansionSlots = Mathf.Max(0, chosen.b.expansionSlots - 1);

                if (newNode.area >= redNodeAreaThreshold)
                {
                    SpawnRedLockedNode(newNode, chosen.a, chosen.b);
                    ConnectOrInsertBetweenAdjacent(chosen.a, 1.5f, 4f);
                    ConnectOrInsertBetweenAdjacent(chosen.b, 1.5f, 4f);
                }

                return true;
            }
        }
        return false;
    }

    Vector2 Rotate(Vector2 v, float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        float ca = Mathf.Cos(rad);
        float sa = Mathf.Sin(rad);
        return new Vector2(v.x * ca - v.y * sa, v.x * sa + v.y * ca);
    }

    void SpawnRedLockedNode(Node orange, Node a, Node b)
    {
        Vector2 centroid = (orange.Pos + a.Pos + b.Pos) / 3f;
        Node red = CreateNode(centroid, 0.5f, Color.red, 0, false, true);
        CreateEdge(red, a);
        CreateEdge(red, b);
        CreateEdge(red, orange);
    }

    [Header("Testing variables")]
    public Vector2 size = new();

    [ContextMenu("Add Random Expansion (auto size)")]
    public void ContextAddRandom()
    {
        float sizeX = UnityEngine.Random.Range(size.x, size.y);
        AddRandomExpansionNode(sizeX);
    }

    void ConnectOrInsertBetweenAdjacent(Node hexNode, float minDistance, float maxDistance)
    {
        if (hexNode == null || hexNode.neighbors.Count < 2)
            return;

        // Get leftmost and rightmost neighbors (by angle around the hex node)
        var sortedNeighbors = hexNode.neighbors
            .OrderBy(n =>
            {
                Vector2 dir = (n.Pos - hexNode.Pos).normalized;
                return Mathf.Atan2(dir.y, dir.x);
            }).ToList();

        for (int i = 0; i < sortedNeighbors.Count - 1; i++)
        {
            Node left = sortedNeighbors[i];
            Node right = sortedNeighbors[i + 1];
            float dist = Vector2.Distance(left.Pos, right.Pos);

            if (dist < minDistance)
            {
                if (!EdgeExists(left, right))
                    CreateEdge(left, right);
            }
            else if (dist > maxDistance)
            {
                Vector2 mid = (left.Pos + right.Pos) * 0.5f;
                Node red = CreateNode(mid, 0.5f, Color.red, 0, false, true);
                CreateEdge(red, left);
                CreateEdge(red, right);
            }
        }
    }

}
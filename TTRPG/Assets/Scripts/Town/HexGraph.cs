using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static TownManager;

public class HexGraph : MonoBehaviour
{
    public TownManager town;
    public int baseNodes = 6;
    public float baseRadius = 2f;
    public Vector2 ringSpacingRandom;
    public float ringSpacing = 2f;
    public int maxRings = 5;
    public bool autoExpand = true;
    public float expandDelay = 1f;
    public Vector2 additionalNodes = new Vector2(-1, 3);

    private int currentRing = 0;
    private float timer;

    private List<TownManager.Node> currentRingNodes = new();
    private int currentRingNodeCount = 0;
    private float currentRingRadius = 0f;

    public static HexGraph instance;
    public event Action<TownManager.Node, bool> OnNodeCreate;
    public event Action OnNewRingCreate;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (town == null)
            town = TownManager.instance;

        if (town == null)
        {
            Debug.LogError("HexGraph: TownManager not found.");
            enabled = false;
            return;
        }

        GenerateRing(baseNodes, baseRadius);
    }

    void Update()
    {
        if (!autoExpand) return;

        timer += Time.deltaTime;
        if (timer >= expandDelay)
        {
            timer = 0;
            ExpandNextRing();
        }
    }

    void ExpandNextRing()
    {
        if (currentRing >= maxRings) return;

        currentRing++;

        // Stable growth in early rings, randomness in later ones
        int randomOffset = 0;
        if (currentRing > 2)
            randomOffset = UnityEngine.Random.Range(
                Mathf.FloorToInt(additionalNodes.x),
                Mathf.FloorToInt(additionalNodes.y) + 1);

        int nodeCount = Mathf.Max(3, baseNodes + currentRing + randomOffset);

        ringSpacing = UnityEngine.Random.Range(ringSpacingRandom.x, ringSpacingRandom.y);
        float radius = baseRadius + Mathf.Pow(currentRing, 1.5f) * ringSpacing;

        GenerateRing(nodeCount, radius);
    }

    void GenerateRing(int nodeCount, float radius)
    {
        List<TownManager.Node> newRingNodes = new();
        float step = 360f / nodeCount;

        for (int i = 0; i < nodeCount; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            var node = town.CreateNode(pos, 1f, new Color(1f, 0.5f, 0f), town.maxConnections, false, false);
            newRingNodes.Add(node);
        }

        // Connect adjacent nodes to form the ring
        for (int i = 0; i < newRingNodes.Count; i++)
        {
            var a = newRingNodes[i];
            var b = newRingNodes[(i + 1) % newRingNodes.Count];
            town.CreateEdge(a, b);
        }

        // Connect each new node to two closest from previous rings
        foreach (var newNode in newRingNodes)
        {
            var closestNodes = FindClosestNodesFromPreviousRings(newNode, town, 2);
            foreach (var c in closestNodes)
                town.CreateEdge(newNode, c);
        }
    }

    public void CreateNextNodeOnCurrentRing()
    {
        bool newRing = false;

        if (currentRing >= maxRings) return;

        // Start a new ring if needed
        if (currentRingNodes.Count == 0)
        {
            currentRing++;
            int randomOffset = 0;
            if (currentRing > 2)
                randomOffset = UnityEngine.Random.Range(
                    Mathf.FloorToInt(additionalNodes.x),
                    Mathf.FloorToInt(additionalNodes.y) + 1);

            currentRingNodeCount = Mathf.Max(3, baseNodes + currentRing + randomOffset);
            currentRingRadius = baseRadius + Mathf.Pow(currentRing, 1.5f) * ringSpacing;
            newRing = true;

            OnNewRingCreate?.Invoke();
        }

        int i = currentRingNodes.Count;
        if (i >= currentRingNodeCount)
        {
            // close the ring before clearing
            var firstNode = currentRingNodes[0];
            var lastNode = currentRingNodes[currentRingNodes.Count - 1];
            town.CreateEdge(lastNode, firstNode);

            currentRingNodes.Clear();
            return;
        }

        // Create the next node along the circle
        float step = 360f / currentRingNodeCount;
        float angle = step * i * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentRingRadius;

        var node = town.CreateNode(pos, 1f, new Color(1f, 0.5f, 0f), town.maxConnections, false, false);
        currentRingNodes.Add(node);

        // connect to previous node
        if (currentRingNodes.Count > 1)
        {
            var a = currentRingNodes[currentRingNodes.Count - 2];
            var b = currentRingNodes[currentRingNodes.Count - 1];
            town.CreateEdge(a, b);
        }

        // connect to two closest nodes from previous rings
        var closestNodes = FindClosestNodesFromPreviousRings(node, town, 2);
        foreach (var c in closestNodes)
            town.CreateEdge(node, c);
        
        OnNodeCreate?.Invoke(node, newRing);
        //StartCoroutine(OnNodeCreateWait(node, newRing));
    }

    private IEnumerator OnNodeCreateWait(TownManager.Node node, bool newRing)
    {
        yield return new WaitForSeconds(0.05f);

        OnNodeCreate?.Invoke(node, newRing);
    }

    List<TownManager.Node> FindClosestNodesFromPreviousRings(TownManager.Node node, TownManager town, int count)
    {
        var allNodes = GetNodesExceptNewest(town, node);
        List<TownManager.Node> sorted = new List<TownManager.Node>(allNodes);
        sorted.Sort((a, b) => Vector2.Distance(node.Pos, a.Pos).CompareTo(Vector2.Distance(node.Pos, b.Pos)));
        return sorted.GetRange(0, Mathf.Min(count, sorted.Count));
    }

    IEnumerable<TownManager.Node> GetNodesExceptNewest(TownManager town, TownManager.Node exclude)
    {
        foreach (var n in town.GetType().GetField("nodes",
                 System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                 .GetValue(town) as List<TownManager.Node>)
        {
            if (n != exclude)
                yield return n;
        }
    }
}

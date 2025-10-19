using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public readonly struct CityNode
{
    public readonly int id;
    public readonly Vector2 pos;
    public CityNode(int id, Vector2 pos) { this.id = id; this.pos = pos; }
}

[Serializable]
public class CityNodeData
{
    public CityNode node;
    public List<int> neighbors = new List<int>();
    public int ringIndex;
    public CityNodeData(CityNode n, int ring) { node = n; ringIndex = ring; }
    public void ConnectTo(int other)
    {
        if (other == node.id) return;
        if (!neighbors.Contains(other)) neighbors.Add(other);
    }
}

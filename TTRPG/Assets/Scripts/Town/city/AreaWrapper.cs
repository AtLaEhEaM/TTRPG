using System.Collections.Generic;
using UnityEngine;

public class AreaWrapper : MonoBehaviour
{
    [System.Serializable]
    public class AreaData
    {
        public List<float> corners = new List<float>();
        public CityAreaType type;
    }

    public List<AreaData> areas = new List<AreaData>();

    public void AddArea(IReadOnlyList<Vector2> corners, CityAreaType type)
    {
        var flat = new List<float>(corners.Count * 2);
        foreach (var v in corners)
        {
            flat.Add(v.x);
            flat.Add(v.y);
        }
        areas.Add(new AreaData { corners = flat, type = type });
    }

    public void ClearAreas() => areas.Clear();
}


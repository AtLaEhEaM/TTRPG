using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildBuildings : MonoBehaviour
{
    public CreateArea createArea;
    public List<BuildingWrapper> buildingsAccordingToArea;
    public float buildDelay = 0.5f;
    public float buildTimePerBuilding = 1.5f;

    private void Start()
    {
        createArea.OnAreaCreated += StartCreatingBuildings;
    }

    public void StartCreatingBuildings(AreaWrapper wrapper)
    {
        StartCoroutine(CreateBuildings(wrapper));
    }

    private IEnumerator CreateBuildings(AreaWrapper wrapper)
    {
        var buildingData = buildingsAccordingToArea.FirstOrDefault(b => b.areaType == wrapper.areaType);
        if (buildingData == null)
        {
            Debug.LogWarning($"No building data found for area type {wrapper.areaType}");
            yield break;
        }

        int numBuildings = Mathf.CeilToInt(wrapper.areaSize / 10f);
        if (wrapper.areaType == AreaType.Temple)
            numBuildings = 1;
        if (wrapper.areaType == AreaType.NobleHousing)
            numBuildings /= 2;

            for (int i = 0; i < numBuildings; i++)
        {
            yield return new WaitForSeconds(buildDelay);

            var pos = GetRandomPointInsidePolygon(wrapper.points);
            var sprite = buildingData.sprites[Random.Range(0, buildingData.sprites.Length)];

            GameObject building = new GameObject($"Building_{wrapper.areaType}_{i}");
            building.transform.position = pos;
            building.transform.localScale = buildingData.spriteScale;

            var sr = building.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            building.transform.localScale = new Vector3(10f, 10f, 10f);

            yield return new WaitForSeconds(buildTimePerBuilding);
        }
    }

    private Vector2 GetRandomPointInsidePolygon(List<Vector2> polygon, float padding = 1.5f, int maxAttempts = 20)
    {
        if (polygon == null || polygon.Count < 3)
            return Vector2.zero;

        // Compute centroid as the main bias center
        Vector2 centroid = Vector2.zero;
        foreach (var p in polygon)
            centroid += p;
        centroid /= polygon.Count;

        // Find polygon bounds for random sampling
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (var p in polygon)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        Vector2 candidate = centroid;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Random sample within bounding box
            Vector2 testPoint = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            // Bias it slightly toward centroid
            testPoint = Vector2.Lerp(testPoint, centroid, 0.35f);

            if (IsPointInsidePolygon(polygon, testPoint))
            {
                // If it's too close to edge, reject and retry
                float distToEdge = DistanceToPolygonEdge(polygon, testPoint);
                if (distToEdge > padding)
                {
                    candidate = testPoint;
                    break;
                }
            }
        }

        return candidate;
    }

    private bool IsPointInsidePolygon(List<Vector2> polygon, Vector2 point)
    {
        bool inside = false;
        int count = polygon.Count;
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) /
                 (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private float DistanceToPolygonEdge(List<Vector2> polygon, Vector2 point)
    {
        float minDist = float.MaxValue;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count];
            Vector2 closest = ClosestPointOnSegment(a, b, point);
            float dist = Vector2.Distance(point, closest);
            if (dist < minDist) minDist = dist;
        }
        return minDist;
    }

    private Vector2 ClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }


}

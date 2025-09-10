using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    private Dictionary<ResourceType, ResourceData> resources = new();

    private void Awake()
    {
        if (instance == null) instance = this;

        // Initialize resources (example starting values)
        resources[ResourceType.Gold] = new ResourceData(100, 10, 0);   // 10 gold/hour
        resources[ResourceType.Food] = new ResourceData(200, 5, 2);    // 5 gain, 2 loss
        resources[ResourceType.Wood] = new ResourceData(0, 3, 0);
        resources[ResourceType.Stone] = new ResourceData(0, 1, 0);
    }

    private void Update()
    {
        UpdateResources(Time.deltaTime);
    }

    private void UpdateResources(float deltaTime)
    {
        float hours = deltaTime / 3600f; // convert seconds to hours

        foreach (var resource in resources)
        {
            var data = resource.Value;
            float netGain = data.values.y - data.values.z;
            data.values.x += netGain * hours;
        }
    }

    public float GetResource(ResourceType type)
    {
        return resources[type].values.x;
    }

    public void AddResource(ResourceType type, float amount)
    {
        resources[type].values.x += amount;
    }

    public bool SpendResource(ResourceType type, float amount)
    {
        if (resources[type].values.x >= amount)
        {
            resources[type].values.x -= amount;
            return true;
        }
        return false;
    }

    
}


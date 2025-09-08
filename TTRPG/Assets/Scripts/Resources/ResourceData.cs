using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Gold,
    Food,
    Wood,
    Stone
}

[System.Serializable]
public class ResourceData
{
    // x = current
    // y = gain per hour
    // z = loss per hour
    public Vector3 values;

    public ResourceData(float current, float gain, float loss)
    {
        values = new Vector3(current, gain, loss);
    }

    public ResourceData()
    {
        values = Vector3.zero;
    }
}

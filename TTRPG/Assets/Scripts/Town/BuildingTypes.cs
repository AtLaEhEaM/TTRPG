using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTypes : MonoBehaviour
{
    
}

[Serializable]
public class BuildingWrapper
{
    public AreaType areaType;
    public Sprite[] sprites;
    public Vector2 spriteScale = new Vector2(1f, 5f);
}

[Serializable]
public enum AreaType
{
    Farmland,
    Industrial,
    Temple,
    Market,
    CommonHousing,
    NobleHousing
}

[Serializable]
public class AreaWrapper
{
    public AreaType areaType;
    public List<Vector2> points;
    public float areaSize;
}
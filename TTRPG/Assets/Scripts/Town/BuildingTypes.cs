using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTypes : MonoBehaviour
{
    
}

[Serializable]
public class BuildingWrapper
{
    public CityAreaType areaType;
    public Sprite[] sprites;
    public Vector2 spriteScale = new Vector2(1f, 5f);
}

[Serializable]
public enum CityAreaType
{
    Farmland,
    Industrial,
    Temple,
    Market,
    CommonHousing,
    NobleHousing
}

[Serializable]
public class AreaWrapperr
{
    public CityAreaType areaType;
    public List<Vector2> points;
    public float areaSize;
}
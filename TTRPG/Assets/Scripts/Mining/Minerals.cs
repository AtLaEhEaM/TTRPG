using System;
using UnityEngine;
using System.Collections.Generic;

public class Minerals : MiningManager
{
    
}

[Serializable]
public class MiningTrip
{
    public int workers;
    public long timeStarted;
    public int caveLayer;
    public List<MineralData> mineralsMineableList;
}

public enum MineralTypes
{
    stone, 
    iron,
    copper,
}

[Serializable]
public class MineralData
{
    public MineralTypes type;
    public int layerFoundAt;
    public int sellPrice;
    [Range(0, 100)]public int rarityChance;
}

[Serializable]
public class MineralSaveData
{
    public MineralTypes type;
    public int amount;
}
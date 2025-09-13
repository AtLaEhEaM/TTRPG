using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public List<LogBoxData> logBoxDataList = new List<LogBoxData>();
    //public ResourceData resourceData = new ResourceData();
    public List<LoadFoodData> foodDataList = new List<LoadFoodData>();

    public int goldCount;
    public int foodCount;
    public int workerCount;
}


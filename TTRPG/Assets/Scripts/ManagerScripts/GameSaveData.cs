using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public List<LogBoxData> logBoxDataList = new();
    public List<LoadFoodData> foodDataList = new();
    public EconomyData economyData = new();
    public List<SavedEventData> scheduledEvents = new();
    public List<MineralSaveData> mineralSaveDataList = new();
}

[Serializable]
public class SavedEventData
{
    public string id;
    public long finishTimeBinary;
}

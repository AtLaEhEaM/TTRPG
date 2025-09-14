using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public List<LogBoxData> logBoxDataList = new List<LogBoxData>();
    public List<LoadFoodData> foodDataList = new List<LoadFoodData>();
    public EconomyData economyData = new();
    public List<SavedEventData> scheduledEvents = new List<SavedEventData>();
}

[Serializable]
public class SavedEventData
{
    public string id;
    public long finishTimeBinary;
}

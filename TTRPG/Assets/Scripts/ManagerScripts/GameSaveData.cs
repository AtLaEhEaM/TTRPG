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
    public List<MiningTrip> miningTripsList = new();
    public PopulationData populationData = new();
    public int maxMiningLayer = 0;
}

[Serializable]
public class SavedEventData
{
    public string id;
    public long finishTimeBinary;
}

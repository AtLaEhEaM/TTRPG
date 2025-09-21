using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Mining : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider workersSlider;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button confirmButton;

    [Header("Settings")]
    [SerializeField] private int stepSize = 1;

    [SerializeField] private int workers;
    [SerializeField] private int gold;
    [SerializeField] private int food;
    [SerializeField] private int maxTime;

    private void Start()
    {
        workersSlider.wholeNumbers = true;
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;

        GameManager.instance.economyManager.OnEconomyUpdate += SetSliderMinMax;
        confirmButton.onClick.AddListener(OnConfirm);

        workersSlider.onValueChanged.AddListener(OnWorkersChanged);
        OnWorkersChanged(workersSlider.value);
    }

    void LoadData()
    {
        SetSliderMinMax();
    }

    public void SetSliderMinMax()
    {
        workersSlider.minValue = 0;
        workersSlider.maxValue = GameSavingManager.instance.saveData.economyData.workers;

        //Debug.Log("slider max value being set: "+GameSavingManager.instance.saveData.economyData.workers);

        if (workersSlider.value < workersSlider.minValue)
            workersSlider.value = workersSlider.minValue;

        if (workersSlider.value > workersSlider.maxValue)
            workersSlider.value = workersSlider.maxValue;

        Debug.Log($"[Mining] Slider set to range {workersSlider.minValue} - {workersSlider.maxValue}, current value: {workersSlider.value}");
    }


    private void OnWorkersChanged(float value)
    {
        workers = Mathf.RoundToInt(value / stepSize) * stepSize;

        gold = workers * 10;
        food = workers * 2;
        maxTime = Mathf.Max(30, workers * 50);

        goldText.text = $"Gold: {gold}";
        foodText.text = $"Food: {food}";

        if (maxTime > 60)
            timeText.text = $"{maxTime % 60} hours";
        else
            timeText.text = $"{maxTime} minutes";
    }

    private void OnConfirm()
    {
        if (GameManager.instance.economyManager.economyData.gold < gold
            || GameManager.instance.economyManager.economyData.workers < workers
            || GameManager.instance.economyManager.economyData.food < food)
            return;

        MiningTrip trip = new MiningTrip
        {
            workers = workers,
            timeStarted = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            completionTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + maxTime,
            caveLayer = 1,
            mineralsMineableList = new List<MineralData>()
        };

        string dispTime;
        if (maxTime > 60)
            dispTime = $"{maxTime % 60} hours";
        else
            dispTime = $"{maxTime} minutes";

        GameManager.instance.economyManager.UpdateWorkers(-workers);
        LogBoxManager.instance.NewBox(LogBoxType.Mining,
            $"<color=#A52A2A>{workers}</color> sent to the mines for {dispTime}<br>" +
            $"{food} <color=#00FF00>food</color> {gold} <color=#FFFF00>gold</color> given");

        MiningManager.instance.AddTrip(trip);

        Debug.Log($"New MiningTrip added with {workers} workers, duration {maxTime}s");
    }
}


[Serializable]
public class MiningTrip
{
    public int workers;
    public long timeStarted;
    public long completionTime;
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
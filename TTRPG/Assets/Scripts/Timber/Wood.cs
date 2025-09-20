using System;
using UnityEngine;
using UnityEngine.UI;

public class Wood : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider goldSlider;
    [SerializeField] private Slider workerSlider;

    [Header("Costs & Results")]
    public int goldCost;
    public int workerCost;
    public float timeCost;
    public float amountOfWood;

    [Header("Settings")]
    [SerializeField] private Vector2 goldSliderConstraints;
    [SerializeField] private Vector2 workerSliderConstraints;
    [SerializeField] private int stepSize = 1;
    [SerializeField] private WoodType type;

    [Header("Balance Tweaks")]
    [SerializeField] private float amountMultiplier = 0.001f; 
    [SerializeField] private float timeMultiplier = 1000f;    

    private void Start()
    {
        SetupSlider(goldSlider, goldSliderConstraints);
        SetupSlider(workerSlider, workerSliderConstraints);

        var econ = GameManager.instance?.economyManager;
        if (econ != null)
            econ.OnEconomyUpdate += MaxValues;

        goldSlider.onValueChanged.AddListener(_ => UpdateValues());
        workerSlider.onValueChanged.AddListener(_ => UpdateValues());

        MaxValues();
        UpdateValues();
    }

    public void Confirm()
    {
        if (goldCost > GameManager.instance.economyManager.economyData.gold)
            return;

        LogBoxManager.instance.CreateLogBox(
            LogBoxType.TreePlantation,
            $"You send {workerCost} workers to chop {type} trees!<br>Workers may return in {Mathf.FloorToInt(timeCost)} minutes!"
        );

        int workersSent = workerCost;
        WoodType sentType = type;
        float woodSent = amountOfWood;

        TimeEventScheduler.instance.ScheduleEvent(
            $"wood_{type}_{Guid.NewGuid()}",
            TimeSpan.FromSeconds(timeCost),
            () => CompleteWoodChopping(sentType, workersSent, woodSent)
        );

        GameManager.instance.economyManager.UpdateGold(-goldCost);
        GameManager.instance.economyManager.UpdateWorkers(-workerCost);
    }

    public void CompleteWoodChopping(WoodType choppedType, int returnedWorkers, float woodGained)
    {
        GameManager.instance.economyManager.UpdateWorkers(returnedWorkers);
        GameManager.instance.economyManager.UpdateWood(Mathf.RoundToInt(woodGained));

        LogBoxManager.instance.CreateLogBox(
            LogBoxType.TreePlantation,
            $"Your {returnedWorkers} workers returned after chopping {choppedType}!<br>You gained {Mathf.RoundToInt(woodGained)} {choppedType} wood."
        );
    }

    private void SetupSlider(Slider slider, Vector2 constraints)
    {
        int min = Mathf.RoundToInt(constraints.x);
        int max = Mathf.RoundToInt(constraints.y);

        min = Mathf.CeilToInt((float)min / stepSize) * stepSize;
        max = Mathf.FloorToInt((float)max / stepSize) * stepSize;

        if (min > max) min = max = Mathf.RoundToInt(constraints.x);

        slider.wholeNumbers = true;
        slider.minValue = min;
        slider.maxValue = max;

        float current = slider.value;
        if (current < min || current > max)
            slider.SetValueWithoutNotify(Mathf.Clamp(current, min, max));
    }

    public void MaxValues()
    {
        var save = GameSavingManager.instance?.saveData;
        if (save == null) return;

        goldSliderConstraints.y = save.economyData.gold;
        workerSliderConstraints.y = save.economyData.workers;

        SetupSlider(goldSlider, goldSliderConstraints);
        SetupSlider(workerSlider, workerSliderConstraints);
    }

    private void UpdateValues()
    {
        goldCost = Mathf.RoundToInt(goldSlider.value / stepSize) * stepSize;
        workerCost = Mathf.RoundToInt(workerSlider.value / stepSize) * stepSize;

        MaxValues();

        WoodData data = GameManager.instance.woodDataList.Find(w => w.type == type);

        if (data != null && data.timeToChop > 0 && workerCost > 0 && goldCost > 0)
        {
            float typeFactor = (type == WoodType.Oak ? 2f : 3f);
            amountOfWood = ((workerCost / 2f) * (goldCost / 4f) * typeFactor) * amountMultiplier;

            timeCost = (data.timeToChop * amountOfWood / (workerCost * goldCost)) * timeMultiplier;

            timeCost = TimeAdjustmentScript.LogReduce(timeCost * 2);
        }
        else
        {
            amountOfWood = 0f;
            timeCost = 0f;
        }

        Debug.Log($"Wood: {type}, Gold: {goldCost}, Workers: {workerCost}, Time: {timeCost:F2}, Amount: {amountOfWood:F2}");
    }

    public void ChangeWoodType(string _name)
    {
        foreach (WoodType _type in Enum.GetValues(typeof(WoodType)))
        {
            if (_type.ToString() == _name)
            {
                type = _type;
                UpdateValues();
                return;
            }
            else
                continue;
        }
    }
}

[Serializable]
public class WoodData
{
    public WoodType type;
    public int purchasePrice;
    public int timeToChop;
}

public enum WoodType
{
    Oak,
    Birch,
}

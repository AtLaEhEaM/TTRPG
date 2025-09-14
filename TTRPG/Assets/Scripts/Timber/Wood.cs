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

    private void OnDestroy()
    {
        var econ = GameManager.instance?.economyManager;
        if (econ != null)
            econ.OnEconomyUpdate -= MaxValues;
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

        if (data != null && data.timeToChop != 0)
        {
            amountOfWood = (((workerCost / 2f) * (goldCost / 4f)) * (data.type == WoodType.Oak ? 4 : 8)/100);
            timeCost = ((float)data.timeToChop * amountOfWood / (workerCost * goldCost)*1000);
            timeCost = TimeAdjustmentScript.LogReduce(timeCost);
        }
        else
        {
            amountOfWood = 0f;
            timeCost = 0f;
        }

        Debug.Log($"Wood: {type}, Gold: {goldCost}, Workers: {workerCost}, Time: {timeCost}, Amount: {amountOfWood}");
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

    public WoodType AssignWoodType(string _name)
    {
        if (Enum.TryParse(_name, true, out WoodType result))
            return result;

        return WoodType.Oak;
    }
}

[Serializable]
public class WoodData
{
    public WoodType type;
    public int purchasePrice;
    public int timeToChop;
    //public int woodPerTree;
}

public enum WoodType
{
    Oak,
    Birch,
}

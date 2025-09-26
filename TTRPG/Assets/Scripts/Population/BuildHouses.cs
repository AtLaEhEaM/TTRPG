using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics.Contracts;

public class BuildHouses : MonoBehaviour
{
    public enum HouseTypes
    {
        Small, Large,
    }

    [Header("House Settings")]
    public HouseTypes houseType;
    public Vector2 houseGoldCost;   // x = small, y = large
    public Vector2 houseWoodCost;   // x = small, y = large
    public Vector2 houseStoneCost;  // x = small, y = large
    public Vector2 houseBuildTime;  // x = small, y = large (in minutes)

    [Header("UI References")]
    public Slider houseSlider;
    public TMP_Text costText;
    public Button confirmButton;

    private int houseCount;

    private void Start()
    {
        houseSlider.wholeNumbers = true;
        houseSlider.onValueChanged.AddListener(OnSliderChanged);
        confirmButton.onClick.AddListener(OnConfirm);

        GameManager.instance.economyManager.OnEconomyUpdate += SetSmallHouse;
        SetMaxSliderValues();
        OnSliderChanged(houseSlider.value);
    }

    public void SetMaxSliderValues()
    {
        float gold = GameManager.instance.economyManager.economyData.gold;
        float max;

        if (houseType == HouseTypes.Small)
            max = Mathf.FloorToInt(gold / houseGoldCost.x);
        else
            max = Mathf.FloorToInt(gold / houseGoldCost.y);

        houseSlider.maxValue = Mathf.Max(0, max);
        houseSlider.value = houseSlider.maxValue > 0 ? 1 : 0;
    }

    private void OnSliderChanged(float value)
    {
        houseCount = Mathf.RoundToInt(value);
        UpdateCostDisplay();
    }

    private void UpdateCostDisplay()
    {
        if (houseCount <= 0)
        {
            costText.text = "Select houses to build";
            return;
        }

        if (houseType == HouseTypes.Small)
        {
            int goldCost = Mathf.RoundToInt(houseGoldCost.x * houseCount);
            int woodCost = Mathf.RoundToInt(houseWoodCost.x * houseCount);
            int stoneCost = Mathf.RoundToInt(houseStoneCost.x * houseCount);
            float time = houseBuildTime.x * houseCount;

            costText.text = $"Cost: {goldCost}G, {woodCost}W, {stoneCost}S\nTime: {time:F0}s";
        }
        else
        {
            int goldCost = Mathf.RoundToInt(houseGoldCost.y * houseCount);
            int woodCost = Mathf.RoundToInt(houseWoodCost.y * houseCount);
            int stoneCost = Mathf.RoundToInt(houseStoneCost.y * houseCount);
            float time = houseBuildTime.y * houseCount;

            costText.text = $"Cost: {goldCost}G, {woodCost}W, {stoneCost}S\nTime: {time:F0}s";
        }
    }

    private void OnConfirm()
    {
        if (houseCount <= 0)
            return;

        // Calculate totals
        float goldCost, woodCost, stoneCost, totalTime;
        if (houseType == HouseTypes.Small)
        {
            goldCost = houseGoldCost.x * houseCount;
            woodCost = houseWoodCost.x * houseCount;
            stoneCost = houseStoneCost.x * houseCount;
            totalTime = houseBuildTime.x * houseCount;
        }
        else
        {
            goldCost = houseGoldCost.y * houseCount;
            woodCost = houseWoodCost.y * houseCount;
            stoneCost = houseStoneCost.y * houseCount;
            totalTime = houseBuildTime.y * houseCount;
        }

        // Deduct resources
        GameManager.instance.economyManager.UpdateGold((int)-goldCost);
        GameManager.instance.economyManager.UpdateMinerals(MineralTypes.stone, (int)stoneCost);
        GameManager.instance.economyManager.UpdateWood((int)woodCost);

        // Schedule the build
        string id = $"house_{houseType}_{houseCount}_{Guid.NewGuid()}";
        TimeEventScheduler.instance.ScheduleEvent(
            id,
            TimeSpan.FromSeconds(totalTime),
            () => CompleteBuild(houseCount, houseType)
        );

        LogBoxManager.instance.NewBox(LogBoxType.Construction,
            $"Started construction of {houseCount} {houseType} houses.<br>" +
            $"Construction will be completed in {totalTime} minutes!"
            );

        Debug.Log($" Scheduled {houseCount} {houseType} house(s). Build time: {totalTime:F0}m");
    }

    private void CompleteBuild(int count, HouseTypes type)
    {
        //if(type == HouseTypes.Small)

        // else

    }

    public void UpdateSlider()
    {
        SetMaxSliderValues();       
        houseCount = Mathf.RoundToInt(houseSlider.value);
        UpdateCostDisplay();          
    }

    public void SetSmallHouse()
    {
        houseType = HouseTypes.Small;
        UpdateSlider();
    }

    public void SetLargeHouse()
    {
        houseType = HouseTypes.Large;
        UpdateSlider();
    }

}

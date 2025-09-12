using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GrowCrops : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Slider costSlider;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI cropText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Slider Settings")]
    [SerializeField] private Vector2 sliderConstraints;
    [SerializeField] private int stepSize = 5;

    [Header("Crop Settings")]
    [SerializeField] private FoodTypes foodTypes;
    public int gold { get; private set; }
    public int crop { get; private set; }
    public float timeNeeded { get; private set; }

    private void Awake()
    {
        if (costSlider != null) costSlider.wholeNumbers = true;
    }

    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
        GameManager.instance.economyManager.OnGoldUpdate += LoadData;
        if (costSlider != null) costSlider.onValueChanged.AddListener(SnapSlider);
    }

    private void OnDestroy()
    {
        if (GameSavingManager.instance != null) GameSavingManager.instance.OnSaveDataLoadedEvent -= LoadData;
        if (costSlider != null) costSlider.onValueChanged.RemoveListener(SnapSlider);
    }

    private void LoadData()
    {
        sliderConstraints.x = 10f;
        sliderConstraints.y = GameSavingManager.instance.saveData.goldCount;
        //Debug.Log("hello world");

        ApplySliderConstraintsAndSnap();
    }

    private void ApplySliderConstraintsAndSnap()
    {
        Debug.Log("updated slider ");
        int min = Mathf.RoundToInt(sliderConstraints.x);
        int max = Mathf.RoundToInt(sliderConstraints.y);
        min = Mathf.CeilToInt((float)min / stepSize) * stepSize;
        max = Mathf.FloorToInt((float)max / stepSize) * stepSize;
        if (min > max) { min = max = Mathf.RoundToInt(sliderConstraints.x); }

        costSlider.minValue = min;
        costSlider.maxValue = max;
        float clamped = Mathf.Clamp(costSlider.value, costSlider.minValue, costSlider.maxValue);
        costSlider.SetValueWithoutNotify(clamped);
        SnapSlider(costSlider.value);
    }

    private void SnapSlider(float value)
    {
        int snappedValue = Mathf.RoundToInt(value / stepSize) * stepSize;
        snappedValue = Mathf.Clamp(snappedValue, Mathf.RoundToInt(costSlider.minValue), Mathf.RoundToInt(costSlider.maxValue));
        costSlider.SetValueWithoutNotify(snappedValue);

        var foodList = GameManager.instance.foodDataList;
        for (int i = 0; i < foodList.Count; i++)
        {
            var food = foodList[i];
            if (food.type == foodTypes)
            {
                gold = snappedValue;
                crop = food.priceToPlant > 0 ? snappedValue / food.priceToPlant : 0;
                timeNeeded = crop * food.timeToGrow;
                UpdateTexts();
                return;
            }
        }

        gold = snappedValue;
        crop = 0;
        timeNeeded = 0f;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (goldText != null) goldText.text = $"Gold: {gold}";
        if (cropText != null) cropText.text = $"Crops: {crop}";
        float dispTime = DisplayTime(timeNeeded);
        if (timeText != null) timeText.text = $"Time: {dispTime:F1}";
    }

    public void SetFoodType(string foodType)
    {
        if (Enum.TryParse(foodType, true, out FoodTypes parsedType))
        {
            foodTypes = parsedType;
            LoadData();
        }
    }

    public float DisplayTime(float dispTime)
    {
        dispTime = timeNeeded / 60f;

        if (dispTime < 1)
            return dispTime = 1;
        else
            return dispTime;
    }

    public void Confirm()
    {
        if (GameManager.instance.economyManager.gold < 10) // issue   
            return;

        float _time = TimeAdjustmentScript.LogReduce(timeNeeded);

        GameManager.instance.economyManager.UpdateGold(-gold);

        LogBoxManager.instance.NewFarmerBox(LogBoxType.Farm, true, foodTypes, crop, _time);

        GameManager.instance.cropGrowthManager.GrowFood(foodTypes, crop, _time);

        GameSavingManager.instance.SaveGame();
    }
}

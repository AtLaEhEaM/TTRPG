using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GrowCrops : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider costSlider;
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

    public float reducedTime;

    private void Awake()
    {
        if (costSlider != null) costSlider.wholeNumbers = true;
    }

    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
        GameManager.instance.economyManager.OnEconomyUpdate += LoadData;
        if (costSlider != null) costSlider.onValueChanged.AddListener(SnapSlider);
    }

    private void OnDestroy()
    {
        if (GameSavingManager.instance != null)
            GameSavingManager.instance.OnSaveDataLoadedEvent -= LoadData;

        if (costSlider != null)
            costSlider.onValueChanged.RemoveListener(SnapSlider);
    }

    private void LoadData()
    {
        sliderConstraints.x = 10f;
        sliderConstraints.y = GameSavingManager.instance.saveData.economyData.gold;

        ApplySliderConstraintsAndSnap();
    }

    private void ApplySliderConstraintsAndSnap()
    {
        int min = Mathf.RoundToInt(sliderConstraints.x);
        int max = Mathf.RoundToInt(sliderConstraints.y);

        min = Mathf.CeilToInt((float)min / stepSize) * stepSize;
        max = Mathf.FloorToInt((float)max / stepSize) * stepSize;

        if (min > max)
            min = max = Mathf.RoundToInt(sliderConstraints.x);

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

                // Always use log-reduced time
                float rawTime = crop * food.timeToGrow;
                timeNeeded = TimeAdjustmentScript.LogReduce(rawTime);

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

        reducedTime = TimeAdjustmentScript.LogReduce(timeNeeded);
        
        if (timeText != null) timeText.text = $"Time: {reducedTime:F1}m";
    }

    public void SetFoodType(string foodType)
    {
        if (Enum.TryParse(foodType, true, out FoodTypes parsedType))
        {
            foodTypes = parsedType;
            LoadData();
        }
    }

    public void Confirm()
    {
        if (GameManager.instance.economyManager.economyData.gold < gold)
            return;

        GameManager.instance.economyManager.UpdateGold(-gold);

        // use the already log-reduced timeNeeded
        LogBoxManager.instance.NewFarmerBox(true, foodTypes, crop, reducedTime);
        GameManager.instance.cropGrowthManager.GrowFood(foodTypes, crop, reducedTime);

        GameSavingManager.instance.SaveGame();
    }
}

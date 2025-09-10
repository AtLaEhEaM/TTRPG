using System;
using System.Collections;
using UnityEngine;

public class FoodGrowthManager : MonoBehaviour
{
    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    private void OnDestroy()
    {
        if (GameSavingManager.instance != null)
            GameSavingManager.instance.OnSaveDataLoadedEvent -= LoadData;
    }

    // Called after save data is loaded
    public void LoadData()
    {
        foreach (var food in GameSavingManager.instance.saveData.foodDataList)
        {
            // Resume timers for all saved crops
            StartCoroutine(GrowTimer(food));
        }
    }

    // Called when planting new crops
    public void GrowFood(FoodTypes type, int amount, float timeToGrow)
    {
        var foodData = new LoadFoodData
        {
            foodType = type,
            amount = amount,
            remainingTime = timeToGrow
        };

        GameSavingManager.instance.saveData.foodDataList.Add(foodData);

        StartCoroutine(GrowTimer(foodData));
    }

    private IEnumerator GrowTimer(LoadFoodData foodData)
    {
        while (foodData.remainingTime > 0f)
        {
            foodData.remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Crop finished growing
        foodData.remainingTime = 0f;

        LogBoxManager.instance.NewFarmerBox(
            LogBoxType.Farm,
            false,
            foodData.foodType,
            foodData.amount,
            foodData.remainingTime
        );

        GameManager.instance.economyManager.UpdateFood(foodData.amount);

        GameSavingManager.instance.saveData.foodDataList.Remove(foodData);
        GameSavingManager.instance.SaveGame();
    }
}

using System;
using System.Threading;
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

    // When loading game, resume all crops
    private void LoadData()
    {
        foreach (var food in GameSavingManager.instance.saveData.foodDataList)
        {
            TimeEventScheduler.instance.ResumeEvent(
                $"food_{food.foodType}_{Guid.NewGuid()}",
                DateTime.UtcNow.AddSeconds(food.remainingTime).ToBinary(),
                () => CompleteFood(food)
            );
        }
    }

    // Plant new food crop
    public void GrowFood(FoodTypes type, int amount, float timeToGrow)
    {
        var foodData = new LoadFoodData
        {
            foodType = type,
            amount = amount,
            remainingTime = timeToGrow
        };

        GameSavingManager.instance.saveData.foodDataList.Add(foodData);

        TimeEventScheduler.instance.ScheduleEvent(
            $"food_{type}_{Guid.NewGuid()}",
            TimeSpan.FromSeconds(timeToGrow),
            () => CompleteFood(foodData)
        );

        GameSavingManager.instance.SaveGame();
    }

    private void CompleteFood(LoadFoodData foodData)
    {
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

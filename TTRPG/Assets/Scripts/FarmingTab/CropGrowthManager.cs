using System;
using System.Linq;
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

    private void LoadData()
    {
        var savedFoods = GameSavingManager.instance.saveData.foodDataList;
        if (savedFoods == null || savedFoods.Count == 0)
            return;

        var foodsCopy = savedFoods.ToList();

        foreach (var food in foodsCopy)
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
            false,
            foodData.foodType,
            foodData.amount,
            0f 
        );


        GameManager.instance.economyManager.UpdateFood(foodData.amount);

        GameSavingManager.instance.saveData.foodDataList.Remove(foodData);
        GameSavingManager.instance.SaveGame();
    }
}

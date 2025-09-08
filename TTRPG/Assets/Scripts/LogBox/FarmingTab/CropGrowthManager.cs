using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class FoodGrowthManager : MonoBehaviour
{
    public event Action OnFoodUpdate;

    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    public void LoadData()
    {
        for (int i = 0; i< GameSavingManager.instance.saveData.foodDataList.Count; i++)
        {
            StartCoroutine(GrowTimer(GameSavingManager.instance.saveData.foodDataList[i].foodType,
                                     GameSavingManager.instance.saveData.foodDataList[i].amount,
                                     GameSavingManager.instance.saveData.foodDataList[i].remainingTime));
        }
    }

    public void GrowFood(FoodTypes type, int amount, float timeToGrow)
    {
        StartCoroutine(GrowTimer(type, amount, timeToGrow));
    }

    private IEnumerator GrowTimer(FoodTypes type, int amount, float timeToGrow)
    {
        FoodData foodData = new FoodData();
        foodData.foodType = type;
        foodData.amount = amount;
        foodData.remainingTime = timeToGrow;

        GameSavingManager.instance.saveData.foodDataList.Add(foodData);

        while (foodData.remainingTime > 0f)
        {
            foodData.remainingTime -= Time.deltaTime;

            yield return null; 
        }

        foodData.remainingTime = 0f;

        LogBoxManager.instance.NewFarmerBox(LogBoxType.Farm, 0, false, type, amount, timeToGrow);

        OnFoodUpdate?.Invoke();

        GameSavingManager.instance.saveData.foodDataList.Remove(foodData);
        GameSavingManager.instance.saveData.foodCount += foodData.amount;
        GameSavingManager.instance.SaveGame();
    }

}

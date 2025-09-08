using System;
using UnityEngine;

public class PlantCrops : MonoBehaviour
{
    public FoodTypes foodType;
    public int amount;
    public float timer;

    public void PlantCrop()
    {
        GameManager.instance.cropGrowthManager.GrowFood(foodType, amount, timer);
    }
}

public enum FoodTypes
{
    Wheat,
    Rice,
    Barley,
}

[Serializable]
public class FoodData
{
    public FoodTypes foodType;
    public int amount;
    public float remainingTime; //to add a timer on crops for how long they will take to grow 
}

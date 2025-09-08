using System;
using UnityEngine;

[Serializable]
public class FarmerLogs
{
    private string finalText;

    public string FarmerResultString(FoodTypes type, int amount)
    {
        int foodValue = ConvertToFood(type, amount);
        int total = foodValue;

        string mood = GetHarvestMood(total);
        //for testing rn
        bool isPositive = true;

        string sign = isPositive ? "+" : "-";
        string color = isPositive ? "#00FF00" : "#FF0000";
        

        return finalText =
            $"Your <color={color}>Farmers</color> celebrate {mood}!<br>" +
            $"They harvested {amount} {type}, worth " +
            $"<color=#FFD700>{sign}{foodValue} Food</color>";
    }


    public string StartPlantationStrings(FoodTypes type, int amount, float time)
    {
        return finalText = "<color=#00FF00>Farmers </color>plant " + amount + " " + type + " <br>harvest may be ready in " +
            (Mathf.FloorToInt(time)) + " minutes";
    }

    private string GetHarvestMood(int foodAmount)
    {
        if (foodAmount < 20) return "a meager harvest";
        if (foodAmount < 40) return "a moderate harvest";
        if (foodAmount < 80) return "a plentiful harvest";
        return "a bountiful harvest";
    }

    private int ConvertToFood(FoodTypes type, int amount)
    {
        switch (type)
        {
            case FoodTypes.Wheat: return amount/5;        // 5 Wheat = 1 Food
            case FoodTypes.Rice: return amount/4;         // 4 Rice = 1 Food
            case FoodTypes.Barley: return amount / 3;
            //case FoodTypes.Meat: return amount * 2;     // maybe Meat is worth more?
            //case FoodTypes.Wood: return 0;              // wood is not food at all
            default: return 0;
        }
    }


}

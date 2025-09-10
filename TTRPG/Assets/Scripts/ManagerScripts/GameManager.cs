using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //scripts references here
    public FoodGrowthManager cropGrowthManager;
    public GameEconomyManager economyManager;
    public List<FoodData> foodDataList;

    public void Awake()
    {
        instance = this;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //scripts references here
    public FoodGrowthManager cropGrowthManager;

    public void Awake()
    {
        instance = this;
    }
}

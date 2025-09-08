using System;
using TMPro;
using UnityEngine;

public class LoadResources : MonoBehaviour
{
    public TextMeshProUGUI[] amountDisplay;

    public event Action OnGoldUpdate;

    void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
        GameManager.instance.cropGrowthManager.OnFoodUpdate += LoadData;
    }

    public void LoadData()
    {
        amountDisplay[0].text = GameSavingManager.instance.saveData.goldCount.ToString();
        amountDisplay[1].text = GameSavingManager.instance.saveData.foodCount.ToString();
        //amountDisplay[0].text = GameSavingManager.instance.saveData.goldCount.ToString();
        //amountDisplay[0].text = GameSavingManager.instance.saveData.goldCount.ToString();

    }
}

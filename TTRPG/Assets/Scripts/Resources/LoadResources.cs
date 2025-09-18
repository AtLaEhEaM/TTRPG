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

        GameManager.instance.economyManager.OnEconomyUpdate += LoadData;
    }

    public void LoadData()
    {
        amountDisplay[0].text = GameSavingManager.instance.saveData.economyData.gold.ToString();
        amountDisplay[1].text = GameSavingManager.instance.saveData.economyData.food.ToString();
        amountDisplay[2].text = GameSavingManager.instance.saveData.economyData.wood.ToString();
        amountDisplay[3].text = GameSavingManager.instance.saveData.economyData.workers.ToString();
        //amountDisplay[4].text = GameSavingManager.instance.saveData.mineralSaveDataList;

        int i = 4;
        int j = 0;
        foreach(var resource in GameSavingManager.instance.saveData.mineralSaveDataList)
        {
            amountDisplay[i].text = resource.amount.ToString();
            i++;
            j++;
        }

    }
}

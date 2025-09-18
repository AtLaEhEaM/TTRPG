using System;
using UnityEngine;

public class GameEconomyManager : MonoBehaviour
{
    public EconomyData economyData = new();

    public event Action OnEconomyUpdate;
    public event Action OnMineralsUpdate;


    public void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        economyData = GameSavingManager.instance.saveData.economyData;
        OnEconomyUpdate?.Invoke();
    }



    public void UpdateGold(int _gold)
    {
        GameSavingManager.instance.saveData.economyData.gold += _gold;

        GameSavingManager.instance.SaveGame();

        OnEconomyUpdate?.Invoke();
    }

    public void UpdateFood(int _food)
    {
        GameSavingManager.instance.saveData.economyData.food += _food;

        GameSavingManager.instance.SaveGame();

        OnEconomyUpdate?.Invoke();
    }

    public void UpdateMinerals(MineralTypes type, int amount)
    {
        var list = GameSavingManager.instance.saveData.mineralSaveDataList;
        Debug.Log("hello from the abyssss1");


        var item = list.Find(m => m.type == type);
        if (item != null)
        {
            Debug.Log("hello from the abyssss2");

            item.amount += amount;
        }
        else
        {
            Debug.Log("hello from the abyssss");
            list.Add(new MineralSaveData { type = type, amount = amount });
        }

        GameSavingManager.instance.SaveGame();
        OnMineralsUpdate?.Invoke();
    }

    public void UpdateWood(int _wood)
    {
        GameSavingManager.instance.saveData.economyData.wood += _wood;

        GameSavingManager.instance.SaveGame();

        OnEconomyUpdate?.Invoke();
    }

    public void UpdateWorkers(int _workers)
    {
        GameSavingManager.instance.saveData.economyData.workers += _workers;
        
        GameSavingManager.instance.SaveGame();
        
        OnEconomyUpdate?.Invoke();
    }
}

[Serializable]
public class EconomyData
{
    public int gold;
    public int food;
    public int wood;
    public int workers;
}


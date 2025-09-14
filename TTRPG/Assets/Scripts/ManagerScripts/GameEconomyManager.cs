using System;
using UnityEngine;

public class GameEconomyManager : MonoBehaviour
{
    public EconomyData economyData = new();

    public event Action OnEconomyUpdate;

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
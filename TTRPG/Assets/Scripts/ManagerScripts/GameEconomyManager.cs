using System;
using UnityEngine;

public class GameEconomyManager : MonoBehaviour
{
    public int gold;
    public int food;

    public event Action OnGoldUpdate;
    public event Action OnFoodUpdate;

    public void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        gold = GameSavingManager.instance.saveData.goldCount;
        food = GameSavingManager.instance.saveData.foodCount;
    }

    public void UpdateGold(int _gold)
    {
        gold += _gold;

        SaveResources();

        OnGoldUpdate?.Invoke();
    }

    public void UpdateFood(int _food)
    {
        food += _food;

        SaveResources();

        OnFoodUpdate?.Invoke();
    }

    public void SaveResources()
    {
        GameSavingManager.instance.saveData.goldCount = gold;
        GameSavingManager.instance.saveData.foodCount = food;

        GameSavingManager.instance.SaveGame();
    }
}

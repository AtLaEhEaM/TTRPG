using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [Header("Population Settings")]
    public int popIncreasePerMin;
    public float updateEveryXseconds = 60f;

    [Header("Runtime State")]
    int currentMin;
    float seconds;
    bool maxPop = false;
    bool pingSent = false;

    [Header("Debug / Testing")]
    public bool testPing = false;

    [Header("References")]
    public static PopulationManager instance;
    public event Action OnPopUpdate;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pingSent = false;
        currentMin = DateTime.Now.Minute;

        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        int lastMin;

        if (string.IsNullOrEmpty(GameSavingManager.instance.saveData.lastSavedTime))
        {
            lastMin = DateTime.Now.Minute;
        }
        else
        {
            lastMin = int.Parse(GameSavingManager.instance.saveData.lastSavedTime);
        }

        if (lastMin < currentMin)
        {
            int loopMissedMins = (lastMin - currentMin) / 5;

            if (loopMissedMins < 1)
                return;

            for(int i = 0; i < loopMissedMins; i++)
            {
                PopulationIncrease(popIncreasePerMin);
            }
        }
    }

    public void Update()
    {
        seconds += Time.deltaTime;

        if (seconds > updateEveryXseconds)
        {
            PopulationIncrease(popIncreasePerMin);
            seconds = 0f;
        }

        if (testPing)
        {
            PopulationIncrease(popIncreasePerMin);
            testPing = false;
        }
    }

    public void PopulationIncrease(int _popIncreasePerMin)
    {
        var saveManager = GameSavingManager.instance;
        var popData = saveManager.saveData.populationData;

        popData.currentPop += _popIncreasePerMin;

        HandleMaxPop(popData);

        saveManager.saveData.populationData = popData;

        OnPopUpdate?.Invoke();
    }

    bool HandleMaxPop(PopulationData popData)
    {
        if (popData.currentPop >= popData.maxPop)
        {
            popData.currentPop = popData.maxPop;

            if (!pingSent)
            {
                LogBoxManager.instance.NewBox(LogBoxType.Alert,
                    $"<color=red>Max population reached!</color> Build more <color=#A52A2A>houses</color> to house more people!");
                pingSent = true;
            }

            maxPop = true;
            return true;
        }
        else
        {
            maxPop = false;
            pingSent = false;
        }

        return false;
    }


}


[Serializable]
public enum PopulationClass
{
    Children,
    Soldiers,
    Workers,
}

[Serializable]
public class PopulationData
{
    public int maxPop;
    public int currentPop;
    public int childrenPop;
    public int[] childrenData;
    public int armyPop;
    public int workersPop;
}
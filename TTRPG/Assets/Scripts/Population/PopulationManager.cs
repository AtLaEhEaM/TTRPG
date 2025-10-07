using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public int popIncreasePerMin;
    int currentMin;
    float seconds;
    bool pingSent = false;
    public bool testPing = false;

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
                PopulationUpdate(popIncreasePerMin);
            }
        }
    }

    public void Update()
    {
        seconds = +Time.deltaTime;

        if(seconds > 60f)
        {
            PopulationUpdate(popIncreasePerMin);
            seconds = 0;
        }

        if (testPing)
        {
            PopulationUpdate(popIncreasePerMin);
            testPing = false;
        }
    }

    public void PopulationUpdate(int _popIncreasePerMin)
    {
        GameSavingManager.instance.saveData.populationData.currentPop += _popIncreasePerMin;

        HandleMaxPop();

        OnPopUpdate?.Invoke();
    }

    void HandleMaxPop()
    {
        if(GameSavingManager.instance.saveData.populationData.currentPop >=
        GameSavingManager.instance.saveData.populationData.maxPop
        && !pingSent)
        {
            LogBoxManager.instance.NewBox(LogBoxType.Alert,
                $"<color=red>Max population reached!</#A52A2A> Build more <color=brown>houses</color> to house more people!");

            pingSent = true;
        }
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
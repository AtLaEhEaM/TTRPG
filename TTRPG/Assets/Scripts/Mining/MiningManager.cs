using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager instance;

    [Header("Mining Settings")]
    public float tickInterval = 4f; // run every 4 seconds
    private float tickTimer = 0f;

    public List<MiningTrip> miningList = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        if (GameSavingManager.instance.saveData.miningTripsList == null)
            return;

        foreach (MiningTrip trip in GameSavingManager.instance.saveData.miningTripsList)
            AddTrip(trip);
    }

    public void AddTrip(MiningTrip trip)
    {
        trip.timeStarted = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        miningList.Add(trip);

        GameSavingManager.instance.saveData.miningTripsList.Add(trip);
        GameSavingManager.instance.SaveGame();
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval && miningList != null && miningList.Count > 0)
        {
            ProcessMiningTrips();
            tickTimer = 0f;
        }
    }

    private void ProcessMiningTrips()
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (MiningTrip trip in miningList)
        {
            CheckForEvent(trip, currentTime);
            TryMineTrip(trip);
        }
    }

    private void CheckForEvent(MiningTrip trip, long currentTime) //random chance of rolling an event for mining, may be good or bad
    {
        long elapsed = currentTime - trip.timeStarted;
        int requiredTime = 120;

        int _chanceOfEvent = UnityEngine.Random.Range(0, 10);

        if (elapsed >= requiredTime)
        {
            if (_chanceOfEvent <= 4)
            {
                _chanceOfEvent = UnityEngine.Random.Range(0, 10);

                if (_chanceOfEvent <= 4)
                    GameManager.instance.miningEvents.GoodEvent(trip, currentTime);
                else
                    GameManager.instance.miningEvents.BadEvent(trip, currentTime);
            }
        }
    }

    private void TryMineTrip(MiningTrip trip)
    {
        int roll = UnityEngine.Random.Range(0, 100);

        if (roll < trip.caveLayer * 10)
            return;

        foreach (MineralData data in trip.mineralsMineableList)
        {
            MineMineral(trip, data);
        }
    }

    private void MineMineral(MiningTrip trip, MineralData data)
    {
        int roll = UnityEngine.Random.Range(0, 100);

        if (roll >= data.rarityChance)
            return;

        int layer = trip.caveLayer + 1;
        int amount = Mathf.FloorToInt(Mathf.Pow(trip.workers, layer));

        amount = (int)TimeAdjustmentScript.ReduceTime(amount);

        if (amount > 0)
        {
            Debug.Log($"Trip mined {amount} {data.type} at layer {layer} with {trip.workers} workers");
            GameManager.instance.economyManager.UpdateMinerals(data.type, amount);
        }
    }
}

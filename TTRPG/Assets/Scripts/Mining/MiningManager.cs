using System;
using System.Collections.Generic;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public static MiningManager instance;

    [Header("Mining Settings")]
    public float tickInterval = 16f; 
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

    public void AddTrip(MiningTrip trip) => miningList.Add(trip);
    

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
        foreach (MiningTrip trip in miningList)
        {
            int chance = UnityEngine.Random.Range(0, 100);

            // If this trip fails the initial cave layer roll, skip it but keep checking others
            if (chance < trip.caveLayer * 10)
                continue;

            foreach (MineralData data in trip.mineralsMineableList)
            {
                chance = UnityEngine.Random.Range(0, 100);

                if (chance < data.rarityChance)
                {
                    // Controlled growth formula: workers * log(caveLayer + 1)
                    int amount = Mathf.FloorToInt(trip.workers * Mathf.Log(trip.caveLayer + 1, 2));

                    amount = (int)TimeAdjustmentScript.ReduceTime(amount);

                    if (amount > 0)
                        GameManager.instance.economyManager.UpdateMinerals(data.type, amount);
                }
            }
        }
    }
}

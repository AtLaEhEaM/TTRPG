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

            if (chance < trip.caveLayer * 10)
                continue;

            foreach (MineralData data in trip.mineralsMineableList)
            {
                chance = UnityEngine.Random.Range(0, 100);

                if (chance < data.rarityChance)
                {
                    //Debug.Log("hello from the abyss");
                    int _layer = trip.caveLayer + 1;
                    int amount = Mathf.FloorToInt(Mathf.Pow(trip.workers, _layer));

                    amount = (int)TimeAdjustmentScript.ReduceTime(amount);
                    Debug.Log("amount: " + amount + " Layer: " + _layer + " workers: " + trip.workers);

                    if (amount > 0)
                        GameManager.instance.economyManager.UpdateMinerals(data.type, amount);
                }
            }
        }
    }
}

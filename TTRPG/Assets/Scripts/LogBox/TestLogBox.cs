using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class TestLogBox : MonoBehaviour
{
    [Header("General Info")]
    public LogBoxType type;
    public int _case;

    [Header("Crop Data")]
    public bool plant = false;
    public FoodTypes _type;
    public int amount;
    public float timer;

    [Header("Economy")]
    public bool addGold;
    public int gold;
    public int workers;

    [Header("Send Logic")]
    public int sendNumber;
    public bool senddd = false;

    public List<MineralData> dataa = new();

    [Header("Alerts & Messages")]
    public bool sentAlert = false;
    public bool sentMessage = false;

    public bool caveevent = false;
    public bool goodevent = false;

    private void Update()
    {
        if (caveevent)
        {
            caveevent = false;

            MiningTrip trip = new();
            trip.caveLayer = 2;
            trip.mineralsMineableList = dataa;
            trip.timeStarted = 4;
            trip.caveLayer = 2;
            trip.workers = workers;

            MiningManager.instance.AddTrip(trip);

            if (goodevent)
                GameManager.instance.miningEvents.GoodEvent(trip, trip.timeStarted);
            else
                GameManager.instance.miningEvents.BadEvent(trip, trip.timeStarted);
        }

        if (sentAlert)
        {
            LogBoxManager.instance.NewFarmerBox(
                plant,                // plantation flag
                FoodTypes.Wheat,      // crop type
                0,                    // amount
                0                     // grow time
            );
            sentAlert = false;
        }

        if (sentMessage)
        {
            LogBoxManager.instance.NewFarmerBox(
                plant,                // plantation flag
                _type,                // crop type
                amount,               // amount
                timer                 // grow time
            );

        }

        if (addGold)
        {
            GameSavingManager.instance.saveData.economyData.workers += workers;
            addGold = false;
            GameManager.instance.economyManager.UpdateGold(gold);
            Debug.Log("saved gold");
        }

        if (senddd)
        {
            senddd = false;
            //for (int i = 0; i < sendNumber; i++)
            //{
            //    LogBoxManager.instance.NewFarmerBox(plant, _type, amount, timer);

            //    GameManager.instance.cropGrowthManager.GrowFood(_type, amount, timer);
            //}

            MiningTrip trip = new();
            trip.caveLayer = 2;
            trip.mineralsMineableList = dataa;
            trip.timeStarted = 4;
            trip.caveLayer = 2;
            trip.workers = workers;

            MiningManager.instance.AddTrip(trip);
        }
    }
}

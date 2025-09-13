using Unity.VisualScripting;
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

    [Header("Alerts & Messages")]
    public bool sentAlert = false;
    public bool sentMessage = false;

    private void Update()
    {
        if(sentAlert)
        {
            LogBoxManager.instance.NewFarmerBox(type, plant, FoodTypes.Wheat, 0, 0);
            sentAlert = false;
        }
        if (sentMessage)
        {
            LogBoxManager.instance.NewFarmerBox(type, plant, _type, amount, timer);

            GameManager.instance.cropGrowthManager.GrowFood(_type, amount, timer);

            sentMessage = false;
        }
        if (addGold)
        {
            GameSavingManager.instance.saveData.workerCount += workers;
            addGold = false;
            GameManager.instance.economyManager.UpdateGold(gold);
            Debug.Log("saved gold");
        }

        if (senddd)
        {
            senddd = false;
            for (int i = 0; i < sendNumber; i++)
            {
                LogBoxManager.instance.NewFarmerBox(type, plant, _type, amount, timer);

                GameManager.instance.cropGrowthManager.GrowFood(_type, amount, timer);
            }
        }
    }
}

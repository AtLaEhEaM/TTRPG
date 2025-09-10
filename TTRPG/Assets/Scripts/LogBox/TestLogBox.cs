using Unity.VisualScripting;
using UnityEngine;

public class TestLogBox : MonoBehaviour
{
    public LogBoxType type;
    public int _case;

    public bool plant = false;
    public FoodTypes _type;
    public int amount;
    public float timer;
    public bool addGold;
    public int gold;


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
            addGold = false;
            GameManager.instance.economyManager.UpdateGold(gold);
            Debug.Log("saved gold");
        }
    }
}

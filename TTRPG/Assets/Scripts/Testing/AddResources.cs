using UnityEngine;

public class AddResources : MonoBehaviour
{
    public int gold;
    public int wood;
    public int food;
    public int workers;
    public MineralTypes type;
    public int mineralCount;
    public bool send;

    public void Update()
    {
        if (send)
        {
            GameManager.instance.economyManager.UpdateGold(gold);
            GameManager.instance.economyManager.UpdateFood(food);
            GameManager.instance.economyManager.UpdateWood(wood);
            GameManager.instance.economyManager.UpdateWorkers(workers);
            GameManager.instance.economyManager.UpdateMinerals(type, mineralCount);

            send = false;
        }

        GameSavingManager.instance.saveData.populationData.armyPop += 10;
    }
}

using TMPro;
using UnityEngine;

public class DisplayPopulation : MonoBehaviour
{
    public PopulationData populationData;
    public TextMeshProUGUI[] displayInfoTexts;


    public void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        populationData = GameSavingManager.instance.saveData.populationData;
        DisplayData();
    }

    void DisplayData()
    {

        displayInfoTexts[0].text = $"{populationData.currentPop.ToString()} / {populationData.totalPop.ToString()}";
        displayInfoTexts[1].text = $"{populationData.armyPop.ToString()}";
        displayInfoTexts[2].text = $"{populationData.workersPop.ToString()}";
        displayInfoTexts[3].text = $"{populationData.childrenPop.ToString()}";
    }
}

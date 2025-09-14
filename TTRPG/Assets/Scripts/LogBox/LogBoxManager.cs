using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogBoxManager : MonoBehaviour
{
    [Header("References")]
    public GameObject LogBoxText;
    public Transform logBoxParent;

    [Header("Data")]
    public FarmerLogs farmerLogs = new();
    public TreePlantationLogs treeLogs = new();
    public List<LogBoxData> boxes = new();

    public static LogBoxManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadData;
    }

    void LoadData()
    {
        var saved = GameSavingManager.instance.saveData.logBoxDataList;
        if (saved == null || saved.Count == 0)
            return;

        foreach (var box in saved)
        {
            CreateLogBox(box.type, box.text);
        }
    }

    public void CreateLogBox(LogBoxType type, string message)
    {
        var boxData = new LogBoxData { text = message, type = type };

        if (boxes.Count < 20)
        {
            var newBox = Instantiate(LogBoxText, logBoxParent);
            ApplyBoxData(newBox, boxData);
            newBox.transform.SetAsFirstSibling();
        }
        else
        {
            // recycle oldest
            var oldest = logBoxParent.GetChild(logBoxParent.childCount - 1).gameObject;
            ApplyBoxData(oldest, boxData);
            oldest.transform.SetAsFirstSibling();
            boxes.RemoveAt(0);
        }

        boxes.Add(boxData);
        GameSavingManager.instance.saveData.logBoxDataList = new List<LogBoxData>(boxes);
        GameSavingManager.instance.SaveGame();
    }


    public void NewFarmerBox(bool plantation, FoodTypes type, int amount, float timeToGrow)
    {
        string finalString = !plantation
            ? farmerLogs.FarmerResultString(type, amount)
            : farmerLogs.StartPlantationStrings(type, amount, timeToGrow);

        CreateLogBox(LogBoxType.Farm, finalString);
    }

    public void NewTreePlantationBox(bool plantation, string treeType, int amount, float timeToGrow)
    {
        string finalString = plantation
            ? $"Started plantation of {amount} {treeType}(s), grows in {timeToGrow} days."
            : $"Harvested {amount} {treeType}(s).";

        CreateLogBox(LogBoxType.TreePlantation, finalString);
    }

    void ApplyBoxData(GameObject box, LogBoxData data)
    {
        box.GetComponent<TextMeshProUGUI>().text = data.text;
        box.GetComponent<LogBoxInfo>()._type = data.type;
    }
}

public enum LogBoxType
{
    Farm,
    Army,
    Scout,
    TreePlantation, 
}

[Serializable]
public class LogBoxData
{
    public string text;
    public LogBoxType type;
}

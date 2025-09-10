using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class LogBoxManager : MonoBehaviour
{
    [Header("References")]
    public GameObject LogBoxText;
    public Transform logBoxParent;

    [Header("Data")]
    public FarmerLogs farmerLogs = new();
    public List<LogBoxData> boxes = new();

    public static LogBoxManager instance;
    private string finalText;

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
            LoadBox(box.type, box.text);
            KeepRecord(box);
        }
    }

    public void NewFarmerBox(LogBoxType _type, bool plantation, FoodTypes type, int amount, float timeToGrow)
    {
        string finalString;

        if (!plantation)
            finalString = farmerLogs.FarmerResultString(type, amount);
        else
            finalString = farmerLogs.StartPlantationStrings(type, amount, timeToGrow);
            // create UI
        var newBox = Instantiate(LogBoxText, logBoxParent);
        newBox.GetComponent<TextMeshProUGUI>().text = finalString;
        newBox.GetComponent<LogBoxInfo>()._type = _type;
        newBox.transform.SetAsFirstSibling();

        // create data
        var boxData = new LogBoxData { text = finalString, type = _type };

        KeepRecord(boxData);
        GameSavingManager.instance.saveData.logBoxDataList = new List<LogBoxData>(boxes);
        GameSavingManager.instance.SaveGame();
    }

    public void LoadBox(LogBoxType _type, string _text)
    {
        var newBox = Instantiate(LogBoxText, logBoxParent);
        newBox.GetComponent<TextMeshProUGUI>().text = _text;
        newBox.GetComponent<LogBoxInfo>()._type = _type;
        newBox.transform.SetAsFirstSibling();
    }

    void KeepRecord(LogBoxData data)
    {
        // Remove oldest UI and data if we exceed 20
        if (boxes.Count >= 20)
        {
            boxes.RemoveAt(0);
            Destroy(logBoxParent.GetChild(logBoxParent.childCount - 1).gameObject);
        }

        boxes.Add(data);
    }
}
public enum LogBoxType
{
    Farm,
    Army,
    Scout,
}

[Serializable]
public class LogBoxData
{
    public string text;
    public LogBoxType type;
}

/*
// Green
"<color=#00FF00>Green</color>"

// Red
"<color=#FF0000>Red</color>"

// Blue
"<color=#0000FF>Blue</color>"

// Yellow
"<color=#FFFF00>Yellow</color>"

// Cyan
"<color=#00FFFF>Cyan</color>"

// Magenta
"<color=#FF00FF>Magenta</color>"

// Orange
"<color=#FFA500>Orange</color>"

// Purple
"<color=#800080>Purple</color>"

// Pink
"<color=#FFC0CB>Pink</color>"

// Brown
"<color=#A52A2A>Brown</color>"

// Gray
"<color=#808080>Gray</color>"

// White
"<color=#FFFFFF>White</color>"
*/
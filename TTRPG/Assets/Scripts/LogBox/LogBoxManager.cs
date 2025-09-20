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

    public void NewBox(LogBoxType boxType, string message)
    {
        CreateLogBox(boxType, message);
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
    Mining,
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

/*Name	Hex Code	Example in TMP
White	#FFFFFF	<color=#FFFFFF>White</color>
Black	#000000	<color=#000000>Black</color>
Red	#FF0000	<color=#FF0000>Red</color>
Green	#00FF00	<color=#00FF00>Green</color>
Blue	#0000FF	<color=#0000FF>Blue</color>
Yellow	#FFFF00	<color=#FFFF00>Yellow</color>
Cyan	#00FFFF	<color=#00FFFF>Cyan</color>
Magenta	#FF00FF	<color=#FF00FF>Magenta</color>
Orange	#FFA500	<color=#FFA500>Orange</color>
Purple	#800080	<color=#800080>Purple</color>
Pink	#FFC0CB	<color=#FFC0CB>Pink</color>
Gray	#808080	<color=#808080>Gray</color>
LightGray	#D3D3D3	<color=#D3D3D3>Light Gray</color>
DarkGray	#A9A9A9	<color=#A9A9A9>Dark Gray</color>
Brown	#A52A2A	<color=#A52A2A>Brown</color>
Gold	#FFD700	<color=#FFD700>Gold</color>
Silver	#C0C0C0	<color=#C0C0C0>Silver</color>
Lime	#32CD32	<color=#32CD32>Lime</color>
Teal	#008080	<color=#008080>Teal</color>
Navy	#000080	<color=#000080>Navy</color>
*/
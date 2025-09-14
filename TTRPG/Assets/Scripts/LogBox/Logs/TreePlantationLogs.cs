using System;
using UnityEngine;

[Serializable]
public class TreePlantationLogs
{
    private string finalText;

    public string TreeHarvestResultString(int amount, int thresholdSmall, int thresholdMedium)
    {
        string mood = GetChopMood(amount, thresholdSmall, thresholdMedium);

        // Always positive since we’re gaining logs
        string sign = "+";
        string color = "#8B4513"; // earthy brown for workers

        return finalText =
            $"Your <color={color}>Workers</color> {mood}!<br>" +
            $"They returned with <color=#FFD700>{sign}{amount} Tree Logs</color>";
    }

    public string StartTreePlantationString(string treeType, int amount, float time)
    {
        return finalText =
            $"<color=#8B4513>Workers</color> planted {amount} {treeType} saplings.<br>" +
            $"Harvest may be ready in {Mathf.FloorToInt(time)} minutes";
    }

    private string GetChopMood(int amount, int small, int medium)
    {
        if (amount < small)
            return "cut down a small patch of forest";
        if (amount < medium)
            return "chopped through a stretch of woodland";
        return "felled a vast area of towering trees";
    }
}

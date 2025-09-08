using UnityEngine;

[CreateAssetMenu(menuName = "Logs/LogEntry")]
public class LogEntry : ScriptableObject
{
    public LogBoxType type;
    public string key; // like "PositiveWheatSmall"
    [TextArea] public string templateText;
}

using TMPro;
using UnityEngine;

public class FilterButtons : MonoBehaviour
{
    public GameObject logBoxParent;
    private TextMeshProUGUI[] logBoxes;

    private LogBoxType? activeFilter = null; // null = no filter

    private void GetBoxes()
    {
        logBoxes = logBoxParent.GetComponentsInChildren<TextMeshProUGUI>(true);
    }

    public void FilterFarm()
    {
        ToggleFilter(LogBoxType.Farm);
    }

    public void FilterArmy()
    {
        ToggleFilter(LogBoxType.Army);
    }

    public void FilterScout()
    {
        ToggleFilter(LogBoxType.Scout);
    }

    private void ToggleFilter(LogBoxType type)
    {
        GetBoxes();

        // if the same filter is active, reset
        if (activeFilter == type)
        {
            ResetBoxes();
            activeFilter = null;
            return;
        }

        // apply new filter
        foreach (var box in logBoxes)
        {
            var info = box.GetComponent<LogBoxInfo>();
            box.gameObject.SetActive(info != null && info._type == type);
        }

        activeFilter = type;
    }

    private void ResetBoxes()
    {
        foreach (var box in logBoxes)
            box.gameObject.SetActive(true);
    }
}

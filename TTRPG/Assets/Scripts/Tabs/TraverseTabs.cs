using UnityEngine;

public class TraverseTabs : MonoBehaviour
{
    public GameObject[] tabs;
    public GameObject currentActiveTab;

    public void Activate(int index)
    {
        if (tabs == null || tabs.Length == 0)
        {
            Debug.LogError("No tabs assigned!");
            return;
        }

        if (index < 0 || index >= tabs.Length)
        {
            Debug.LogError($"Invalid index {index}, tabs length = {tabs.Length}");
            return;
        }

        if (currentActiveTab != null)
            currentActiveTab.SetActive(false);

        tabs[index].SetActive(true);
        currentActiveTab = tabs[index];
    }
}

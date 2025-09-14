using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject currentSubTab;
    public GameObject backButton;
    public GameObject MainTabs;

    public void OpenSubtab(GameObject _tab)
    {
        MainTabs.SetActive(false);
        currentSubTab = _tab;

        _tab.SetActive(true);
        backButton.SetActive(true);
    }

    public void CloseButton()
    {
        MainTabs.SetActive(true);

        currentSubTab.SetActive(false);
        backButton.SetActive(false);
    }
}

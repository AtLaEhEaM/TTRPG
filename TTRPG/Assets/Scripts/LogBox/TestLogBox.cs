using Unity.VisualScripting;
using UnityEngine;

public class TestLogBox : MonoBehaviour
{
    public LogBoxType type;
    public int _case;

    public bool sentAlert = false;

    private void Update()
    {
        if(sentAlert)
        {
            LogBoxManager.instance.NewBox(type, _case);
            sentAlert = false;
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGraphTest : MonoBehaviour
{
    public bool spawn = false;
    public int speed = 10;
    int curr = 0;
    public bool everyOtherFrame = false;
    public bool inc = false;
    public int dec = 3;
    public int maxNodes = 50;
    int currNodes = 0;
    public Transform parent;
    public HexGraph graph;
    float del = 5f;
    float c = 0;

    void Update()
    {
        

        if (inc)
        {
            c += Time.deltaTime;
            if(c >= del) spawn = true;
        }

        if (spawn)
        {
            graph.CreateNextNodeOnCurrentRing();
        }
    }


    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DeleteAllChildrenExceptSelf(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}

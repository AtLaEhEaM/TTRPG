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

    void Update()
    {
        if (inc)
        {
            TownManager.instance.maxConnections = dec;
            inc = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }

        if (spawn)
        {
            curr++;

            if (curr % speed == 0)
            {
                TownManager.instance.ContextAddRandom();
            }
        }
    }


    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

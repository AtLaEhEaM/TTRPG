using UnityEngine;

public class TestCityGen : MonoBehaviour
{
    public Vector2 ran;
    public bool test = false;
    public bool contin = false;
    int i = 1;

    void Update()
    {
        if (test || contin)
        {
            ran.x = UnityEngine.Random.Range(ran.x, ran.y);
            ran.y = UnityEngine.Random.Range(ran.x, ran.y);
            CityGenerator.instance.AddSingleNode(ran, i);
            i++;
            test = false;
        }    
    }
}

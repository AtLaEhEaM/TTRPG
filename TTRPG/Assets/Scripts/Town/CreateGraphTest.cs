using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGraphTest : MonoBehaviour
{
    public HexGraph graph;


    void Update()
    {
        
    }

    //private IEnumerator waitt()
    //{
    //    StartCoroutine(crete());
    //}

    IEnumerator crete()
    {
        yield return new WaitForSeconds(0.05f);
        graph.CreateNextNodeOnCurrentRing();

        StartCoroutine(crete());
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

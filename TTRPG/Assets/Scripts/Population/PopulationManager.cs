using System;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    


}

[Serializable]
public enum PopulationClass
{
    Children,
    Soldiers,
    Workers,
}

[Serializable]
public class PopulationData
{
    public int totalPop;
    public int currentPop;
    public int childrenPop;
    public int armyPop;
    public int workersPop;
}
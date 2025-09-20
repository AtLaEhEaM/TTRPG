using UnityEngine;

public class MiningEvents : MonoBehaviour
{

    public void GoodEvent(MiningTrip trip, long time) 
    {
        int _roll = Random.Range(0, 10);
        
        switch (_roll)
        {
            case 0:
                CheckAndProgressLayer(trip, time); 
                break;
        }
    }
    public void BadEvent(MiningTrip trip, long time) 
    {
        int _roll = Random.Range(0, 10);

        switch (_roll)
        {
            case 0:
                CaveDisaster(trip);
                break;
        }
    }


    private void CheckAndProgressLayer(MiningTrip trip, long currentTime) //good event
    {
        long elapsed = currentTime - trip.timeStarted;
        int requiredTime = 200 * trip.caveLayer;

        if (elapsed >= requiredTime)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            int successThreshold = 100 - (trip.caveLayer * 10);

            if (chance < successThreshold)
            {
                trip.caveLayer++;
                trip.timeStarted = (int)currentTime;
                Debug.Log($"Trip advanced to layer {trip.caveLayer}");
            }
        }
    }


    private void CaveDisaster(MiningTrip trip)
    {
        int _chance = Random.Range(0, 10);

        if(_chance < 5) //action event where you can save the miners
        {

        }
        else //event where miners die 
        {
            if (trip.workers <= 0)
                return;

            int deadWorkers = Random.Range(0, (trip.workers)/2);
            int remainingWorkers = trip.workers - deadWorkers;

            trip.workers = remainingWorkers;

            string message = "The cave walls come in crushing " + deadWorkers + " <color=#A52A2A>Workers!</color><br>" +
                remainingWorkers + " <color=#A52A2A> Workers</color> remain";

            LogBoxManager.instance.NewBox(LogBoxType.Mining, message);
        }
    }
}

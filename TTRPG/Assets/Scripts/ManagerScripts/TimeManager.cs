using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeEventScheduler : MonoBehaviour
{
    public static TimeEventScheduler instance;

    [SerializeField]private List<TimedEventData> events = new List<TimedEventData>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (events.Count == 0) return;

        DateTime now = DateTime.UtcNow;

        for (int i = events.Count - 1; i >= 0; i--)
        {
            if (now >= events[i].FinishTime)
            {
                events[i].onComplete?.Invoke();
                events.RemoveAt(i);
            }
        }
    }

    public void ScheduleEvent(string id, TimeSpan duration, Action onComplete)
    {
        var ev = new TimedEventData
        {
            id = id,
            finishTimeBinary = DateTime.UtcNow.Add(duration).ToBinary(),
            onComplete = onComplete
        };
        events.Add(ev);
    }

    public void ResumeEvent(string id, long finishTimeBinary, Action onComplete)
    {
        DateTime finishTime = DateTime.FromBinary(finishTimeBinary);
        if (DateTime.UtcNow >= finishTime)
        {
            onComplete?.Invoke();
        }
        else
        {
            var ev = new TimedEventData
            {
                id = id,
                finishTimeBinary = finishTimeBinary,
                onComplete = onComplete
            };
            events.Add(ev);
        }
    }
}

[Serializable]
public class TimedEventData
{
    public string id;                // Unique identifier
    public long finishTimeBinary;    // Stored in save data
    public Action onComplete;        // Action when finished

    public DateTime FinishTime => DateTime.FromBinary(finishTimeBinary);
}
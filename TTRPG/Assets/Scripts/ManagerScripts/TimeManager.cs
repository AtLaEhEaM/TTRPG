using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeEventScheduler : MonoBehaviour
{
    public static TimeEventScheduler instance;

    [SerializeField] private List<TimedEventData> events = new List<TimedEventData>();

    public void Start()
    {
        GameSavingManager.instance.OnSaveDataLoadedEvent += LoadEvents;
    }

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
        duration *= 60f;

        var ev = new TimedEventData
        {
            id = id,
            finishTimeBinary = DateTime.UtcNow.Add(duration).ToBinary(),
            onComplete = onComplete
        };
        events.Add(ev);
        SaveEvents();
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

    public void SaveEvents()
    {
        var saveList = new List<SavedEventData>();
        foreach (var e in events)
        {
            saveList.Add(new SavedEventData
            {
                id = e.id,
                finishTimeBinary = e.finishTimeBinary
            });
        }
        GameSavingManager.instance.saveData.scheduledEvents = saveList;
    }

    public void LoadEvents()
    {
        foreach (var saved in GameSavingManager.instance.saveData.scheduledEvents)
        {
            if (saved.id.StartsWith("wood_"))
            {
                ResumeEvent(saved.id, saved.finishTimeBinary, () =>
                {

                });
            }
        }
    }
}

[Serializable]
public class TimedEventData
{
    public string id;
    public long finishTimeBinary;
    public Action onComplete;

    public DateTime FinishTime => DateTime.FromBinary(finishTimeBinary);
}

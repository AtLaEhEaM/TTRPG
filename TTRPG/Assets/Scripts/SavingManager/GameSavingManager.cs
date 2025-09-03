using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSavingManager : MonoBehaviour
{
    public GameSaveData saveData = new();
    [SerializeField] string saveFileName = "GameSaveData.json";

    public event Action OnSaveDataLoadedEvent;

    public static GameSavingManager instance;

    [Header("For Editor")]
    public bool _SaveGame = false;
    public bool _LoadGame = false;
    public bool _DeleteSave = false;
    public bool _LoadLastSave = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (_SaveGame)
        {
            if (saveData == null) Debug.Log("help");
            SaveGame();
            _SaveGame = false;
        }
        if (_LoadGame)
        {
            LoadGame();
            _LoadGame = false;
        }
        if (_DeleteSave)
        {
            DeleteSave();
            _DeleteSave = false;
        }
        if (_LoadLastSave)
        {
            LoadGame();
            _LoadLastSave = false;
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        LoadGame();
    }

    void OpenSavedFolder()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath,saveFileName);
#if UNITY_EDITOR 
        UnityEditor.EditorUtility.RevealInFinder(saveFilePath);
#endif
    }

    void DeleteSave()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted from: " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("No save file to delete!");
        }
    }

    public void SaveGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game loaded from: " + saveFilePath);
        }
        else
        {
            //SaveGame();
            Debug.LogWarning("Save file not found!");
        }

        OnSaveDataLoadedEvent?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public static class SaveSystem 
{
    public static SaveData CurrentGameData
    {
        get
        {
            if (currentGameData == null)
            {
                string fileLocation = Application.persistentDataPath + SaveFileName;
                if (File.Exists(fileLocation))
                {
                    string jsonFileData = File.ReadAllText(fileLocation);
                    currentGameData = JsonConvert.DeserializeObject<SaveData>(jsonFileData);
                }
                else
                {
                    CreateNewGame();
                }
            }
            return currentGameData;
        }

        set
        {
            currentGameData = value;
        }
    }

    private static void CreateNewGame()
    {
        currentGameData = new SaveData();
    }

    private static SaveData currentGameData = null;
    private const string SaveFileName = "/ChannelDeathSave.json";

    /// <summary>
    /// Save the game to json file on disk
    /// </summary>
    public static void SaveGame()
    {
        string fileLocation = Application.persistentDataPath + SaveFileName;
        string jsonFileData = JsonConvert.SerializeObject(CurrentGameData);
        File.WriteAllText(fileLocation, jsonFileData);
    }
}

[Serializable]
public class SaveData
{
    public float TimeLimitRemainingInMinutes;
    public List<GameObject> QuestionList;
    public List<Victim> VictimHistory;

    public SaveData()
    {
        TimeLimitRemainingInMinutes = GameManager.Instance.TotalGameTimeLimitInMinutes;
        QuestionList = new List<GameObject>();
        VictimHistory = new List<Victim>();
    }
}

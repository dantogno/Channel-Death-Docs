using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveSystem 
{
    private const string SaveFileName = "/ChannelDeathSave.json";
    /// <summary>
    /// Save the game to json file on disk
    /// </summary>
    public void SaveGame()
    {
        string fileLocation = Application.persistentDataPath + SaveFileName;
        // string jsonFileData = JsonConvert.SerializeObject(currentGameData);
        // File.WriteAllText(fileLocation, jsonFileData);
    }
}

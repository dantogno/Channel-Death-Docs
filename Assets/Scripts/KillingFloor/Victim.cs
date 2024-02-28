using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Victim : MonoBehaviour
{
    public static Action<string> VictimNameUpdated;
    public bool isFemale;

    public string Name
    {
        get => displayName;
        set
        {
            displayName = value;
            VictimNameUpdated?.Invoke(displayName);
        }
    }

    public VictimState State { get; set; } = VictimState.None;

    private string displayName;
    private static List<string> availableFemaleNames = new List<string>();
    private static List<string> availableMaleNames = new List<string>();
    private static System.Random random = new System.Random();


 
    /// <summary>
    /// Initially loads the names from the CSV file in the resources folder.
    /// </summary>
    public static void InitializeNameListsFromCSV()
    {
        try
        {
            // load the names from the CSV file in the resources folder
            TextAsset csvFile = Resources.Load<TextAsset>("ChannelDeathNames");
            string[] lines = csvFile.text.Split('\n');

            // Parse each line and split by comma
            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                // Assuming male names are in the first column and female names in the second column
                if (columns.Length >= 2)
                {
                    availableMaleNames.Add(columns[0].Trim());
                    availableFemaleNames.Add(columns[1].Trim());
                }
            }

            SetAvailableNamesFromSavedData();
        }
        catch (FileNotFoundException)
        {
            Debug.LogError("CSV file not found. Make sure the file path is correct.");
        }
    }


    /// <summary>
    /// Needs to be called once after loading the game.
    /// The lists should update automatically during gameplay.
    /// </summary>
    private static void SetAvailableNamesFromSavedData()
    {
        // remove any names that are in the history
        foreach (var victim in SaveSystem.CurrentGameData.VictimHistory)
        {
            availableFemaleNames.Remove(victim.Name);
        }
    }

    public void SetRandomName()
    {
        var name = string.Empty;
        if (isFemale)
        {
            if (availableFemaleNames.Count == 0)
            {
                Debug.LogWarning("Out of names! Reusing. This is bad for some puzzle questions...");
                name = "NoNameJane";
            }
            else
            {
                int index = random.Next(availableFemaleNames.Count);
                name = availableFemaleNames[index];
                availableFemaleNames.RemoveAt(index);
            }
        }
        else
        {
            if (availableFemaleNames.Count == 0)
            {
                Debug.LogWarning("Out of names! Reusing. This is bad for some puzzle questions...");
                name = "NoNameBlane";
            }
            else
            {
                int index = random.Next(availableMaleNames.Count);
                name = availableMaleNames[index];
                availableMaleNames.RemoveAt(index);
            }
        }
       Name = name;
    }

    public enum VictimState
    {
        None,
        Rescued,
        Dead
    }
}

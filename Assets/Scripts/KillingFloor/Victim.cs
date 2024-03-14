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
    public VictimData VictimData = new VictimData();

    public string Name
    {
        get => displayName;
        set
        {
            displayName = value;
            VictimNameUpdated?.Invoke(displayName);
            VictimData.Name = displayName;
        }
    }

    public VictimState State
    {
        get => state;
        set
        {
            state = value;
            VictimData.State = state;
        }
    }

    private VictimState state;

    private string displayName;
    public static List<string> AvailableFemaleNames { get; private set; } = new List<string>();
    public static List<string> AvailableMaleNames { get; private set; } = new List<string>();
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

                // Assuming female names are in the first column and male names in the second column
                if (columns.Length >= 2)
                {
                    AvailableFemaleNames.Add(columns[0].Trim());
                    AvailableMaleNames.Add(columns[1].Trim());
                }
            }
            // Remove column headers, they aren't names!
            AvailableFemaleNames.RemoveAt(0);
            AvailableMaleNames.RemoveAt(0);

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
            AvailableFemaleNames.Remove(victim.Name);
        }
    }

    public void SetRandomName()
    {
        var name = string.Empty;
        if (isFemale)
        {
            if (AvailableFemaleNames.Count == 0)
            {
                Debug.LogWarning("Out of names! Reusing. This is bad for some puzzle questions...");
                name = "NoNameJane";
            }
            else
            {
                int index = random.Next(AvailableFemaleNames.Count);
                name = AvailableFemaleNames[index];
                AvailableFemaleNames.RemoveAt(index);
            }
        }
        else
        {
            if (AvailableFemaleNames.Count == 0)
            {
                Debug.LogWarning("Out of names! Reusing. This is bad for some puzzle questions...");
                name = "NoNameBlane";
            }
            else
            {
                int index = random.Next(AvailableMaleNames.Count);
                name = AvailableMaleNames[index];
                AvailableMaleNames.RemoveAt(index);
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
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

    private static string[] maleNames;// = { "Chad", "Brad", "Troy", "Brock", "Drake", "Blade", "Rex", "Ace", "Jace", "Zane", "Seth", "Cole", "Shane", "Brett", "Bryce", "Cody", "Drew", "Duke", "Gage", "Jett", "Kane", "Lance", "Max", "Nash", "Trent" };
    private static string[] femaleNames;// = { "Tiffany", "Brittany", "Ashley", "Courtney", "Heather", "Amber", "Crystal", "Misty", "Kelly", "Lacey", "Stacey", "Tracy", "Brandy", "Candy", "Mandy", "Jenny", "Lily", "Ruby", "Chloe", "Zoe", "Lexi", "Kylie", "Hailey", "Bailey", "Kayla" };
   // private static string[] unisexNames;// = { "Alex", "Blake", "Casey", "Charlie", "Chris", "Dakota", "Devon", "Jamie", "Jesse", "Jordan", "Kendall", "Lee", "Logan", "Morgan", "Parker", "Pat", "Quinn", "Reese", "Riley", "River", "Robin", "Sam", "Skyler", "Taylor", "Terry" };

    // Choose an appropriate name for the victim from the list of names, but don't repeat names until we've used them all.
    private static List<string> availableFemaleNames = new List<string>();
    private static List<string> availableMaleNames = new List<string>();
    private static System.Random random = new System.Random();

    //private static string[] LoadNamesFromCSV(string path)
    //{
    //    TextAsset names = Resources.Load<TextAsset>(path);
    //    string[] lines = names.text.Split('\n');

    //}

 
    /// <summary>
    /// Initially loads the names from the CSV file in the resources folder.
    /// </summary>
    private void InitializeNameListsFromCSV()
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
        availableFemaleNames.AddRange(femaleNames);
        availableMaleNames.AddRange(maleNames);
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
                availableFemaleNames.AddRange(femaleNames);
            }

            int index = random.Next(availableFemaleNames.Count);
            name = availableFemaleNames[index];
            availableFemaleNames.RemoveAt(index);
        }
        else
        {
            if (availableMaleNames.Count == 0)
            {
                Debug.LogWarning("Out of names! Reusing. This is bad for some puzzle questions...");
                availableMaleNames.AddRange(maleNames);
            }
            int index = random.Next(availableMaleNames.Count);
            name = availableMaleNames[index];
            availableMaleNames.RemoveAt(index);
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

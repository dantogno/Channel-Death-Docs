using System;
using System.Collections;
using System.Collections.Generic;
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
  
    private static string[] maleNames = { "Chad", "Brad", "Troy", "Brock", "Drake", "Blade", "Rex", "Ace", "Jace", "Zane", "Seth", "Cole", "Shane", "Brett", "Bryce", "Cody", "Drew", "Duke", "Gage", "Jett", "Kane", "Lance", "Max", "Nash", "Trent" };
    private static string[] femaleNames = { "Tiffany", "Brittany", "Ashley", "Courtney", "Heather", "Amber", "Crystal", "Misty", "Kelly", "Lacey", "Stacey", "Tracy", "Brandy", "Candy", "Mandy", "Jenny", "Lily", "Ruby", "Chloe", "Zoe", "Lexi", "Kylie", "Hailey", "Bailey", "Kayla" };
    private static string[] unisexNames = { "Alex", "Blake", "Casey", "Charlie", "Chris", "Dakota", "Devon", "Jamie", "Jesse", "Jordan", "Kendall", "Lee", "Logan", "Morgan", "Parker", "Pat", "Quinn", "Reese", "Riley", "River", "Robin", "Sam", "Skyler", "Taylor", "Terry" };

    // Choose an appropriate name for the victim from the list of names, but don't repeat names until we've used them all.
    private static List<string> availableFemaleNames = new List<string>();
    private static List<string> availableMaleNames = new List<string>();
    private static System.Random random = new System.Random();
    public void SetRandomName()
    {
        var name = string.Empty;
        if (isFemale)
        {
            if (availableFemaleNames.Count == 0)
            {
                availableFemaleNames.AddRange(femaleNames);
                availableFemaleNames.AddRange(unisexNames);
            }

            int index = random.Next(availableFemaleNames.Count);
            name = availableFemaleNames[index];
            availableFemaleNames.RemoveAt(index);
        }
        else
        {
            if (availableMaleNames.Count == 0)
            {
                availableMaleNames.AddRange(maleNames);
                availableMaleNames.AddRange(unisexNames);
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

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

        InitializePuzzleQuestions();
    }

    private static void InitializePuzzleQuestions()
    {
        // initialize the question list by loading all questions from the resources folder and choosing a random subset of ten uniuqe questions
        List<PuzzleQuestion> allQuestions = new List<PuzzleQuestion>(Resources.LoadAll<PuzzleQuestion>("PuzzleQuestions"));
        List<PuzzleQuestion> chosenQuestions = new List<PuzzleQuestion>();

        // todo: change to OverarchingPuzzleController.NumberOfQuestions
        for (int i = 0; i < 1; i++)
        {
            PuzzleQuestion question = allQuestions[UnityEngine.Random.Range(0, allQuestions.Count)];
            allQuestions.Remove(question);
            chosenQuestions.Add(question);
        }
        currentGameData.QuestionList = chosenQuestions;
    }

    private static SaveData currentGameData = null;
    private const string SaveFileName = "/ChannelDeathSave.json";

    /// <summary>
    /// Save the game to json file on disk
    /// </summary>
    public static void SaveGameToFile()
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
    public List<PuzzleQuestion> QuestionList;
    public List<Victim> VictimHistory;

    public SaveData()
    {
        TimeLimitRemainingInMinutes = GameManager.Instance.TotalGameTimeLimitInMinutes;
        QuestionList = new List<PuzzleQuestion>();
        VictimHistory = new List<Victim>();
    }
}

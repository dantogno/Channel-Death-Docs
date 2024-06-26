using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using static Victim;

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
                    Debug.Log(jsonFileData);

                    currentGameData = JsonConvert.DeserializeObject<SaveData>(jsonFileData);
                    // clear victims in history that have state == none
                    // these are victims that were not saved or killed when the game was saved
                    if (currentGameData.VictimHistory != null)
                    {
                        currentGameData.VictimHistory.RemoveAll(victim => victim.State == VictimState.None);
                    }
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

    public static void CreateNewGame()
    {
        currentGameData = new SaveData(new List<PuzzleQuestion>(), new List<VictimData>(), new List<PuzzleQuestionSave>(), GameManager.Instance.TotalTimeLimitInSeconds);

        InitializePuzzleQuestionList();

        // TODO: get multiple questions to load, progress through them!

        // LoadTestPuzzleQuestion();
    }

    private static void LoadTestPuzzleQuestion()
    {
        PuzzleQuestion question = Resources.Load<PuzzleQuestion>("PuzzleQuestions/PuzzleQuestion_FavColor");
        Debug.Log($"Adding question: {question.QuestionText} with answer index {question.CorrectAnswerIndex}");
        currentGameData.QuestionList.Add(question);
    }

    /// <summary>
    /// initialize the question list by loading all questions from the resources folder 
    /// and choosing a random subset of ten unique questions
    /// </summary>
    private static void InitializePuzzleQuestionList()
    {
        var allQuestions = Resources.LoadAll<PuzzleQuestion>("PuzzleQuestions").ToList();
        List<PuzzleQuestion> chosenQuestions = new List<PuzzleQuestion>();

        for (int i = 0; i < OverarchingPuzzleController.NumberOfQuestions; i++)
        {
            PuzzleQuestion question = allQuestions[UnityEngine.Random.Range(0, allQuestions.Count)];
            allQuestions.Remove(question);
            chosenQuestions.Add(question);
            question.InitializeQuestion();
        }
        for (int i = 0; i < chosenQuestions.Count; i++) {
            currentGameData.SavedQuestionList.Add(new PuzzleQuestionSave());
            currentGameData.SavedQuestionList[i].AnswerBank = chosenQuestions[i].AnswerBank;
            currentGameData.SavedQuestionList[i].ClueBank = chosenQuestions[i].ClueBank;
            currentGameData.SavedQuestionList[i].HasClueBeenGiven = chosenQuestions[i].HasClueBeenGiven ;
            currentGameData.SavedQuestionList[i].QuestionText = chosenQuestions[i].QuestionText ;
            currentGameData.SavedQuestionList[i].IsInitialized = chosenQuestions[i].IsInitialized;
            currentGameData.SavedQuestionList[i].CorrectAnswerIndex = chosenQuestions[i].CorrectAnswerIndex ;
            currentGameData.SavedQuestionList[i].Type = chosenQuestions[i].Type ;
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
    public float TimeRemainingInSeconds;
    public List<PuzzleQuestion> QuestionList;
    public List<VictimData> VictimHistory;
    public List<PuzzleQuestionSave> SavedQuestionList;

    public SaveData(List<PuzzleQuestion> QuestionList, List<VictimData> VictimHistory, List<PuzzleQuestionSave> SavedQuestionList, float TimeRemainingInSeconds)
    {
        this.TimeRemainingInSeconds = TimeRemainingInSeconds;
        this.QuestionList = QuestionList;
        this.VictimHistory = VictimHistory;
        this.SavedQuestionList = SavedQuestionList;
        if (SavedQuestionList.Count > 0) {
            for (int i = 0; i < SavedQuestionList.Count; i++) {
                this.QuestionList[i].AnswerBank = SavedQuestionList[i].AnswerBank;
                this.QuestionList[i].ClueBank = SavedQuestionList[i].ClueBank;
                this.QuestionList[i].HasClueBeenGiven = SavedQuestionList[i].HasClueBeenGiven;
                this.QuestionList[i].QuestionText = SavedQuestionList[i].QuestionText;
                this.QuestionList[i].IsInitialized = SavedQuestionList[i].IsInitialized;
                this.QuestionList[i].CorrectAnswerIndex = SavedQuestionList[i].CorrectAnswerIndex;
                this.QuestionList[i].Type = SavedQuestionList[i].Type;
            }
        }
    }
}

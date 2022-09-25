using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Scoreboard : MonoBehaviour
{

    [SerializeField] private int maxScoreboardEntries = 5;
    [SerializeField] private Transform highscoresHolderTransform = null;
    [SerializeField] private GameObject scoreboardEntryObject = null;

    [Header("Test")]
    [SerializeField] ScoreboardEntryData testEntryData;

    private string SavePath => $"{Application.persistentDataPath}/highscores.json";

    private void Start()
    {
        ScoreboardSavedData savedScores = GetSavedScores();

        UpdateUI(savedScores);
    }

    [ContextMenu("Add Entry")]
    public void AddTestEntry()
    {
        AddEntry(testEntryData);
    }

    public void AddEntry(ScoreboardEntryData scoreboardEntryData)
    {
        ScoreboardSavedData savedScores = GetSavedScores();
        bool scoreAdded = false;
        for (int i = 0; i < savedScores._hightScores.Count; i++)
        {
            if (scoreboardEntryData._entryScore > savedScores._hightScores[i]._entryScore)
            {
                savedScores._hightScores.Insert(i, scoreboardEntryData);
                scoreAdded = true;
                break;
            }
        }
        if (!scoreAdded && savedScores._hightScores.Count < maxScoreboardEntries)
        {
            savedScores._hightScores.Add(scoreboardEntryData);
        }

        if (savedScores._hightScores.Count > maxScoreboardEntries)
        {
            savedScores._hightScores.RemoveRange(maxScoreboardEntries, savedScores._hightScores.Count - maxScoreboardEntries);
        }

        UpdateUI(savedScores);

        SaveScores(savedScores);
    }

    private void UpdateUI(ScoreboardSavedData savedScores)
    {
        foreach (Transform child in highscoresHolderTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (ScoreboardEntryData highscore in savedScores._hightScores)
        {
            Instantiate(scoreboardEntryObject, highscoresHolderTransform).GetComponent<ScoreboardEntryUI>().Initialise(highscore);
        }
    }

    private ScoreboardSavedData GetSavedScores()
    {
        if (!File.Exists(SavePath))
        {
            File.Create(SavePath).Dispose();
            return new ScoreboardSavedData();
        }
        else
        {
            //using to aad leakages
            using (StreamReader stream = new StreamReader(SavePath))
            {
                string json = stream.ReadToEnd();
                return JsonUtility.FromJson<ScoreboardSavedData>(json);
            }
        }

    }

    private void SaveScores(ScoreboardSavedData scoreboardSavedData)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(scoreboardSavedData, true);
            stream.Write(json);
        }
    }

}

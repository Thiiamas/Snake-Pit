using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _entryNameText = null;
    [SerializeField] private TextMeshProUGUI _entryNameScore = null;

    public void Initialise(ScoreboardEntryData scoreboardEntryData)
    {
        _entryNameText.text = scoreboardEntryData._entryName;
        _entryNameScore.text = scoreboardEntryData._entryScore.ToString();
    }
}

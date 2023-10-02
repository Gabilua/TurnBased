using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchTurnStateChangeDisplay : MonoBehaviour
{
    [SerializeField] StageManager _stageManager;
    [SerializeField] MatchManager _matchManager;
    [SerializeField] TextMeshProUGUI _turnStateChangeDisplay;
    [SerializeField] Animator _turnStateChangeDisplayAnimator;

    private void Start()
    {
        _stageManager.StageEnd += StageEnd;

        _matchManager.TurnStateChange += TurnStateChange;
        _matchManager.MatchEnd += MatchEnded;
    }

    void StageEnd(bool playerWon)
    {
        string endMatchMessage = playerWon ? "VICTORY!" : "DEFEAT!";

        ShowTurnStateChangeDisplay(endMatchMessage);
    }

    void TurnStateChange(MatchTurnState turnState)
    {
        if (turnState == MatchTurnState.PlayerTurnWaitForInput)
            ShowTurnStateChangeDisplay("READY?");
        else if (turnState == MatchTurnState.TurnExecution)
            ShowTurnStateChangeDisplay("GO!");
    }

    void MatchEnded(bool hasPlayerWon)
    {
    
    }

    void ShowTurnStateChangeDisplay(string msg)
    {
        _turnStateChangeDisplay.text = msg;
        _turnStateChangeDisplayAnimator.SetTrigger("Action");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchTurnStateChangeDisplay : MonoBehaviour
{
    [SerializeField] MatchManager _matchManager;
    [SerializeField] TextMeshProUGUI _turnStateChangeDisplay;
    [SerializeField] Animator _turnStateChangeDisplayAnimator;

    private void Start()
    {
        _matchManager.TurnStateChange += TurnStateChange;
        _matchManager.MatchEnd += MatchEnded;
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
        string endMatchMessage = hasPlayerWon ? "VICTORY!" : "DEFEAT!";

        ShowTurnStateChangeDisplay(endMatchMessage);
    }

    void ShowTurnStateChangeDisplay(string msg)
    {
        _turnStateChangeDisplay.text = msg;
        _turnStateChangeDisplayAnimator.SetTrigger("Action");
    }
}

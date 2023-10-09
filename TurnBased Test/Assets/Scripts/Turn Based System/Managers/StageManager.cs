using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

    public enum StageProgressState { PreGame, InProgress, Completed}

public class StageManager : MonoBehaviour
{
    public Action<StageProgressState> StageStateChange;
    public Action<bool> StageEnd;

    StageInfo _currentStage;

    [SerializeField] MatchManager _matchManager;
    [SerializeField] PlayerTeamController _playerTeamController;
    [SerializeField] StageProgressdDisplay _stageProgressUI;
    [SerializeField] Image _stageScenery;
    [SerializeField] CombatManager _combatManager;

    [SerializeField] float timeToStartFirstMatch;
    [SerializeField] float timeToAdvanceToNextMatch;

    public StageProgressState _currentStageProgressState { get; private set; }

    MatchInfo _currentActiveMatch;

    int _currentActiveMatchIndex = 0;

    #region Unity

    #endregion

    #region Setup & Auxiliaries

    public void SetupStage(StageInfo stage)
    {
        _currentStage = stage;

        _matchManager.MatchEnd += MatchEnded;
        
        _currentActiveMatchIndex = 0;

        SetStageProgressState(StageProgressState.InProgress);

        _stageScenery.sprite = _currentStage.stageScenery;

        _stageProgressUI.SetupStageDisplay(_currentStage);

        Run.After(timeToStartFirstMatch, () => StartNewMatch());
    }

    void SetStageProgressState(StageProgressState state)
    {
        _currentStageProgressState = state;

        StageStateChange?.Invoke(_currentStageProgressState);
    }


    #endregion

    #region Stage Progress

    void StartNewMatch()
    {
        _currentActiveMatch = _currentStage.orderedMatches[_currentActiveMatchIndex];

        bool firstMatch = _currentActiveMatchIndex == 0;

        _stageProgressUI.UpdateMatchDisplay(_currentActiveMatch);

        _matchManager.SetupNewMatch(_playerTeamController.GetCurrentPlayerTeamInfo(), _currentActiveMatch, firstMatch);
    }

    void MatchEnded(bool playerWon)
    {
        _stageProgressUI.UpdateStageProgress();

        if (!IsStageOver())
            Run.After(timeToAdvanceToNextMatch, ()=> AdvanceToNextMatch());
        else
            Run.After(timeToAdvanceToNextMatch, () => EndStage(playerWon));
    }

    bool IsStageOver()
    {
        bool result = false;

        if (_currentActiveMatchIndex == _currentStage.orderedMatches.Count - 1)
            result = true;

        return result;
    }

    void AdvanceToNextMatch()
    {
        _currentActiveMatchIndex++;

        StartNewMatch();
    }

    void EndStage(bool playerWon)
    {
        SetStageProgressState(StageProgressState.Completed);

        _combatManager.ResetCombatManagerUI();

        StageEnd?.Invoke(playerWon);
    }

    #endregion

}

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
    [SerializeField] CombatManager _combatManager;
    [SerializeField] PlayerTeamController _playerTeamController;
    [SerializeField] StageProgressdDisplay _stageProgressUI;
    [SerializeField] StageRewardsScreen _stageRewardScreen;
    [SerializeField] Image _stageScenery;

    public float timeToStartFirstMatch;
    [SerializeField] float timeToAdvanceToNextMatch;
    [SerializeField] float timeToShowRewardScreen;

    public StageProgressState _currentStageProgressState { get; private set; }

    MatchInfo _currentActiveMatch;

    bool _stageResult;

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

        if (playerWon)
        {
            List<CharacterInfo> defeatedUnlockables = UnlockableCharactersDefeatedThisMatch();

            if (defeatedUnlockables.Count > 0)
                GameManager.instance.UnlockableCharactersDefeatedInMatch(defeatedUnlockables);
        }

        if (!IsStageOver())
            Run.After(timeToAdvanceToNextMatch, ()=> AdvanceToNextMatch());
        else
            Run.After(timeToAdvanceToNextMatch, () => EndStage(playerWon));
    }

    List<CharacterInfo> UnlockableCharactersDefeatedThisMatch()
    {
        List<CharacterInfo> defeatedUnlockables = new List<CharacterInfo>();

        foreach (var opponent in _currentActiveMatch._matchOpponents)
        {
            foreach (var unlockableCharacter in GameManager.instance.GetUnlockableCharacters())
            {
                if (opponent == unlockableCharacter)
                    defeatedUnlockables.Add(opponent);
            }
        }

        return defeatedUnlockables;
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

    void ToggleRewardScreen(bool state)
    {
        _stageRewardScreen.gameObject.SetActive(state);
    }

    void UpdateRewardScreen()
    {
        List<EquipmentInfo> droppedEquipments = new List<EquipmentInfo>();

        foreach (var stageReward in _currentStage.stageEquipmentRewards)
        {
            if(UnityEngine.Random.value * 100 <= stageReward.equipmentDropChance)
                droppedEquipments.Add(stageReward.equipmentReward);
        }

        int goldReward = Mathf.CeilToInt(UnityEngine.Random.Range(_currentStage.stageGoldRewardRange.x, _currentStage.stageGoldRewardRange.y));

        foreach (var droppedEquipment in droppedEquipments)
            GameManager.instance.AddEquipmentToPlayerStorage(droppedEquipment);

        GameManager.instance.EarnPlayerMoney(goldReward);

        ToggleRewardScreen(true);

        _stageRewardScreen.SetupRewardScreen(goldReward, droppedEquipments);
    }

    void EndStage(bool playerWon)
    {
        SetStageProgressState(StageProgressState.Completed);

        _combatManager.ResetCombatManagerUI();

        _stageResult = playerWon;

        UpdateRewardScreen();
    }

    public void RewardScreenClosed()
    {
        ToggleRewardScreen(false);

        Run.After(timeToAdvanceToNextMatch, () => StageEnd?.Invoke(_stageResult));
    }

    #endregion

}

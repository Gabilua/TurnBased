using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    public Action<MatchTurnState> TurnStateChange;
    public Action<RealtimeCombatant, bool> CombatantCreated;
    public Action<bool> AllCombatantsCreated;
    public Action MatchSetupDone;
    public Action<bool> MatchEnd;

    [SerializeField] float _timeDelayToAdvanceTurn;
    [SerializeField] float _intervalBetweenCombatantSpawn;

    [SerializeField] List<Transform> _playerCharacterSpots = new List<Transform>();
    [SerializeField] List<Transform> _enemyCharacterSpots = new List<Transform>();

    [SerializeField] Transform _playerSide;
    [SerializeField] Transform _enemySide;

    [SerializeField] GameObject _combatantPrefab;

    MatchInfo _currentMatchInfo;

    bool IsMatchSetupDone;

    #region Unity

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    #region Match Setup

    public void SetupNewMatch(List<CharacterInfo> playerTeam, MatchInfo matchInfo, bool firstMatchInARow)
    {
        ResetMatchState();
        
        _currentMatchInfo = matchInfo;

        if (firstMatchInARow)
        { 
            SetupTeam(playerTeam, GetPlayerTeam, _playerCharacterSpots);
        }
        else
        {
            GetEnemyTeam.Clear();

            foreach (var enemy in GetEnemyTeam)
                Destroy(enemy.gameObject, 2f);
        }
        
        SetupTeam(_currentMatchInfo._matchOpponents, GetEnemyTeam, _enemyCharacterSpots);

        SetupLists();

        AllCombatantsCreated?.Invoke(firstMatchInARow);

        Run.After(_timeDelayToAdvanceTurn, () => AdvanceTurn());
    }

    void CombatantSetupFinished(RealtimeCombatant combatant)
    {
        bool allCombatantsSetup = true;

        foreach (var matchCombatant in GetAllCombatants)
        {
            if (!matchCombatant.IsSetupForMatch)
            {
                allCombatantsSetup = false;
                break;
            }
        }

        if (!allCombatantsSetup)
            return;

        if(!IsMatchSetupDone)
            FinishMatchSetup();
    }

    void FinishMatchSetup()
    {
        IsMatchSetupDone = true;

       Run.After(0.1f, () => MatchSetupDone?.Invoke());
    }

    void SetupTeam(List<CharacterInfo> charactersForTeam, List<RealtimeCombatant> intendedTeam, List<Transform> teamSpots)
    {
        for (int i = 0; i < charactersForTeam.Count; i++)
        {
            //yield return new WaitForSeconds(_intervalBetweenCombatantSpawn);

            RealtimeCombatant combatant = Instantiate(_combatantPrefab, teamSpots[i]).GetComponent<RealtimeCombatant>();

            combatant.SetupCharacter(charactersForTeam[i]);
            intendedTeam.Add(combatant);

            bool isPlayerCombatant = intendedTeam == GetPlayerTeam;

            if (isPlayerCombatant)
                combatant.gameObject.name = "Player's " + combatant._characterInfo.name;
            else
                combatant.gameObject.name = "Enemy " + combatant._characterInfo.name;

            combatant.CombatantFinishedSetup += CombatantSetupFinished;

            CombatantCreated?.Invoke(combatant, isPlayerCombatant);
        }
    }

    void SetupLists()
    {
        GetAllCombatants.Clear();

        GetAllCombatants.AddRange(GetPlayerTeam);
        GetAllCombatants.AddRange(GetEnemyTeam);

        foreach (var combatant in GetAllCombatants)
            combatant.SetCombatantList(GetAllCombatants);
    }

    void ResetMatchState()
    {
        foreach (var spot in _playerCharacterSpots)
        {
            if (spot.GetComponentInChildren<RealtimeCombatant>())
                Destroy(spot.GetComponentInChildren<RealtimeCombatant>().gameObject);
        }

        foreach (var spot in _enemyCharacterSpots)
        {
            if (spot.GetComponentInChildren<RealtimeCombatant>())
                Destroy(spot.GetComponentInChildren<RealtimeCombatant>().gameObject);
        }

        GetAllCombatants.Clear();
        GetPlayerTeam.Clear();
        GetEnemyTeam.Clear();

        IsMatchSetupDone = false;

        ChangeTurnState(MatchTurnState.PreBattle);
    }

    #endregion

    #region Match Progress

    public void ReadyToAdvanceTurn()
    {
        Run.After(_timeDelayToAdvanceTurn, () => AdvanceTurn());
    }

    void AdvanceTurn()
    {
        if (HasBattleEnded)
        {
            EndMatch();

            return;
        }

        switch (GetMatchTurnState)
        {
            case MatchTurnState.PreBattle:
                {
                    ChangeTurnState(MatchTurnState.PlayerTurnWaitForInput);

                    UpdateTeamTurnState(GetPlayerTeam, CombatantTurnState.WaitingForInput);
                    UpdateTeamTurnState(GetEnemyTeam, CombatantTurnState.NotMyTeamsTurn);
                }
                break;
            case MatchTurnState.PlayerTurnWaitForInput:
                {
                    ChangeTurnState(MatchTurnState.EnemyTurnWait);

                    UpdateTeamTurnState(GetEnemyTeam, CombatantTurnState.WaitingForInput);
                    UpdateTeamTurnState(GetPlayerTeam, CombatantTurnState.NotMyTeamsTurn);
                }
                break;
            case MatchTurnState.EnemyTurnWait:
                {
                    ChangeTurnState(MatchTurnState.TurnExecution);
                }
                break;
            case MatchTurnState.TurnExecution:
                {
                    ChangeTurnState(MatchTurnState.PlayerTurnWaitForInput);

                    UpdateTeamTurnState(GetPlayerTeam, CombatantTurnState.WaitingForInput);
                    UpdateTeamTurnState(GetEnemyTeam, CombatantTurnState.NotMyTeamsTurn);
                }
                break;
        }
    }

    void UpdateTeamTurnState(List<RealtimeCombatant> team, CombatantTurnState turnState)
    {
        foreach (var combatant in team)
        {
            if (combatant.currentTurnState == CombatantTurnState.Dead)
                continue;

            combatant.SetTurnState(turnState);
        }
    }   

    public void ChangeTurnState(MatchTurnState state)
    {
        GetMatchTurnState = state;

        TurnStateChange?.Invoke(GetMatchTurnState);
    }

    void EndMatch()
    {
        ChangeTurnState(MatchTurnState.PostBattle);

        bool hasPlayerWon = IsTeamDefeated(GetEnemyTeam);

        UpdateTeamTurnState(GetPlayerTeam, CombatantTurnState.PostGame);
        UpdateTeamTurnState(GetEnemyTeam, CombatantTurnState.PostGame);

        MatchEnd?.Invoke(hasPlayerWon);
    }

    public bool HasBattleEnded => IsTeamDefeated(GetPlayerTeam) || IsTeamDefeated(GetEnemyTeam);

    public bool IsTeamDefeated(List<RealtimeCombatant> team)
    {
        bool teamDefeated = true;

        foreach (var member in team)
        {
            if (member.currentTurnState == CombatantTurnState.Dead) continue;

            teamDefeated = false;
            break;
        }

        return teamDefeated;
    }

    public bool AllCombatantsDoneForTheTurn()
    {
        if (GetMatchTurnState == MatchTurnState.PlayerTurnWaitForInput)
            return WholeTeamDoneForTheTurn(GetPlayerTeam);
        else if (GetMatchTurnState == MatchTurnState.EnemyTurnWait)
            return WholeTeamDoneForTheTurn(GetEnemyTeam);

        return false;
    }

    bool WholeTeamDoneForTheTurn(List<RealtimeCombatant> team)
    {
        foreach (var combatant in team)
        {
            if (combatant.currentTurnState == CombatantTurnState.Dead)
                continue;

            if (combatant.currentTurnState != CombatantTurnState.DoneForTheTurn)
                return false;
        }

        return true;
    }

    #endregion

    #region Getters

    public List<RealtimeCombatant> GetPlayerTeam = new List<RealtimeCombatant>();

    public List<RealtimeCombatant> GetEnemyTeam = new List<RealtimeCombatant>();

    public List<RealtimeCombatant> GetAllCombatants = new List<RealtimeCombatant>();

    public MatchTurnState GetMatchTurnState { get; private set; }

    #endregion
}

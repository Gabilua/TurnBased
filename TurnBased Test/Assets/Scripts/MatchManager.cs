using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    public Action<MatchTurnState> TurnStateChange;
    public Action<RealtimeCombatant, bool> CombatantCreated;
    public Action MatchSetupDone;
    public Action<bool> MatchEnd;

    [SerializeField] float _timeDelayToAdvanceTurn;

    List<Transform> _playerCharacterSpots = new List<Transform>();
    List<Transform> _enemyCharacterSpots = new List<Transform>();

    [SerializeField] CharacterInfo[] _defaultPlayerTeam;
    [SerializeField] CharacterInfo[] _defaultEnemyTeam;
    [SerializeField] Transform _playerSide;
    [SerializeField] Transform _enemySide;

    [SerializeField] GameObject _combatantPrefab;

    #region Unity

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        ChangeTurnState(MatchTurnState.PreBattle);

        SetupCharacterSpots();

        SetupTeam(_defaultPlayerTeam, GetPlayerTeam, _playerCharacterSpots);
        SetupTeam(_defaultEnemyTeam, GetEnemyTeam, _enemyCharacterSpots);

        SetupLists();

        MatchSetupDone?.Invoke();

        Run.After(_timeDelayToAdvanceTurn, () => AdvanceTurn());
    }

    #endregion

    #region Match Setup

    void SetupCharacterSpots()
    {
        Transform[] playerTransforms = _playerSide.GetComponentsInChildren<Transform>();
        Transform[] enemyTransforms = _enemySide.GetComponentsInChildren<Transform>();

        for (int i = 1; i < playerTransforms.Length; i++)
        {
            _playerCharacterSpots.Add(playerTransforms[i]);
        }

        for (int i = 1; i < enemyTransforms.Length; i++)
        {
            _enemyCharacterSpots.Add(enemyTransforms[i]);
        }
    }

    void SetupTeam(CharacterInfo[] charactersForTeam, List<RealtimeCombatant> intendedTeam, List<Transform> teamSpots)
    {
        for (int i = 0; i < charactersForTeam.Length; i++)
        {
            RealtimeCombatant combatant = Instantiate(_combatantPrefab, teamSpots[i]).GetComponent<RealtimeCombatant>();

            combatant.SetupCharacter(charactersForTeam[i]);
            intendedTeam.Add(combatant);

            bool isPlayerCombatant = intendedTeam == GetPlayerTeam;

            if (isPlayerCombatant)
                combatant.gameObject.name = "Player's " + combatant._characterInfo.characterName;
            else
                combatant.gameObject.name = "Enemy " + combatant._characterInfo.characterName;

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
            ChangeTurnState(MatchTurnState.PostBattle);

            bool hasPlayerWon = IsTeamDefeated(GetEnemyTeam);

            UpdateTeamTurnState(GetPlayerTeam, CombatantTurnState.PostGame);
            UpdateTeamTurnState(GetEnemyTeam, CombatantTurnState.PostGame);

            MatchEnd?.Invoke(hasPlayerWon);

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

    public List<RealtimeCombatant> GetPlayerTeam { get; } = new List<RealtimeCombatant>();

    public List<RealtimeCombatant> GetEnemyTeam { get; } = new List<RealtimeCombatant>();

    public List<RealtimeCombatant> GetAllCombatants { get; } = new List<RealtimeCombatant>();

    public MatchTurnState GetMatchTurnState { get; private set; }

    #endregion
}

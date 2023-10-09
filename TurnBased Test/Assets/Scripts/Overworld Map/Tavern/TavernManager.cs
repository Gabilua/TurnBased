using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TavernManager : MonoBehaviour
{
    public Action<RecruitedCharacter> TeamMemberRecruited;
    public Action<RecruitedCharacter> TeamMemberDismissed;
    public Action<bool> OnTavernSceneExitSelected;
    public Action OnTavernExited;

    [SerializeField] PlayerTeamController _playerTeamController;

    [SerializeField] GameObject _tavernScene;

    [SerializeField] List<Transform> _characterSpots = new List<Transform>();
    [SerializeField] GameObject _tavernCharacterPrefab;

    [SerializeField] TavernCharacterInfoDisplay _characterInfoDisplay;
    [SerializeField] Animator _characterInfoDisplayAnimator;

    [SerializeField] Transform _teamMemberDisplayHolder;
    [SerializeField] GameObject _teamMemberDisplayPrefab;

    [SerializeField] RecruitedCharacter[] _testCharacters;

    RecruitedCharacter _currentlySelectedCharacter;

    public void SetupTavernScene(List<RecruitedCharacter> savedPlayerTeam)
    {
        _tavernScene.SetActive(true);

        SetupTavernCharacters(_testCharacters);
        SetupTeamCharacters(savedPlayerTeam);
    }

    public void ExitTavernScene()
    {
        _tavernScene.SetActive(false);

        OnTavernExited?.Invoke();
    }

    public void TavernSceneExitSelected()
    {
        OnTavernSceneExitSelected?.Invoke(true);
    }

    void SetupTavernCharacters(RecruitedCharacter[] availableCharacters)
    {
        foreach (Transform child in _characterSpots)
        {
            if (child.childCount > 0)
                Destroy(child.GetChild(0).gameObject);
        }

        _characterSpots.Shuffle();

        for (int i = 0; i < availableCharacters.Length; i++)
        {
            TavernCharacter tavernCharacter = Instantiate(_tavernCharacterPrefab, _characterSpots[i]).GetComponent<TavernCharacter>();
            tavernCharacter.SetupCharacter(availableCharacters[i]);

            tavernCharacter.gameObject.name = "Tavern "+availableCharacters[i].characterInfo.name;

            tavernCharacter.TavernCharacterSelected += TavernCharacterSelected;
        }
    }

    void SetupTeamCharacters(List<RecruitedCharacter> teamCharacters)
    {
        foreach (Transform child in _teamMemberDisplayHolder)
            Destroy(child.gameObject);

        for (int i = 0; i < teamCharacters.Count; i++)
        {
            TavernTeamMemberDisplay display = Instantiate(_teamMemberDisplayPrefab, _teamMemberDisplayHolder).GetComponent<TavernTeamMemberDisplay>();
            display.SetupTeamMemberDisplay(teamCharacters[i]);
            display.TeamMemberClicked += TeamMemberSelected;
        }
    }

    void SetupDismissedCharacterOnTavern(RecruitedCharacter dismissedCharacter)
    {
        foreach (Transform child in _teamMemberDisplayHolder)
        {
            if(child.gameObject.GetComponent<TavernTeamMemberDisplay>()._characterInfo == dismissedCharacter)
            {
                child.gameObject.GetComponent<TavernTeamMemberDisplay>().TeamMemberClicked -= TeamMemberSelected;
                Destroy(child.gameObject);
                break;
            }
        }

        foreach (var spot in _characterSpots)
        {
            if(spot.childCount == 0)
            {
                TavernCharacter tavernCharacter = Instantiate(_tavernCharacterPrefab, spot).GetComponent<TavernCharacter>();
                tavernCharacter.SetupCharacter(dismissedCharacter);

                tavernCharacter.gameObject.name = "Tavern " + dismissedCharacter.characterInfo.name;

                tavernCharacter.TavernCharacterSelected += TavernCharacterSelected;

                break;
            }
        }
    }

    void RemoveRecruitedCharacterFromTavern(RecruitedCharacter recruitedCharacter)
    {
        foreach (var spot in _characterSpots)
        {
            if (spot.childCount == 0)
                continue;

            if (spot.GetComponentInChildren<TavernCharacter>() && spot.GetComponentInChildren<TavernCharacter>()._characterInfo == recruitedCharacter)
            {
                spot.GetComponentInChildren<TavernCharacter>().TavernCharacterSelected -= TavernCharacterSelected;
                Destroy(spot.GetComponentInChildren<TavernCharacter>().gameObject);
                break;
            }
        }

        TavernTeamMemberDisplay display = Instantiate(_teamMemberDisplayPrefab, _teamMemberDisplayHolder).GetComponent<TavernTeamMemberDisplay>();
        display.SetupTeamMemberDisplay(recruitedCharacter);
        display.TeamMemberClicked += TeamMemberSelected;
    }

    void TeamMemberSelected(TavernTeamMemberDisplay character)
    {
        SelectCharacter(character._characterInfo, true);
    }

    void TavernCharacterSelected(TavernCharacter character)
    {
        SelectCharacter(character._characterInfo, false);
    }

    void SelectCharacter(RecruitedCharacter character, bool teamMember)
    {
        _currentlySelectedCharacter = character;

        _characterInfoDisplay.gameObject.SetActive(true);
        _characterInfoDisplayAnimator.SetTrigger("Action");

        _characterInfoDisplay.UpdateDisplay(character, teamMember);
    }

    public void DeselectCharacter()
    {
        _currentlySelectedCharacter = null;

        _characterInfoDisplay.gameObject.SetActive(false);
    }

    public void RecruitCharacter()
    {
        if (_playerTeamController.GetSavedPlayerTeam().Count == 6)
            return;

        TeamMemberRecruited?.Invoke(_currentlySelectedCharacter);
        RemoveRecruitedCharacterFromTavern(_currentlySelectedCharacter);
        DeselectCharacter();
    }

    public void DismissCharacter()
    {
        if (_playerTeamController.GetSavedPlayerTeam().Count == 1)
            return;

        TeamMemberDismissed?.Invoke(_currentlySelectedCharacter);
        SetupDismissedCharacterOnTavern(_currentlySelectedCharacter);
        DeselectCharacter();
    }
}

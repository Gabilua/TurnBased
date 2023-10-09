using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    [SerializeField] StageManager _stageManager;
    [SerializeField] PlayerTeamController _playerTeamController;
    [SerializeField] TavernManager _tavernManager;

    [SerializeField] Transform _mapNodeHolder;
    List<MapNodeController> _allMapNodes = new List<MapNodeController>();

    [SerializeField] Image _overworldStagePreview;
    [SerializeField] TextMeshProUGUI _overworldStageName;
    [SerializeField] float _timeToTransitionOutOfMatchEndScreen;

    [SerializeField] Button _startStageButton;

    [SerializeField] ScreenTransitionController _transitionController;

    public MapNodeController _startingMapNode;
    public MapNodeController _currentMapNode { get; private set; }
    public List<MapNodeController> GetAllMapNodes()
    {
        if (_allMapNodes.Count == 0)
            SetupMapNodes();

        return _allMapNodes;
    }

    private void Awake()
    {
        _tavernManager.TeamMemberRecruited += PlayerTeamMemberRecruited;
        _tavernManager.TeamMemberDismissed += PlayerTeamMemberDismissed;
        _tavernManager.OnTavernSceneExitSelected += StageEnd;

        _stageManager.StageEnd += StageEnd;
    }

    void PlayerTeamMemberRecruited(RecruitedCharacter member)
    {
        _playerTeamController.AddMemberToTeam(member);

        GameManager.instance.SaveCurrentGameProgress();
    }

    void PlayerTeamMemberDismissed(RecruitedCharacter member)
    {
        _playerTeamController.RemoveMemberFromTeam(member);

        GameManager.instance.SaveCurrentGameProgress();
    }
    
    void SetupMapNodes()
    {
        _allMapNodes.AddRange(_mapNodeHolder.GetComponentsInChildren<MapNodeController>());

        foreach (var node in _allMapNodes)
        {
            if (node == _startingMapNode)
                node.UnlockNode();          

            node.MapNodeSelected += MapNodeSelected;
        }
    }

    public void DownloadMapNodeProgress(List<MapNodeProgressState> savedMapNodeProgress)
    {
        if (_allMapNodes.Count == 0)
            SetupMapNodes();

        for (int i = 0; i < savedMapNodeProgress.Count; i++)
        {
            if (savedMapNodeProgress[i].mapNodeState == MapNodeState.Unlocked)
                _allMapNodes[i].UnlockNode();
            else if (savedMapNodeProgress[i].mapNodeState == MapNodeState.Current)
            {
                _allMapNodes[i].UnlockNode();
                _allMapNodes[i].SetAsCurrent();
                _currentMapNode = _allMapNodes[i];
            }
        }
    }

    void MapNodeSelected(MapNodeController node)
    {
        if (_transitionController.IsMidTransition)
            return;

        if(node != _currentMapNode)
            SetCurrentMapNode(node);
    }

    void StartStageFadeInTransition()
    {       
        _transitionController.TransitionMapAlpha(false);       
    }

    void StartStageFadeOutTransition()
    {
        _transitionController.TransitionMapAlpha(true);
    }

    public void StartSelectedStage()
    {
        StartStageFadeInTransition();
        StartStage();
    }

    void StartStage()
    {
        if (!_currentMapNode.nodeStageInfo.isTown)
            _stageManager.SetupStage(_currentMapNode.nodeStageInfo);
        else
            Run.After(_transitionController._screenTransitionDuration, ()=> _tavernManager.SetupTavernScene(_playerTeamController.GetSavedPlayerTeam()));
    }

    void StageEnd(bool playerWon)
    {
        StartStageFadeOutTransition();

        if (!playerWon)
            return;

        Run.After(_transitionController._screenTransitionDuration+ _timeToTransitionOutOfMatchEndScreen, () => 
        {
            if (_currentMapNode.nodeStageInfo.isTown)
                _tavernManager.ExitTavernScene();

            foreach (var node in _currentMapNode._nodesAccessibleFromThisOne)
                node.UnlockNode();

            _currentMapNode.SetAsCurrent();

            GameManager.instance.SaveCurrentGameProgress();
        });
    }

    void SetCurrentMapNode(MapNodeController node)
    {
        if (_currentMapNode != null)
            _currentMapNode.UnlockNode();

        _currentMapNode = node;
        _currentMapNode.SetAsCurrent();

        _overworldStageName.gameObject.SetActive(true);

        _overworldStageName.text = node.nodeStageInfo.name;
        _overworldStagePreview.sprite = node.nodeStageInfo.stageScenery;

        _startStageButton.gameObject.SetActive(true);
    }
}

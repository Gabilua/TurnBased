using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] StageManager _stageManager;
    [SerializeField] PlayerTeamController _playerTeamController;

    [SerializeField] Transform _mapNodeHolder;
    List<MapNodeController> _allMapNodes = new List<MapNodeController>();

    [SerializeField] ScreenTransitionController _transitionController;

    MapNodeController _currentMapNode;

    private void Start()
    {
        SetupMapNodes();
    }

    void SetupMapNodes()
    {
        _allMapNodes.AddRange(_mapNodeHolder.GetComponentsInChildren<MapNodeController>());

        foreach (var node in _allMapNodes)
        {
            node.MapNodeSelected += MapNodeSelected;
        }
    }

    void MapNodeSelected(MapNodeController node)
    {
        if(node == _currentMapNode)
        {
            StartStageFadeInTransition();
            StartStage();
        }
        else
            SetCurrentMapNode(node);
    }

    void StartStageFadeInTransition()
    {       
        _transitionController.TransitionMapAlpha(false);       
    }

    void StartStage()
    {
        _stageManager.SetupStage(_currentMapNode.nodeStageInfo);
    }

    void SetCurrentMapNode(MapNodeController node)
    {
        _currentMapNode = node;
        _currentMapNode.SetAsCurrent();
    }
}

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

    public MapNodeController _startingMapNode;

    public MapNodeController _currentMapNode { get; private set; }

    public List<MapNodeController> GetAllMapNodes()
    {
        if (_allMapNodes.Count == 0)
            SetupMapNodes();

        return _allMapNodes;
    }
    
    void SetupMapNodes()
    {
        _allMapNodes.AddRange(_mapNodeHolder.GetComponentsInChildren<MapNodeController>());

        foreach (var node in _allMapNodes)
        {
            if (node == _startingMapNode)
            {
                node.UnlockNode();
                SetCurrentMapNode(node);
            }

            node.MapNodeSelected += MapNodeSelected;
        }
    }

    public void DownloadMapNodeProgress(List<MapNodeController> savedMapNodes)
    {
        for (int i = 0; i < savedMapNodes.Count; i++)
        {
            if (savedMapNodes[i].currentNodeState == MapNodeController.MapNodeState.Unlocked)
                _allMapNodes[i].UnlockNode();
            else if (savedMapNodes[i].currentNodeState == MapNodeController.MapNodeState.Current)
            {
                _allMapNodes[i].SetAsCurrent();
                _currentMapNode = _allMapNodes[i];
            }
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

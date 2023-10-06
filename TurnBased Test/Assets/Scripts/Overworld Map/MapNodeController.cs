using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapNodeController : MonoBehaviour
{
    public Action<MapNodeController> MapNodeSelected;

    public enum MapNodeState { Locked, Unlocked, Current }

    public StageInfo nodeStageInfo;
    public MapNodeState currentNodeState { get; private set; }
    [SerializeField] MapNodeController[] _nodesAccessibleFromThisOne;

    [Header("GFX")]
    [SerializeField] SpriteRenderer _nodeGFX;
    [SerializeField] Animator _nodeGFXAnimator;
    [SerializeField] SpriteRenderer _mapGFX;
    [SerializeField] SpriteRenderer _mapGFXShadow;
    [SerializeField] Transform _connectionsHolder;
    [SerializeField] GameObject _lineConnectionPrefab;

    [SerializeField] Sprite[] _nodeStateGraphics;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (nodeStageInfo != null)
        {
            gameObject.name =transform.GetSiblingIndex()+") "+ nodeStageInfo.name+" - Stage";
        }
    }

#endif

    private void Awake()
    {
        SetupNodeMapGFX();

        ResetNodeConnections();
    }

    public void UnlockNode()
    {
        SetNodeState(MapNodeState.Unlocked);
    }

    public void SetAsCurrent()
    {
        SetNodeState(MapNodeState.Current);
    }

    void SetNodeState(MapNodeState state)
    {
        currentNodeState = state;

        UpdateNodeGraphics();

        _nodeGFXAnimator.SetTrigger("Action");
    }

    public void MapNodeClicked()
    {
        if (currentNodeState == MapNodeState.Locked)
            return;

        _nodeGFXAnimator.SetTrigger("Action");

        MapNodeSelected?.Invoke(this);
    }

    void UpdateNodeGraphics()
    {
        _nodeGFX.sprite = _nodeStateGraphics[(int)currentNodeState];

        if (currentNodeState != MapNodeState.Locked)
            SetupNodeConnections();
    }

    [ContextMenu("Setup Connections")]
    void SetupNodeConnections()
    {
        ResetNodeConnections();

        foreach (var accessibleNode in _nodesAccessibleFromThisOne)
        {
            LineRenderer line = Instantiate(_lineConnectionPrefab).GetComponent<LineRenderer>();

            line.positionCount = 2;

            line.SetPosition(0, transform.position);
            line.SetPosition(1, accessibleNode.transform.position);

            line.transform.SetParent(_connectionsHolder);
        }
    }

    void ResetNodeConnections()
    {
        foreach (Transform connection in _connectionsHolder)
            DestroyImmediate(connection.gameObject);
    }

    void SetupNodeMapGFX()
    {
        if (nodeStageInfo.stageMapGFX != null)
        {
            _mapGFX.gameObject.SetActive(true);
            _mapGFX.sprite = nodeStageInfo.stageMapGFX;
            _mapGFXShadow.sprite = nodeStageInfo.stageMapGFX;
        }
        else
        {
            _mapGFX.gameObject.SetActive(false);
            _mapGFX.sprite = null;
            _mapGFXShadow.sprite = null;
        }
    }
}

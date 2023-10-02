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
    [SerializeField] Transform _connectionsHolder;
    [SerializeField] GameObject _lineConnectionPrefab;

    [SerializeField] Sprite[] _nodeStateGraphics;

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
       // if (currentNodeState == MapNodeState.Locked)
           // return;

        _nodeGFXAnimator.SetTrigger("Action");

        MapNodeSelected?.Invoke(this);
    }

    void UpdateNodeGraphics()
    {
        _nodeGFX.sprite = _nodeStateGraphics[(int)currentNodeState];

        SetupNodeConnections();
    }

    [ContextMenu("SetupNodeConnections")]
    void SetupNodeConnections()
    {
        foreach (Transform connection in _connectionsHolder)
            Destroy(connection.gameObject);

        foreach (var accessibleNode in _nodesAccessibleFromThisOne)
        {
            LineRenderer line = Instantiate(_lineConnectionPrefab).GetComponent<LineRenderer>();

            line.positionCount = 2;

            line.SetPosition(0, transform.position);
            line.SetPosition(1, accessibleNode.transform.position);

            line.transform.SetParent(_connectionsHolder);
        }
    }
}

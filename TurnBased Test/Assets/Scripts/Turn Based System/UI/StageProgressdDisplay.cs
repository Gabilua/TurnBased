using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageProgressdDisplay : MonoBehaviour
{
    [SerializeField] GameObject _nodePrefab;
    [SerializeField] Transform _nodeHolder;
    [SerializeField] TextMeshProUGUI _matchTitle;
    [SerializeField] TextMeshProUGUI _stageTitle;

    List<StageProgressNode> _activeNodes = new List<StageProgressNode>();

    public void SetupStageDisplay(StageInfo stage)
    {
        _stageTitle.text = stage.name;
        _matchTitle.text = "VS. " + stage.orderedMatches[0].name;

        for (int i = 0; i < stage.orderedMatches.Count; i++)
            SpawnProgressNode();
    }

    void SpawnProgressNode()
    {
       StageProgressNode node = Instantiate(_nodePrefab, _nodeHolder).GetComponent<StageProgressNode>();

        _activeNodes.Add(node);
    }

    public void UpdateMatchDisplay(MatchInfo match)
    {
        _matchTitle.text = "VS. " + match.name;
        _activeNodes[0].PlayAnimation();
    }

    public void UpdateStageProgress()
    {
        if (_nodeHolder.childCount > 0)
        {
            StageProgressNode node = _activeNodes[0];

            _activeNodes.Remove(node);
            node.DestroyNode();
        }
    }
}

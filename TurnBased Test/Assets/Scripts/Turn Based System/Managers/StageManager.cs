using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] StageInfo _currentStage;

    [SerializeField] MatchManager _matchManager;
    [SerializeField] PlayerTeamController _playerTeamController;

    private void Start()
    {
        _matchManager.SetupNewMatch(_playerTeamController.GetCurrentPlayerTeamInfo(), _currentStage.orderedMatches[0]);
    }
}

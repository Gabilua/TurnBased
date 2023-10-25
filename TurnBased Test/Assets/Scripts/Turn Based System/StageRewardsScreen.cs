using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class StageRewardsScreen : MonoBehaviour
{
    [SerializeField] GameObject _exitButton;
    [SerializeField] TextMeshProUGUI _goldRewardValue;
    [SerializeField] Transform _equipmentRewardHolder;
    [SerializeField] GameObject _equipmentRewardPrefab;

    [SerializeField] Animator _goldValueDisplayAnimator;
    [SerializeField] Animator _equipmentHolderAnimator;

    [SerializeField] float _timeBeforeRewardsAppear;
    [SerializeField] float _timeBetweenEachEquipmentReward;

    List<EquipmentIconDisplay> _activeEquipmentRewardDisplays = new List<EquipmentIconDisplay>();

    public void SetupRewardScreen(int goldValue, List<EquipmentInfo> equipments)
    {
        ResetEquipmentRewardDisplays();

        Run.After(_timeBeforeRewardsAppear, () => UpdateGoldRewardDisplay(goldValue));

        StartCoroutine(SetupEquipmentDisplays(equipments));
    }

    void ResetEquipmentRewardDisplays()
    {
        _exitButton.SetActive(false);

        _goldRewardValue.text = 0.ToString();

        foreach (Transform child in _equipmentRewardHolder)
            Destroy(child.gameObject);

        _activeEquipmentRewardDisplays.Clear();
    }

    void UpdateGoldRewardDisplay(int value)
    {
        _goldValueDisplayAnimator.SetTrigger("Action");

        int currentMoneyDisplayValue = 0;
        _goldRewardValue.text = currentMoneyDisplayValue.ToString();

        DOTween.To(() => currentMoneyDisplayValue, x => currentMoneyDisplayValue = x, value, 1f)
           .OnUpdate(() =>
           {
               _goldRewardValue.text = currentMoneyDisplayValue.ToString();
           });
    }

    IEnumerator SetupEquipmentDisplays(List<EquipmentInfo> equipments)
    {
        yield return new WaitForSeconds(_timeBeforeRewardsAppear);

        _equipmentHolderAnimator.SetTrigger("Action");

        foreach (var equipment in equipments)
        {
            yield return new WaitForSeconds(_timeBetweenEachEquipmentReward);

            EquipmentIconDisplay display = Instantiate(_equipmentRewardPrefab, _equipmentRewardHolder).GetComponent<EquipmentIconDisplay>();

            display.SetupEquipmentIconDisplay(equipment);

            _activeEquipmentRewardDisplays.Add(display);
        }

        _exitButton.SetActive(true);
    }
}

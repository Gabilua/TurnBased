using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;
    [SerializeField] AudioSource _BGMSource;
    [SerializeField] AudioClip _overworldBGM;

    [SerializeField] float _BGMTransitionDuration;
    [SerializeField] float _maxBGMVolume;

    private void Awake()
    {
        _mapManager.StageLoaded += OnStageStarted;
        _mapManager.StageUnloaded += OnOverworldAccess;
        _mapManager.StageTransitionStarted += OnStageTransition;
    }

    void OnStageStarted(StageInfo info)
    {
        ChangeBGMSourceClip(info.stageBGM);
    }

    void OnStageTransition()
    {
        FadeOutBGM();
    }

    void OnOverworldAccess()
    {
        ChangeBGMSourceClip(_overworldBGM);
    }

    void ChangeBGMSourceClip(AudioClip clip)
    {
        FadeOutBGM();

        _BGMSource.clip = clip;

        Run.After(_BGMTransitionDuration, () => FadeInBGM());
    }

    void FadeOutBGM()
    {
        float updatedValue = _BGMSource.volume;

        DOTween.To(() => updatedValue, x => updatedValue = x, 0, _BGMTransitionDuration)
            .OnUpdate(() => _BGMSource.volume = updatedValue);
    }

    void FadeInBGM()
    {
        _BGMSource.Play();

        float updatedValue = _BGMSource.volume;

        DOTween.To(() => updatedValue, x => updatedValue = x, _maxBGMVolume, _BGMTransitionDuration)
            .OnUpdate(() => _BGMSource.volume = updatedValue);
    }
}


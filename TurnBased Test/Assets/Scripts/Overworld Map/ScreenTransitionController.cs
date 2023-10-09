using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ScreenTransitionController : MonoBehaviour
{
    [SerializeField] GameObject _overworldMap;
    public float _screenTransitionDuration;

    public bool IsMidTransition = false;

    private void Start()
    {
        Transitioner.Instance._transitionBlockAnimationTime = _screenTransitionDuration / 2f;
        Transitioner.Instance._transitionTime = _screenTransitionDuration / 2f;

        Transitioner.Instance.OnTransitionOutComplete += FadeOutComplete;
        Transitioner.Instance.OnTransitionInComplete += FadeInComplete;
    }

    public void TransitionMapAlpha(bool state)
    {
        if (IsMidTransition)
            return;

        IsMidTransition = true;

        FadeOutScreenFX();

        Run.After(_screenTransitionDuration, () => 
        {
            _overworldMap.SetActive(state);
        });
    }

    void FadeInComplete()
    {
        if (IsMidTransition)
            IsMidTransition = false;
    }

    void FadeOutComplete()
    {
        if (IsMidTransition)
            FadeInScreenFX();
    }

    void FadeOutScreenFX()
    {
        Transitioner.Instance.TransitionOutWithoutChangingScene();
    }

    void FadeInScreenFX()
    {
        Transitioner.Instance.TransitionInWithoutChangingScene();
    }
}

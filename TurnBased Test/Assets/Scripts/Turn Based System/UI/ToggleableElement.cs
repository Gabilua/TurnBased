using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableElement : MonoBehaviour
{
    [SerializeField] GameObject _toggelableElement;

    public void ToggleElementState(bool state)
    {
        _toggelableElement.SetActive(state);
    }
}

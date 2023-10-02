using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgressNode : MonoBehaviour
{
    [SerializeField] GameObject _destroyFX;
    [SerializeField] GameObject _activateFX;
    [SerializeField] Animator _animator;

    public void PlayAnimation()
    {
        Instantiate(_activateFX, transform);
        _animator.SetTrigger("Action");
    }

    public void DestroyNode()
    {
        GameObject fx = Instantiate(_destroyFX, transform);
        fx.transform.parent = null;
        fx.transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
}

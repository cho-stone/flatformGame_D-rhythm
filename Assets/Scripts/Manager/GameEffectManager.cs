using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectManager : MonoBehaviour
{
    [SerializeField] Animator lockOnEffectAnimator = null;
    string lockOn = "LockOn";

    public void LockOnEffect()
    {
        lockOnEffectAnimator.SetTrigger(lockOn);
    }
}

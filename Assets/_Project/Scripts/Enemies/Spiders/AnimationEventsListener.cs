using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsListener : MonoBehaviour
{
    public event Action standUpEnd;
    public event Action attack;
    public event Action attackEnd;

    public void OnStandUpEnded() => standUpEnd?.Invoke();
    public void OnAttackPerformed() => attack?.Invoke();
    public void OnAttackEnded() => attackEnd?.Invoke();
}
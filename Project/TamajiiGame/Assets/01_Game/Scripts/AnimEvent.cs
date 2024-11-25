using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent onAttackEvent = new UnityEvent();

    private void Attack()
    {
        onAttackEvent.Invoke();
    }
}

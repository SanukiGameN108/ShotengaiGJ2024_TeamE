using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    public UnityEvent<Collider, Collider> onTriggerEnter = new UnityEvent<Collider, Collider>();
    public UnityEvent<Collider, Collider> onTriggerExit = new UnityEvent<Collider, Collider>();

    private Collider collider = null;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(collider, other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(collider, other);
    }
}

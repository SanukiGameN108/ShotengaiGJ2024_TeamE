using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroySound : MonoBehaviour
{
    private AudioSource m_as = null;

    private void Awake()
    {
        m_as = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!m_as.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM ms_instance = null;
    private AudioSource m_as = null;

    public static void Create()
    {
        if (ms_instance == null)
        {
            var prefab = Resources.Load("Sound/BGM");
            Instantiate(prefab);
        }
    }

    private void Awake()
    {
        ms_instance = this;
        DontDestroyOnLoad(gameObject);
        m_as = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        ms_instance = null;
    }

    public static void Play()
    {
        if (ms_instance == null) return;
        if (ms_instance.m_as.isPlaying) return;
        ms_instance.m_as.Play();
    }

    public static void Stop()
    {
        if (ms_instance == null) return;
        if (!ms_instance.m_as.isPlaying) return;
        ms_instance.m_as.Stop();
    }
}

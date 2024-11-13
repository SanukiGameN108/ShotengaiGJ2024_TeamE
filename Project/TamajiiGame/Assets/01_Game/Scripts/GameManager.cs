using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    private static GameManager ms_instance = null;

    [SerializeField]
    private Timer m_timer = null;

    [SerializeField]
    private PostProcessVolume m_ppVolume = null;

    private MotionBlur m_motionBlur = null;

    private bool m_isGameStart = false;
    public static bool isGameStart
    {
        get
        {
            if (ms_instance == null) return false;
            return ms_instance.m_isGameStart;
        }
    }
    private float m_elapsedTime = 0f;

    private void Awake()
    {
        ms_instance = this;

        //m_ppVolume.profile.TryGetSettings<MotionBlur>(out m_motionBlur);
    }
    private void OnDestroy()
    {
        ms_instance = null;
    }

    public static void GameStart()
    {
        if (ms_instance == null) return;
        ms_instance.m_isGameStart = true;
    }

    private void Update()
    {
        if (!m_isGameStart) return;

        m_elapsedTime += Time.deltaTime;
        m_timer.SetTime(m_elapsedTime);
    }
}

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
    private Clear m_clear = null;

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
    private bool m_isGameClear = false;
    public static bool isGameClear
    {
        get
        {
            if (ms_instance == null) return false;
            return ms_instance.m_isGameClear;
        }
    }
    private float m_elapsedTime = 0f;
    public static float elapsedTime
    {
        get
        {
            if (ms_instance == null) return 0f;
            return ms_instance.m_elapsedTime;
        }
    }
    private void Awake()
    {
        ms_instance = this;
        m_ppVolume.profile.TryGetSettings<MotionBlur>(out m_motionBlur);
    }

    private void Start()
    {
        BGM.Play();
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

    public static void GameClear()
    {
        if (ms_instance == null) return;
        ms_instance.m_isGameClear = true;
        ms_instance.m_clear.Play();
        ms_instance.m_timer.SetShow(false);
    }

    public static void SetOnMotionBlur(bool on)
    {
        if (ms_instance == null) return;
        ms_instance.m_motionBlur.enabled.Override(on);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!m_isGameStart) return;

        if (!m_isGameClear)
        {
            if (TrashManager.trashCount > 0)
            {
                m_elapsedTime += Time.deltaTime;
                m_timer.SetTime(m_elapsedTime);
            }
            else
            {
                GameClear();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_timeText = null;

    public void SetTime(float time)
    {
        int minute = (int)(time / 60f);
        time -= 60f * minute;
        int second = (int)time;
        time -= second;
        int msec = (int)(time * 100f);
        m_timeText.text = string.Format
        (
            "{0:00}:{1:00}:{2:00}",
            minute,
            second,
            msec
        );
    }
}

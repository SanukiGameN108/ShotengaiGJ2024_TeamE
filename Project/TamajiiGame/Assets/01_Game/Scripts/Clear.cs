using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    [SerializeField]
    private Image m_bgImage = null;
    [SerializeField]
    private RectTransform m_clearRT = null;
    [SerializeField]
    private GameObject m_clearTimeObj = null;
    [SerializeField]
    private float m_clearAnimTime = 0.5f;
    [SerializeField]
    private TextMeshProUGUI m_timeText = null;
    [SerializeField]
    private AudioSource m_clearSE = null;
    [SerializeField]
    private AudioSource m_drumSE = null;
    [SerializeField]
    private AudioSource m_drumEndSE = null;

    private CanvasGroup m_cg = null;

    private void Awake()
    {
        m_cg = GetComponent<CanvasGroup>();
        m_cg.alpha = 0f;
        m_clearTimeObj.SetActive(false);
    }

    public void Play()
    {
        StartCoroutine(Co_Anim());
    }

    private void SetAlpha(Graphic g, float a)
    {
        var c = g.color;
        c.a = a;
        g.color = c;
    }

    [SerializeField]
    private float m_bgAnimTime = 0.125f;
    [SerializeField]
    private float m_bgAlpha = 0.8f;

    private IEnumerator Co_BgAnim()
    {
        var elapsed = 0f;
        var time = m_bgAnimTime;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            SetAlpha(m_bgImage, alpha * m_bgAlpha);
            yield return null;

            elapsed += Time.deltaTime;
        }
        SetAlpha(m_bgImage, m_bgAlpha);
    }

    private IEnumerator Co_Anim()
    {
        BGM.Stop();
        m_clearSE.Play();

        m_cg.alpha = 1f;

        SetAlpha(m_bgImage, 0f);

        StartCoroutine(Co_BgAnim());

        var elapsed = 0f;
        var time = 0.25f;
        while (elapsed < time)
        {
            var alpha = Easing.BackOut(elapsed, time, 0f, 1f, 2f);
            m_clearRT.localScale = Vector3.one * alpha;
            yield return null;

            elapsed += Time.deltaTime;
        }
        m_clearRT.localScale = Vector3.one;

        elapsed = 0f;
        time = 2f;
        while (elapsed < time)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(1))
            {
                break;
            }
            yield return null;
            elapsed += Time.deltaTime;
        }

        elapsed = 0f;
        time = 0.125f;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            m_clearRT.localScale = Vector3.one * (1f - alpha);
            yield return null;

            elapsed += Time.deltaTime;
        }
        m_clearRT.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.25f);

        m_clearTimeObj.SetActive(true);

        elapsed = 0f;
        time = m_clearAnimTime;

        m_drumSE.Play();

        var start = 0f;
        var end = GameManager.elapsedTime;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            var currTime = Mathf.Lerp(start, end, alpha);
            SetTime(currTime);
            yield return null;

            elapsed += Time.deltaTime;
        }
        SetTime(end);

        m_drumSE.Stop();
        m_drumEndSE.Play();

        elapsed = 0f;
        time = 0.25f;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            var scale = 1f + Mathf.Sin(Mathf.PI * alpha) * 0.2f;
            m_timeText.rectTransform.localScale = Vector3.one * scale;
            yield return null;

            elapsed += Time.deltaTime;
        }
        m_timeText.rectTransform.localScale = Vector3.one;

        elapsed = 0f;
        time = 2f;
        while (elapsed < time)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(1))
            {
                SceneManager.LoadScene("Title");
            }
            yield return null;
            elapsed += Time.deltaTime;
        }

        while (true)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(1))
            {
                SceneManager.LoadScene("Title");
            }
            yield return null;
        }
    }

    private void SetTime(float time)
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

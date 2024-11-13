using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField]
    private float m_waitTime = 3f;
    [SerializeField]
    private TextMeshProUGUI m_startText = null;

    private void Start()
    {
        SetAlpha(0f);
        PlayAnim();
    }

    private void PlayAnim()
    {
        StartCoroutine(Co_PlayAnim());
    }

    private void SetAlpha(float a)
    {
        var color = m_startText.color;
        color.a = a;
        m_startText.color = color;
    }

    private IEnumerator Co_PlayAnim()
    {
        yield return new WaitForSeconds(m_waitTime);

        var elapsed = 0f;
        var time = 0.5f;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            SetAlpha(alpha);
            yield return null;

            elapsed += Time.deltaTime;
        }

        var interval = 2f;
        StartCoroutine(Co_PushCheck());
        while (true)
        {
            yield return new WaitForSeconds(interval);
            elapsed = 0f;
            time = 1f;
            while (true)
            {
                var alpha = elapsed / time;
                var alpha2 = Mathf.Sin(Mathf.PI * alpha);
                SetAlpha(1f - alpha2);
                yield return null;

                elapsed += Time.deltaTime;
            }
        }
    }

    private IEnumerator Co_PushCheck()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Game");
            }
            yield return null;
        }
    }
}

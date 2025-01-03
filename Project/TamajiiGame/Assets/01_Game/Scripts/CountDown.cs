using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> m_countRTs = new List<RectTransform>();
    [SerializeField]
    private Image m_bgImg = null;
    [SerializeField]
    private AudioSource m_se0 = null;
    [SerializeField]
    private AudioSource m_se1 = null;

    private void Start()
    {
        PlayAnim();
    }

    public void PlayAnim()
    {
        StartCoroutine(Co_Anim());
    }

    private IEnumerator Co_Anim()
    {
        var elapsed = 0f;
        var time = 4f;
        var isPlaySE = false;
        var lastIndex = -1;
        while (elapsed < time)
        {
            var et = elapsed - (int)elapsed;
            int index = Mathf.FloorToInt(time - elapsed);
            if (index != lastIndex)
            {
                lastIndex = index;
                isPlaySE = false;
            }
            for (int i = 0; i < m_countRTs.Count; i++)
            {
                m_countRTs[i].gameObject.SetActive(i == index);
            }
            if (index < m_countRTs.Count)
            {
                var rt = m_countRTs[index];
                if (et <= 0.25f)
                {
                    var alpha = et / 0.25f;
                    var alpha2 = Easing.BackOut(alpha, 1f, 0f, 1f, 2.0f);
                    rt.localScale = Vector3.one * alpha2;
                }
                else if (et <= 0.75f)
                {
                    if (!isPlaySE)
                    {
                        if (index == 0)
                        {
                            m_se1.Play();
                        }
                        else
                        {
                            m_se0.Play();
                        }
                        isPlaySE = true;
                    }
                }
                else
                {
                    var alpha = (1f - et) / 0.25f;
                    rt.localScale = Vector3.one * alpha;
                }
            }
            yield return null;

            elapsed += Time.deltaTime;
        }

        GameManager.GameStart();
        gameObject.SetActive(false);
    }


}

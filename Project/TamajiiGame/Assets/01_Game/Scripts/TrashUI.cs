using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrashUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_trashIconRT = null;
    [SerializeField]
    private TextMeshProUGUI m_remainText = null;

    private int m_lastRemain = -1;
    private Coroutine m_animCor = null;

    public void SetRemain(int remain, int max)
    {
        m_remainText.text = string.Format("{0} / {1}", remain, max);
        if (m_lastRemain >= 0 && remain != m_lastRemain)
        {
            PlayAnim();
        }
        m_lastRemain = remain;
    }

    private void PlayAnim()
    {
        if (m_animCor != null)
        {
            StopCoroutine(m_animCor);
        }
        m_animCor = StartCoroutine(Co_Anim());
    }

    [SerializeField]
    private float m_shrinkScale = 0.2f;

    private IEnumerator Co_Anim()
    {
        var elapsed = 0f;
        var time = 0.125f;
        while (elapsed < time)
        {
            var alpha = elapsed / time;
            var alpha2 = Mathf.Sin(Mathf.PI * alpha);
            var scale = 1f + m_shrinkScale * alpha2;
            m_trashIconRT.localScale = Vector3.one * scale;
            yield return null;

            elapsed += Time.deltaTime;
        }
        m_trashIconRT.localScale = Vector3.one;

        m_animCor = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour
{
    [SerializeField]
    private Slider m_slider = null;
    [SerializeField]
    private Image m_barImg = null;
    [SerializeField]
    private Color m_color0 = Color.green;
    [SerializeField]
    private Color m_color1 = Color.yellow;
    [SerializeField]
    private Color m_color2 = Color.red;

    private CanvasGroup m_cg = null;

    private void Awake()
    {
        m_cg = GetComponent<CanvasGroup>();
        SetHP(1.0f, 1.0f);
    }

    public void SetShow(bool show)
    {
        m_cg.alpha = show ? 1f : 0f;
    }

    public void SetHP(float hp, float hpMax)
    {
        if (hpMax == 0f) return;
        float ratio = Mathf.Clamp01(hp / hpMax);
        m_slider.value = ratio;

        Color color = m_color0;
        if (ratio <= 0.1f) color = m_color2;
        else if (ratio <= 0.3f) color = m_color1;
        m_barImg.color = color;
    }
}

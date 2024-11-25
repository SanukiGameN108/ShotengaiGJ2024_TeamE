using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashArrow : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_imageRT = null;
    private Graphic m_image = null;
    private RectTransform m_rt = null;

    private void Awake()
    {
        m_image = m_imageRT.GetComponent<Graphic>();
        m_rt = GetComponent<RectTransform>();
    }

    public void SetShow(bool show)
    {
        m_image.enabled = show;
    }

    public void SetPos(Vector3 pos)
    {
        m_rt.anchoredPosition = pos;
    }
}

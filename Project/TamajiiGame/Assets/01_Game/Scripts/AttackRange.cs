using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [SerializeField]
    private float m_baseScale = 0.44f;

    private bool m_isOn = false;
    private float m_attackRange = 5f;
    private Renderer m_rend = null;

    private void Awake()
    {
        m_rend = GetComponent<Renderer>();
        m_rend.enabled = false;
        transform.localScale = Vector3.zero;
    }

    public void SetAttackRange(float range)
    {
        m_attackRange = range;
    }

    public void SetOn(bool on)
    {
        m_isOn = on;
    }

    private void Update()
    {
        float range = m_isOn ? m_attackRange : 0f;
        float currScale = transform.localScale.x;
        float scale = Mathf.Lerp(currScale, m_baseScale * range, 30f * Time.deltaTime);
        transform.localScale = Vector3.one * scale;
        m_rend.enabled = scale >= 0.01f;
    }
}

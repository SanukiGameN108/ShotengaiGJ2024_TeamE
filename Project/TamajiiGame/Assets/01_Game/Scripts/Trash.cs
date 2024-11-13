using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField]
    private float m_hp = 1f;
    [SerializeField]
    private HPGauge m_hpGauge = null;
    [SerializeField]
    private GameObject m_deathEffectPrefab = null;

    private float m_hpMax = 0f;
    private Camera m_mainCamera = null;

    private void Awake()
    {
        m_hpMax = m_hp;
        m_mainCamera = Camera.main;
    }

    public void TakeDamage(float damage)
    {
        if (m_hp <= 0f) return;

        if (m_hp <= damage)
        {
            m_hp = 0f;
            Death();
        }
        else
        {
            m_hp -= damage;
            m_hpGauge.SetHP(m_hp, m_hpMax);
        }
    }

    public void Death()
    {
        Destroy(m_hpGauge.gameObject);
        Destroy(gameObject);
        Instantiate(m_deathEffectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    private void LateUpdate()
    {
        Vector3 vec = m_mainCamera.transform.position - transform.position;
        vec.y = 0f;
        transform.rotation = Quaternion.LookRotation(vec);
    }
}

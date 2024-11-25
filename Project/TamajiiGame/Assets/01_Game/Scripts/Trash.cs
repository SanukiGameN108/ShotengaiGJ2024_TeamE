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
    [SerializeField]
    private GameObject m_damageEfffectPrefab = null;
    [SerializeField]
    private float m_effectTime = 0.0625f;
    [SerializeField]
    private float m_seTime = 0.25f;
    [SerializeField]
    private GameObject m_damageSEPrefab = null;
    [SerializeField]
    private GameObject m_deathSEPrefab = null;

    private TrashArrow m_arrow = null;
    private float m_hpMax = 0f;
    private Camera m_mainCamera = null;
    private float m_elapsedTime = 0f;
    private float m_seElapsedTime = 0f;
    private RectTransform m_iconRT = null;

    private void Awake()
    {
        m_hpMax = m_hp;
        m_mainCamera = Camera.main;
    }

    private void Start()
    {
        m_iconRT = MiniMap.instance.CreateTrashIcon(transform.position);
    }

    private void OnDestroy()
    {
        if (m_arrow != null)
        {
            Destroy(m_arrow.gameObject);
        }
        if (m_iconRT != null)
        {
            MiniMap.instance.DeleteTrashIcon(m_iconRT);
        }
    }

    public void SetTrashArrow(TrashArrow arrow)
    {
        m_arrow = arrow;
    }

    public void SetShowHPGauge(bool show)
    {
        m_hpGauge.SetShow(show);
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

            if (m_elapsedTime >= m_effectTime)
            {
                PlayDamageEffect();
                m_elapsedTime -= m_effectTime;
            }
            m_elapsedTime += Time.deltaTime;

            if (m_seElapsedTime >= m_seTime)
            {
                Instantiate(m_damageSEPrefab, transform.position, transform.rotation);
                m_seElapsedTime -= m_seTime;
            }
            m_seElapsedTime += Time.deltaTime;
        }
    }

    private void PlayDamageEffect()
    {
        Vector3 offset = Vector3.zero;
        offset.x += UnityEngine.Random.Range(-1f, 1f);
        offset.z += UnityEngine.Random.Range(-1f, 1f);
        offset.y += UnityEngine.Random.Range(0f, 2f);
        offset = transform.TransformVector(offset);
        Instantiate(m_damageEfffectPrefab, transform.position + offset, Quaternion.identity, transform);
    }

    public void Death()
    {
        Destroy(m_hpGauge.gameObject);
        Destroy(gameObject);
        Instantiate(m_deathEffectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(m_deathSEPrefab, transform.position, transform.rotation);
    }

    private void LateUpdate()
    {
        Vector3 vec = m_mainCamera.transform.position - transform.position;
        vec.y = 0f;
        transform.rotation = Quaternion.LookRotation(vec);

        if (TrashManager.trashCount <= 10)
        {
            var vpos = m_mainCamera.WorldToViewportPoint(transform.position);
            if (0f <= vpos.x && vpos.x <= 1f &&
                0f <= vpos.y && vpos.y <= 1f &&
                vpos.z >= 0f)
            {
                m_arrow.SetShow(false);
            }
            else
            {
                var center = new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
                var spos = m_mainCamera.WorldToScreenPoint(transform.position) - center;
                if (spos.z < 0f)
                {
                    spos.x = -spos.x;
                    spos.y = -spos.y;

                    if (Mathf.Approximately(spos.y, 0f))
                    {
                        spos.y = -center.y;
                    }
                }
                float d = Mathf.Max(Mathf.Abs(spos.x / center.x), Mathf.Abs(spos.y / center.y));
                bool isOffscreen = (spos.z < 0f || d > 1f);
                if (isOffscreen)
                {
                    spos.x /= d;
                    spos.y /= d;
                    spos *= 0.95f;
                }
                m_arrow.SetPos(spos);
                m_arrow.SetShow(isOffscreen);

                if (isOffscreen)
                {
                    m_arrow.transform.eulerAngles = new Vector3
                    (
                        0f, 0f,
                        Mathf.Atan2(spos.y, spos.x) * Mathf.Rad2Deg
                    );
                }
            }
        }
        else
        {
            m_arrow.SetShow(false);
        }
    }
}

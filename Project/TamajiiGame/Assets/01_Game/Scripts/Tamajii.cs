using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Tamajii : MonoBehaviour
{
    private static Tamajii ms_instance = null;
    public static Tamajii instance { get { return ms_instance; } }

    [SerializeField]
    private float m_attackRange = 5f;
    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private RoutePath m_movePath = null;
    [SerializeField]
    private GameObject m_dustEffectPrefab = null;
    [SerializeField]
    private AttackRange m_attackRangeEff = null;

    [SerializeField]
    private float m_moveDist = 0f;
    private float m_pathDist = 0f;
    private float m_targetDist = 0f;
    private bool m_isFowardRoute = true;

    [SerializeField]
    private float m_speedDownRatio = 0.5f;
    private float m_speedRatio = 1f;
    private Vector3 m_lastPos = Vector3.zero;
    private List<Collider> m_enterPathColliders = new List<Collider>();

    private float m_effectElapsedTime = 0f;

    private AnimEvent m_animEvent = null;

    private Animator m_animator = null;

    [SerializeField]
    private AudioSource m_attackSE = null;

    private void Attack()
    {
        if (GameManager.isGameClear) return;
        m_attackSE.Play();
    }

    private void Awake()
    {
        m_animEvent = GetComponentInChildren<AnimEvent>();
        m_animEvent.onAttackEvent.AddListener(Attack);

        ms_instance = this;

        m_pathDist = m_movePath.GetPathLength();
        m_lastPos = m_movePath.GetPathPos(m_moveDist);
        m_isFowardRoute = true;
        m_targetDist = m_isFowardRoute ? m_pathDist : 0f;

        m_animator = GetComponentInChildren<Animator>();

        m_broomTF.SetParent(m_normalTF);
        m_broomTF.localPosition = Vector3.zero;
        m_broomTF.localRotation = Quaternion.identity;
    }

    private void OnDestroy()
    {
        ms_instance = null;
    }

    private PathElem GetSwitchPath()
    {
        foreach (var col in m_enterPathColliders)
        {
            var path = m_movePath.GetSwitchPath(col, m_isFowardRoute);
            if (path != null) return path;
        }
        return null;
    }

    private PathElem GetConnectPath()
    {
        foreach (var col in m_enterPathColliders)
        {
            var path = m_movePath.GetConnectPath(col, m_isFowardRoute);
            if (path != null) return path;
        }
        return null;
    }

    private void PlayDustEffect()
    {
        Vector3 offset = Vector3.zero;
        offset.x += UnityEngine.Random.Range(-2f, 2f);
        offset.z += UnityEngine.Random.Range(0f, 2f);
        offset.y += UnityEngine.Random.Range(0f, 3f);
        offset = transform.TransformVector(offset);

        Instantiate(m_dustEffectPrefab, transform.position + offset, Quaternion.identity, transform);
    }

    [SerializeField]
    private float m_effectTime = 0.0625f;

    [SerializeField]
    private Transform m_broomTF = null;
    [SerializeField]
    private Transform m_normalTF = null;
    [SerializeField]
    private Transform m_attackTF = null;

    private bool IsEndRoute()
    {
        if (m_isFowardRoute)
        {
            if (m_moveDist >= m_targetDist) return true;
        }
        else
        {
            if (m_moveDist <= m_targetDist) return true;
        }

        return false;
    }

    private void Update()
    {
        if (!GameManager.isGameStart) return;
        if (GameManager.isGameClear) return;

        // 移動方向（順ルートが逆ルートか）に合わせて、たまぢぃを移動
        float moveSpeed = m_moveSpeed * Time.deltaTime * m_speedRatio;
        float changeSpeed = 6f;

        if (Input.GetKey(KeyCode.Space))
        {
            m_attackRangeEff.SetOn(true);
            GameManager.SetOnMotionBlur(false);
            //if (m_effectElapsedTime >= m_effectTime)
            //{
            //    PlayDustEffect();
            //    m_effectElapsedTime -= m_effectTime;
            //}
            //m_effectElapsedTime += Time.deltaTime;

            m_animator.SetBool("IsAttack", true);
            m_broomTF.SetParent(m_attackTF);
            m_broomTF.localPosition = Vector3.zero;
            m_broomTF.localRotation = Quaternion.identity;

            m_speedRatio = Mathf.Lerp(m_speedRatio, m_speedDownRatio, changeSpeed * Time.deltaTime);

            TrashManager.TakeDamage(transform.position, m_attackRange);
        }
        else
        {
            if (m_attackSE.isPlaying)
            {
                m_attackSE.Stop();
            }
            m_attackRangeEff.SetOn(false);
            GameManager.SetOnMotionBlur(true);
            m_effectElapsedTime = 0f;

            m_animator.SetBool("IsAttack", false);
            m_broomTF.SetParent(m_normalTF);
            m_broomTF.localPosition = Vector3.zero;
            m_broomTF.localRotation = Quaternion.identity;

            m_speedRatio = Mathf.Lerp(m_speedRatio, 1f, changeSpeed * Time.deltaTime);
        }

        m_moveDist += m_isFowardRoute ? moveSpeed : -moveSpeed;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var switchPath = GetSwitchPath();
            if (switchPath != null && switchPath.path != null)
            {
                var routePath = switchPath.path.GetComponent<RoutePath>();
                if (routePath != null)
                {
                    m_movePath = routePath;
                    m_pathDist = m_movePath.GetPathLength();
                    m_moveDist = m_pathDist * switchPath.pos;
                    m_isFowardRoute = switchPath.isForwardRoute;
                    m_targetDist = m_isFowardRoute ? m_pathDist : 0f;
                    if (!m_isFowardRoute && Mathf.Abs(m_moveDist) <= 0.1f)
                    {
                        m_moveDist = m_pathDist;
                    }
                    else if (m_isFowardRoute && m_moveDist >= m_pathDist - 0.1f)
                    {
                        m_moveDist = 0f;
                    }
                }
            }
        }

        // 移動パスの範囲外に移動した場合
        if (IsEndRoute())
        {
            // 接続先の移動パスが存在するかどうか
            bool isConnect = false;
            var connectPath = GetConnectPath();
            if (connectPath != null && connectPath.path != null)
            {
                var routePath = connectPath.path.GetComponent<RoutePath>();
                if (routePath != null)
                {
                    m_movePath = routePath;
                    m_pathDist = m_movePath.GetPathLength();
                    m_moveDist = m_pathDist * connectPath.pos;
                    m_isFowardRoute = connectPath.isForwardRoute;
                    m_targetDist = m_isFowardRoute ? m_pathDist : 0f;
                    if (!m_isFowardRoute && Mathf.Abs(m_moveDist) <= 0.01f)
                    {
                        m_moveDist = m_pathDist;
                    }
                    else if (m_isFowardRoute && m_moveDist >= m_pathDist - 0.01f)
                    {
                        m_moveDist = 0f;
                    }
                    isConnect = true;
                }
            }

            // 移動パスを変更していなければ、
            if (!isConnect)
            {
                // 順ルートなら、パスの先頭へ戻す
                if (m_isFowardRoute) m_moveDist -= m_pathDist;
                // 逆ルートなら、パスの最後へ戻す
                else m_moveDist += m_pathDist;
            }
        }

        // たまぢぃの位置を更新
        Vector3 pos = m_movePath.GetPathPos(m_moveDist);
        transform.position = pos;

        // たまぢぃを移動方向へ向ける
        Vector3 vec = transform.position - m_lastPos;
        if (vec.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation,
                Quaternion.LookRotation(vec),
                180f * Time.deltaTime
            );
        }
        m_lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CrossArea"))
        {
            m_enterPathColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CrossArea"))
        {
            m_enterPathColliders.Remove(other);
        }
    }
}

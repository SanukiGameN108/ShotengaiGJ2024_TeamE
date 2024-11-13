using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Tamajii : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 10f;
    [SerializeField]
    private RoutePath m_movePath = null;
    [SerializeField]
    private GameObject m_dustEffectPrefab = null;

    [SerializeField]
    private float m_moveDist = 0f;
    private float m_pathDist = 0f;
    private bool m_isFowardRoute = true;

    [SerializeField]
    private float m_speedDownRatio = 0.5f;
    private float m_speedRatio = 1f;
    private Vector3 m_lastPos = Vector3.zero;
    private List<Collider> m_enterPathColliders = new List<Collider>();

    private float m_effectElapsedTime = 0f;

    private Animator m_animator = null;

    private void Awake()
    {
        m_pathDist = m_movePath.GetPathLength();
        m_lastPos = m_movePath.GetPathPos(m_moveDist);

        m_animator = GetComponentInChildren<Animator>();

        m_broomTF.SetParent(m_normalTF);
        m_broomTF.localPosition = Vector3.zero;
        m_broomTF.localRotation = Quaternion.identity;
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

    private void Update()
    {
        if (!GameManager.isGameStart) return;

        // 移動方向（順ルートが逆ルートか）に合わせて、たまぢぃを移動
        float moveSpeed = m_moveSpeed * Time.deltaTime * m_speedRatio;
        float changeSpeed = 6f;

        if (Input.GetKey(KeyCode.Space))
        {
            if (m_effectElapsedTime >= m_effectTime)
            {
                PlayDustEffect();
                m_effectElapsedTime -= m_effectTime;
            }
            m_effectElapsedTime += Time.deltaTime;

            m_animator.SetBool("IsAttack", true);
            m_broomTF.SetParent(m_attackTF);
            m_broomTF.localPosition = Vector3.zero;
            m_broomTF.localRotation = Quaternion.identity;

            m_speedRatio = Mathf.Lerp(m_speedRatio, m_speedDownRatio, changeSpeed * Time.deltaTime);

            TrashManager.TakeDamage(transform.position);
        }
        else
        {
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
                }
            }
        }

        // 移動パスの範囲外に移動した場合
        if (!(0f <= m_moveDist && m_moveDist <= m_pathDist))
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
                    isConnect = true;
                }
            }

            // 移動パスを変更していなければ、現在の移動パスの先頭に戻す
            if (!isConnect)
            {
                m_moveDist -= m_pathDist;
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

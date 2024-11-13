using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class SwitchData
{
    public CinemachinePathBase fromPath = null;
    public bool isForwardRouteFromPath = true;

    public CinemachinePathBase toPath = null;
    [Range(0f, 1f)] public float toPos = 0f;
    public bool isForwardRouteToPath = true;
}

public class SwitchArea : MonoBehaviour
{
    [SerializeField]
    private SwitchData m_switchData = new SwitchData();

    public SwitchData GetSwitchData(CinemachinePathBase from, bool isForwardRoute)
    {
        if (from != m_switchData.fromPath ||
            isForwardRoute != m_switchData.isForwardRouteFromPath) return null;
        return m_switchData;
    }

    private void OnValidate()
    {
        if (m_switchData.toPath != null)
        {
            m_switchData.toPos = Mathf.Clamp01(m_switchData.toPos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = transform.lossyScale;
        Vector3 center = transform.position + new Vector3(0f, size.y * 0.5f, 0f);
        Color color = Color.blue;
        color.a = 0.75f;
        Gizmos.color = color;
        Gizmos.DrawCube(center, size);

        if (m_switchData.toPath != null)
        {
            Vector3 pos = m_switchData.toPath.EvaluatePositionAtUnit
            (
                m_switchData.toPos,
                CinemachinePathBase.PositionUnits.Normalized
            );

            color = Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}

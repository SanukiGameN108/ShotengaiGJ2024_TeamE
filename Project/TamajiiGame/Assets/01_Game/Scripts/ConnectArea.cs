using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class ConnectData
{
    public CinemachinePathBase path = null;
    [Range(0f, 1f)] public float pos = 0f;
    public bool isForwardRoute = true;
}

public class ConnectArea : MonoBehaviour
{
    [SerializeField]
    private ConnectData m_connectData0 = new ConnectData();
    [SerializeField]
    private ConnectData m_connectData1 = new ConnectData();

    public ConnectData GetConnectData(CinemachinePathBase path)
    {
        if (path == null) return null;
        if (m_connectData0.path == path) return m_connectData1;
        if (m_connectData1.path == path) return m_connectData0;
        return null;
    }

    private void OnValidate()
    {
        for (int i = 0; i < 2; i++)
        {
            ConnectData data = i == 0 ? m_connectData0 : m_connectData1;
            if (data.path != null)
            {
                data.pos = Mathf.Clamp01(data.pos);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = transform.lossyScale;
        Vector3 center = transform.position + new Vector3(0f, size.y * 0.5f, 0f);
        Color color = Color.green;
        color.a = 0.75f;
        Gizmos.color = color;
        Gizmos.DrawCube(center, size);

        for (int i = 0; i < 2; i++)
        {
            ConnectData data = i == 0 ? m_connectData0 : m_connectData1;
            if (data.path != null)
            {
                Vector3 pos = data.path.EvaluatePositionAtUnit
                (
                    data.pos,
                    CinemachinePathBase.PositionUnits.Normalized
                );

                color = i == 0 ? Color.red : Color.blue;
                color.a = 0.5f;
                Gizmos.color = color;
                Gizmos.DrawSphere(pos, 0.5f);
            }
        }
    }
}

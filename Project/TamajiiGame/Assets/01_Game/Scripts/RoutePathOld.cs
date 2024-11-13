using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class PathElemOld
{
    public CinemachinePathBase path = null;
    [Range(0f, 1f)] public float pos = 0f;
    public bool isForwardRoute = true;
}

[System.Serializable]
public class PathDataOld
{
    public PathElemOld connectPath = new PathElemOld();
    public PathElemOld switchPath = new PathElemOld();
}

[System.Serializable]
public class CrossDataOld
{
    [Range(0f, 1f)] public float pos = 0f;
    public PathDataOld forward = new PathDataOld();
    public PathDataOld reverse = new PathDataOld();
    public Collider collider = null;
}

public class RoutePathOld : MonoBehaviour
{
    [SerializeField]
    private CinemachinePathBase m_path = null;
    [SerializeField]
    private List<CrossDataOld> m_crossData = new List<CrossDataOld>();

    private void Awake()
    {
        m_path = GetComponent<CinemachinePathBase>();
        CreateCollider();
    }

    private void CreateCollider()
    {
        foreach (var data in m_crossData)
        {
            if (data.forward.connectPath    != null ||
                data.forward.switchPath     != null ||
                data.reverse.connectPath    != null ||
                data.reverse.switchPath     != null)
            {
                var obj = new GameObject(gameObject.name + "_Col");
                obj.transform.SetParent(transform);
                obj.transform.position = GetPathPos(GetPathLength() * data.pos);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one * 10f;
                obj.layer = LayerMask.NameToLayer("CrossArea");

                var col = obj.AddComponent<BoxCollider>();
                col.center = new Vector3(0f, 0.5f, 0f);
                col.isTrigger = true;
                data.collider = col;
            }
        }
    }

    public PathElemOld GetSwitchPath(Collider col, bool isForward)
    {
        var find = m_crossData.Find(_ => _.collider == col);
        if (find == null) return null;

        return isForward ? find.forward.switchPath : find.reverse.switchPath;
    }

    public PathElemOld GetConnectPath(Collider col, bool isForward)
    {
        var find = m_crossData.Find(_ => _.collider == col);
        if (find == null) return null;

        return isForward ? find.forward.connectPath : find.reverse.connectPath;
    }

    public CinemachinePathBase GetPath()
    {
        return m_path;
    }

    public float GetPathLength()
    {
        return m_path.PathLength;
    }

    public Vector3 GetPathPos(float dist)
    {
        return m_path.EvaluatePositionAtUnit(dist, CinemachinePathBase.PositionUnits.Distance);
    }

    public Quaternion GetPathRot(float dist)
    {
        return m_path.EvaluateOrientationAtUnit(dist, CinemachinePathBase.PositionUnits.Distance);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_path == null)
        {
            m_path = GetComponent<CinemachinePathBase>();
            if (m_path == null) return;
        }

        foreach (var data in m_crossData)
        {
            if (data.forward.connectPath.path == null &&
                data.forward.switchPath.path == null &&
                data.reverse.connectPath.path == null &&
                data.reverse.switchPath.path == null) continue;

            Vector3 size = Vector3.one * 15f;
            Vector3 pos = m_path.EvaluatePositionAtUnit(data.pos, CinemachinePathBase.PositionUnits.Normalized);
            Vector3 center = pos + new Vector3(0f, size.y * 0.5f, 0f);
            Color color = Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
            Gizmos.DrawSphere(center, 0.5f);

            DrawGizmosPath(data.forward.connectPath, Color.cyan, size.y);
            DrawGizmosPath(data.forward.switchPath, Color.red, size.y);
            DrawGizmosPath(data.reverse.connectPath, Color.cyan, size.y);
            DrawGizmosPath(data.reverse.switchPath, Color.red, size.y);
        }
    }

    private void DrawGizmosPath(PathElemOld pathElem, Color color, float size)
    {
        if (pathElem.path == null) return;

        Vector3 startPos = pathElem.path.EvaluatePositionAtUnit
        (
            pathElem.pos,
            CinemachinePathBase.PositionUnits.Normalized
        ) + Vector3.up * size * 0.5f;
        float end = pathElem.pos + (pathElem.isForwardRoute ? 0.01f : -0.01f);
        Vector3 endPos = pathElem.path.EvaluatePositionAtUnit
        (
            end,
            CinemachinePathBase.PositionUnits.Normalized
        ) + Vector3.up * size * 0.5f;

        Vector3 dir = (endPos - startPos).normalized;
        Vector3 gpos = startPos + dir * 10f;
        Gizmos.color = color;
        Gizmos.DrawSphere(startPos, 0.5f);
        Gizmos.DrawLine(startPos, gpos);
        GizmosUtility.DrawArrow(gpos, dir, 2f);
    }
}

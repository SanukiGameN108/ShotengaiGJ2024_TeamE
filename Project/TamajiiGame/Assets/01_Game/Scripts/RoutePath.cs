using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class PathElem
{
    public bool on = false;
    public int no = 0;
    public CinemachinePathBase path = null;
    [Range(0f, 1f)] public float pos = 0f;
    public bool isForwardRoute = true;
}

[System.Serializable]
public class PathData
{
    public PathElem connectPath = new PathElem();
    public PathElem switchPath = new PathElem();
}

[System.Serializable]
public class CrossData
{
    public bool foldout = false;
    [Range(0f, 1f)] public float pos = 0f;
    public bool forwardOn = false;
    public bool reverseOn = false;
    public PathData forward = new PathData();
    public PathData reverse = new PathData();
    public Collider collider = null;
    public float colSize = 15f;

    public PathElem GetConnectPath(bool isForward)
    {
        if (isForward)
        {
            if (!forwardOn) return null;
            if (!forward.connectPath.on) return null;
            return forward.connectPath;
        }
        else
        {
            if (!reverseOn) return null;
            if (!reverse.connectPath.on) return null;
            return reverse.connectPath;
        }
    }

    public PathElem GetSwitchPath(bool isForward)
    {
        if (isForward)
        {
            if (!forwardOn) return null;
            if (!forward.switchPath.on) return null;
            return forward.switchPath;
        }
        else
        {
            if (!reverseOn) return null;
            if (!reverse.switchPath.on) return null;
            return reverse.switchPath;
        }
    }
}

public class RoutePath : MonoBehaviour
{
    public bool foldout = false;
    public int lastCount = 0;
    [SerializeField]
    private CinemachinePathBase m_path = null;
    public CinemachinePathBase path { get { return m_path; } }
    [SerializeField]
    public CrossData[] m_crossData;
    public CrossData[] crossData { get { return m_crossData; } }

    private void Awake()
    {
        m_path = GetComponent<CinemachinePathBase>();
    }

    private CinemachinePathBase GetPath(int no)
    {
        var routePaths = RoutePathManager.routePaths;
        if (!(0 <= no && no < routePaths.Count)) return null;
        return routePaths[no].path;
    }

    private void SetupPath(PathElem elem, Vector3 pos)
    {
        elem.path = GetPath(elem.no);
        if (elem.path != null)
        {
            var findPos = elem.path.FindClosestPoint(pos, 0, -1, 20);
            elem.pos = elem.path.FromPathNativeUnits(findPos, CinemachinePathBase.PositionUnits.Normalized);
        }
    }

    public void Setup()
    {
        if (m_crossData == null) return;

        var routePaths = RoutePathManager.routePaths;

        foreach (var data in m_crossData)
        {
            Vector3 pos = m_path.EvaluatePositionAtUnit
            (
                data.pos,
                CinemachinePathBase.PositionUnits.Normalized
            );
            if (data.forwardOn)
            {
                SetupPath(data.forward.connectPath, pos);
                SetupPath(data.forward.switchPath, pos);
            }
            if (data.reverseOn)
            {
                SetupPath(data.reverse.connectPath, pos);
                SetupPath(data.reverse.switchPath, pos);
            }
        }

        CreateCollider();
    }

    private void CreateCollider()
    {
        foreach (var data in m_crossData)
        {
            if (data.GetConnectPath(true) != null   ||
                data.GetSwitchPath(true) != null    ||
                data.GetConnectPath(false) != null  ||
                data.GetSwitchPath(false) != null)
            {
                var obj = new GameObject(gameObject.name + "_Col");
                obj.transform.SetParent(transform);
                obj.transform.position = GetPathPos(GetPathLength() * data.pos);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one * data.colSize;
                obj.layer = LayerMask.NameToLayer("CrossArea");

                var col = obj.AddComponent<BoxCollider>();
                col.center = new Vector3(0f, 0.5f, 0f);
                col.isTrigger = true;
                data.collider = col;
            }
        }
    }

    public PathElem GetSwitchPath(Collider col, bool isForward)
    {
        var find = System.Array.Find(m_crossData, _ => _.collider == col);
        if (find == null) return null;

        return find.GetSwitchPath(isForward);
    }

    public PathElem GetConnectPath(Collider col, bool isForward)
    {
        var find = System.Array.Find(m_crossData, _ => _.collider == col);
        if (find == null) return null;

        return find.GetConnectPath(isForward);
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

    private bool IsDrawGizmos(CrossData data)
    {
        if (data.forwardOn)
        {
            if (data.forward.connectPath.on ||
                data.forward.switchPath.on) return true;
        }
        if (data.reverseOn)
        {
            if (data.reverse.connectPath.on ||
                data.reverse.switchPath.on) return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_path == null)
        {
            m_path = GetComponent<CinemachinePathBase>();
            if (m_path == null) return;
        }

        if (m_crossData == null) return;
        foreach (var data in m_crossData)
        {
            if (!IsDrawGizmos(data)) continue;

            Vector3 size = Vector3.one * data.colSize;
            Vector3 pos = m_path.EvaluatePositionAtUnit(data.pos, CinemachinePathBase.PositionUnits.Normalized);
            Vector3 center = pos + new Vector3(0f, size.y * 0.5f, 0f);
            Color color = Color.blue;
            color.a = 0.75f;
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
            Gizmos.DrawSphere(center, 0.5f);

            //DrawGizmosPath(data.forward.connectPath, Color.cyan, size.y);
            //DrawGizmosPath(data.forward.switchPath, Color.red, size.y);
            //DrawGizmosPath(data.reverse.connectPath, Color.cyan, size.y);
            //DrawGizmosPath(data.reverse.switchPath, Color.red, size.y);
        }
    }

    private void DrawGizmosPath(PathElem PathElem1, Color color, float size)
    {
        if (PathElem1.path == null) return;

        Vector3 startPos = PathElem1.path.EvaluatePositionAtUnit
        (
            PathElem1.pos,
            CinemachinePathBase.PositionUnits.Normalized
        ) + Vector3.up * size * 0.5f;
        float end = PathElem1.pos + (PathElem1.isForwardRoute ? 0.01f : -0.01f);
        Vector3 endPos = PathElem1.path.EvaluatePositionAtUnit
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

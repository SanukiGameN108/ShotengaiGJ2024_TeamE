using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePathManager : MonoBehaviour
{
    private static RoutePathManager ms_instance = null;

    [SerializeField]
    private List<RoutePath> m_routePaths = new List<RoutePath>();
    public static List<RoutePath> routePaths
    {
        get
        {
            if (ms_instance == null) return null;
            return ms_instance.m_routePaths;
        }
    }

    private void Awake()
    {
        ms_instance = this;
        m_routePaths.AddRange(GetComponentsInChildren<RoutePath>());
    }

    private void OnDestroy()
    {
        ms_instance = null;
    }

    private void Start()
    {
        foreach (var route in m_routePaths)
        {
            route.Setup();
        }
    }

    public static bool GetRandomData(out Vector3 outPos, out Quaternion outRot)
    {
        outPos = Vector3.zero;
        outRot = Quaternion.identity;

        if (ms_instance.m_routePaths.Count == 0) return false;

        int index = UnityEngine.Random.Range(0, ms_instance.m_routePaths.Count);
        var path = ms_instance.m_routePaths[index];
        var length = path.GetPathLength();
        var ratio = UnityEngine.Random.Range(0f, 1f);
        var dist = length * ratio;
        var pos = path.GetPathPos(dist);
        var rot = path.GetPathRot(dist);
        var side = rot * Vector3.right;
        var offset = side * UnityEngine.Random.Range(-4f, 4f);

        outPos = pos + offset;
        outRot = rot;
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    private static TrashManager ms_instance = null;

    [SerializeField]
    private TrashUI m_trashUI = null;
    [SerializeField]
    private int m_createTrashCount = 50;
    [SerializeField]
    private List<GameObject> m_trashPrefabs = new List<GameObject>();

    private List<Trash> m_trashes = new List<Trash>();

    private void Awake()
    {
        ms_instance = this;
    }

    private void OnDestroy()
    {
        ms_instance = null;
    }

    private void Start()
    {
        for (int i = 0; i < m_createTrashCount; i++)
        {
            Vector3 pos;
            Quaternion rot;
            bool success = RoutePathManager.GetRandomData(out pos, out rot);
            if (!success) break;

            var index = UnityEngine.Random.Range(0, m_trashPrefabs.Count);
            var obj = Instantiate
            (
                m_trashPrefabs[index],
                pos,
                rot,
                transform
            );
            var trash = obj.GetComponent<Trash>();
            if (trash != null)
            {
                m_trashes.Add(trash);
            }
        }
    }

    public static void TakeDamage(Vector3 pos)
    {
        if (ms_instance == null) return;
        ms_instance.m_trashes.RemoveAll(_ => _ == null);
        foreach (var trash in ms_instance.m_trashes)
        {
            if (trash == null) continue;
            var dist = Vector3.Distance(pos, trash.transform.position);
            if (dist <= 5f)
            {
                trash.TakeDamage(Time.deltaTime);
            }
        }
    }

    private void Update()
    {
        m_trashUI.SetRemain(m_trashes.Count, m_createTrashCount);
    }
}

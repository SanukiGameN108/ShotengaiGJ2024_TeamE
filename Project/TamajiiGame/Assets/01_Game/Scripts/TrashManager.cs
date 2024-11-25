using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    private static TrashManager ms_instance = null;

    [SerializeField]
    private float m_dispHpGaugeRange = 20f;
    [SerializeField]
    private TrashUI m_trashUI = null;
    [SerializeField]
    private int m_createTrashCount = 50;
    [SerializeField]
    private List<GameObject> m_trashPrefabs = new List<GameObject>();
    [SerializeField]
    private Transform m_trashArrowTF = null;
    [SerializeField]
    private GameObject m_trashArrowPrefab = null;

    private List<Trash> m_trashes = new List<Trash>();
    public static int trashCount
    {
        get
        {
            if (ms_instance == null) return 0;
            return ms_instance.m_trashes.Count;
        }
    }
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
                var obj2 = Instantiate(m_trashArrowPrefab, m_trashArrowTF);
                var arrow = obj2.GetComponent<TrashArrow>();
                trash.SetTrashArrow(arrow);
                m_trashes.Add(trash);
            }
        }
    }

    public static void TakeDamage(Vector3 pos, float range)
    {
        if (ms_instance == null) return;
        ms_instance.m_trashes.RemoveAll(_ => _ == null);
        foreach (var trash in ms_instance.m_trashes)
        {
            if (trash == null) continue;
            var dist = Vector3.Distance(pos, trash.transform.position);
            if (dist <= range)
            {
                trash.TakeDamage(Time.deltaTime);
            }
        }
    }

    private void Update()
    {
        m_trashUI.SetRemain(m_trashes.Count, m_createTrashCount);

        var tamajiiPos = Tamajii.instance.transform.position;
        foreach (var trash in m_trashes)
        {
            if (trash == null) continue;
            var pos = trash.transform.position;
            var dist = Vector3.Distance(tamajiiPos, pos);
            trash.SetShowHPGauge(dist <= m_dispHpGaugeRange);
        }
    }
}

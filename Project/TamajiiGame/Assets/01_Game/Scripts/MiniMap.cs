using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private static MiniMap ms_instance = null;
    public static MiniMap instance { get { return ms_instance; } }

    [System.Serializable]
    private class ScaleData
    {
        public Vector2 mapPos = Vector2.zero;
        public Vector2 worldPos = Vector2.zero;
    }
    [SerializeField]
    private ScaleData m_scaleData0 = new ScaleData();
    [SerializeField]
    private ScaleData m_scaleData1 = new ScaleData();

    [SerializeField]
    private Transform m_playerTF = null;
    [SerializeField]
    private RectTransform m_playerIconRT = null;
    [SerializeField]
    private RectTransform m_mapRT = null;
    [SerializeField]
    private Image m_mapImg = null;
    [SerializeField]
    private RectTransform m_trashesRT = null;
    [SerializeField]
    private GameObject m_trashIconPrefab = null;

    private List<RectTransform> m_trashIconRTs = new List<RectTransform>();
    private float m_mapRatio = 0f;
    private Vector2 m_mapSizeRatio = Vector2.one;

    private void Awake()
    {
        ms_instance = this;
        Vector2 diffM = m_scaleData1.mapPos - m_scaleData0.mapPos;
        Vector2 diffW = m_scaleData1.worldPos - m_scaleData0.worldPos;
        if (Mathf.Abs(diffW.x) >= Mathf.Abs(diffW.y))
        {
            m_mapRatio = diffM.x / diffW.x;
        }
        else
        {
            m_mapRatio = diffM.y / diffW.y;
        }

        var tex = m_mapImg.sprite.texture;
        var texSize = new Vector2Int(tex.width, tex.height);
        var imgSize = m_mapRT.sizeDelta;
        m_mapSizeRatio = imgSize / texSize;
    }

    private void OnDestroy()
    {
        ms_instance = null;
    }

    private Vector2 CalcMapPos(Vector3 worldPos)
    {
        Vector2 wpos = new Vector2(worldPos.x, worldPos.z);
        Vector2 diff = wpos - m_scaleData0.worldPos;
        return m_scaleData0.mapPos + diff * m_mapRatio;
    }

    public RectTransform CreateTrashIcon(Vector3 pos)
    {
        var obj = Instantiate(m_trashIconPrefab, m_trashesRT);

        var mapPos = CalcMapPos(pos) * m_mapSizeRatio;

        var rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = mapPos;
        m_trashIconRTs.Add(rt);

        return rt;
    }

    public void DeleteTrashIcon(RectTransform rt)
    {
        m_trashIconRTs.Remove(rt);
        Destroy(rt.gameObject);
    }

    private void LateUpdate()
    {
        var playerPos = m_playerTF.position;
        var mapPos = -CalcMapPos(playerPos) * m_mapSizeRatio * m_mapRT.localScale;
        m_mapRT.anchoredPosition = mapPos;

        var angleY = m_playerTF.eulerAngles.y;
        m_playerIconRT.rotation = Quaternion.Euler(0f, 0f, -angleY);
    }
}

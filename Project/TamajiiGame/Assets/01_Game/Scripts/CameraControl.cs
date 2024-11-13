using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform m_followTargetTF = null;

    private void LateUpdate()
    {
        //Vector3 pos = transform.position;
        //transform.position = Vector3.Lerp(pos, m_followTargetTF.position, 12f * Time.deltaTime);
        transform.position = m_followTargetTF.position;

        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(rot, m_followTargetTF.rotation, 180f * Time.deltaTime);
    }
}

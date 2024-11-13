using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtility
{
    // ����\��
    public static void DrawArrow(Vector3 pos, Vector3 dir, float size)
    {
        // �����x�N�g�������]�l�i�N�H�[�^�j�I���j�����߂�
        Quaternion rot = Quaternion.LookRotation(dir);

        // ���̒�ʂ̒��_�z��
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f),
            new Vector3( 0.5f, -0.5f, 0f)
        };
        // ���̐�[�̒��_
        Vector3 topVtx = new Vector3(0f, 0f, 2f);
        topVtx = pos + rot * topVtx * size;

        // ����`��
        for (int i = 0; i < 4; i++)
        {
            // ��ʂ�`��
            Vector3 current = pos + rot * vertices[i] * size;
            Vector3 next = pos + rot * vertices[(i + 1) % 4] * size;
            Gizmos.DrawLine(current, next);

            // �e���_������̐�[�܂ł̐�������
            Gizmos.DrawLine(current, topVtx);
        }
    }

    // �~����̍��W�����߂�
    private static Vector3 CalcCircumferencePos(float angle, float radius)
    {
        Vector3 ret = Vector3.zero;
        ret.x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        ret.z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        return ret;
    }

    // ��`�̐}�`��`��
    public static void DrawSector(Vector3 pos, Vector3 dir, float angle, float length)
    {
        Quaternion rot = Quaternion.LookRotation(dir);

        // ��`�̌ʂ̕�����
        int div = Mathf.CeilToInt(64 * angle * 2f / 360f);
        div = Mathf.Max(div, 2); // �������̍Œᐔ��2��

        // ��`�̌ʂ̒��_�����߂�
        Vector3[] vertices = new Vector3[div];
        for (int i = 0; i < div; i++)
        {
            float percent = (float)i / (div - 1);
            float calcAng = Mathf.Lerp(-angle, angle, percent);
            Vector3 vtx = CalcCircumferencePos(calcAng, length);
            vertices[i] = pos + rot * vtx;
        }
        // ��`�̌ʂ�`��
        Gizmos.DrawLineStrip(vertices, false);

        // ���S���W�����`�̌ʂ̎n�_�ƏI�_�Ɍ����Đ���`��
        Vector3 start = CalcCircumferencePos(-angle, length);
        Vector3 end = CalcCircumferencePos(angle, length);
        Gizmos.DrawLine(pos, pos + rot * start);
        Gizmos.DrawLine(pos, pos + rot * end);
    }

    // �~�̕`��
    public static void DrawCircle(Vector3 pos, float radius)
    {
        const int div = 64;
        for (int i = 0; i <= div; i++)
        {
            float angleS = 360f * ((float)i / div) * Mathf.Deg2Rad;
            float angleE = 360f * ((float)(i + 1) / div) * Mathf.Deg2Rad;
            Vector3 start = new Vector3(Mathf.Cos(angleS), 0f, Mathf.Sin(angleS));
            Vector3 end = new Vector3(Mathf.Cos(angleE), 0f, Mathf.Sin(angleE));
            Gizmos.DrawLine(pos + start * radius, pos + end * radius);
        }
    }

    // ���̕`��
    public static void DrawPipe(Vector3 pos, float radius, float height)
    {
        const int div = 64;
        for (int i = 0; i <= div; i++)
        {
            float angleS = 360f * ((float)i / div) * Mathf.Deg2Rad;
            float angleE = 360f * ((float)(i + 1) / div) * Mathf.Deg2Rad;
            Vector3 start = new Vector3(Mathf.Cos(angleS), 0f, Mathf.Sin(angleS));
            Vector3 end = new Vector3(Mathf.Cos(angleE), 0f, Mathf.Sin(angleE));

            Vector3 spos = pos + new Vector3(0f, height * 0.5f, 0f);
            Vector3 epos = pos + new Vector3(0f, -height * 0.5f, 0f);
            Gizmos.DrawLine(spos + start * radius, spos + end * radius);
            Gizmos.DrawLine(epos + start * radius, epos + end * radius);
            Gizmos.DrawLine(spos + start * radius, epos + start * radius);
        }
    }
}

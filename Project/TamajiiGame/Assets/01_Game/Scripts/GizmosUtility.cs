using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtility
{
    // 矢印を表示
    public static void DrawArrow(Vector3 pos, Vector3 dir, float size)
    {
        // 方向ベクトルから回転値（クォータニオン）を求める
        Quaternion rot = Quaternion.LookRotation(dir);

        // 矢印の底面の頂点配列
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f),
            new Vector3( 0.5f, -0.5f, 0f)
        };
        // 矢印の先端の頂点
        Vector3 topVtx = new Vector3(0f, 0f, 2f);
        topVtx = pos + rot * topVtx * size;

        // 矢印を描画
        for (int i = 0; i < 4; i++)
        {
            // 底面を描画
            Vector3 current = pos + rot * vertices[i] * size;
            Vector3 next = pos + rot * vertices[(i + 1) % 4] * size;
            Gizmos.DrawLine(current, next);

            // 各頂点から矢印の先端までの線を引く
            Gizmos.DrawLine(current, topVtx);
        }
    }

    // 円周上の座標を求める
    private static Vector3 CalcCircumferencePos(float angle, float radius)
    {
        Vector3 ret = Vector3.zero;
        ret.x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        ret.z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        return ret;
    }

    // 扇形の図形を描画
    public static void DrawSector(Vector3 pos, Vector3 dir, float angle, float length)
    {
        Quaternion rot = Quaternion.LookRotation(dir);

        // 扇形の弧の分割数
        int div = Mathf.CeilToInt(64 * angle * 2f / 360f);
        div = Mathf.Max(div, 2); // 分割数の最低数は2個

        // 扇形の弧の頂点を求める
        Vector3[] vertices = new Vector3[div];
        for (int i = 0; i < div; i++)
        {
            float percent = (float)i / (div - 1);
            float calcAng = Mathf.Lerp(-angle, angle, percent);
            Vector3 vtx = CalcCircumferencePos(calcAng, length);
            vertices[i] = pos + rot * vtx;
        }
        // 扇形の弧を描画
        Gizmos.DrawLineStrip(vertices, false);

        // 中心座標から扇形の弧の始点と終点に向けて線を描画
        Vector3 start = CalcCircumferencePos(-angle, length);
        Vector3 end = CalcCircumferencePos(angle, length);
        Gizmos.DrawLine(pos, pos + rot * start);
        Gizmos.DrawLine(pos, pos + rot * end);
    }

    // 円の描画
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

    // 筒の描画
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

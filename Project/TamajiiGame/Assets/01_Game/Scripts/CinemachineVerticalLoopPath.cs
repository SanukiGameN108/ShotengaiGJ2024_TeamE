using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

[AddComponentMenu("Cinemachine/CinemachineVerticalLoopPath")]
[SaveDuringPlay]
public class CinemachineVerticalLoopPath : CinemachineSmoothPath
{
    private readonly List<Vector3> waypointPositions = new List<Vector3>();
    private readonly List<Quaternion> waypointRotations = new List<Quaternion>();

    public override Quaternion EvaluateOrientation(float pos)
    {
        // まずは本来のEvaluateOrientationと同じように処理していく
        var pathRotation = this.transform.rotation;
        if (this.m_Waypoints.Length <= 0)
        {
            return pathRotation;
        }
        var worldForward = this.EvaluateTangent(pos);
        if (worldForward.AlmostZero())
        {
            return pathRotation;
        }

        // そして本来のEvaluateOrientationと同様にLookRotationによる回転を求める
        var worldUp = pathRotation * Vector3.up;
        var baseLookRotation = Quaternion.LookRotation(worldForward, worldUp);

        // ここで、本来のEvaluateOrientationを実行して回転を得ておき...
        var baseRotation = base.EvaluateOrientation(pos);

        // そこからロール成分を抽出する
        var rollRotation = Quaternion.Inverse(baseLookRotation) * baseRotation;

        // 次に、現在のセグメントの両端のインデックスを得て...
        var t = this.GetBoundingIndices(pos, out var indexA, out var indexB) % 1.0f;

        // 各ウェイポイントでの姿勢を更新したのち、セグメント両端の回転を混合し...
        this.UpdateWaypointRotations();
        var rotation = pathRotation * Quaternion.Slerp(this.waypointRotations[indexA], this.waypointRotations[indexB], t);

        // その回転が指す上方向をを使ってLookRotationによる回転を求め、
        // それをロール成分と組み合わせて返す
        return Quaternion.LookRotation(worldForward, rotation * Vector3.up) * rollRotation;
    }

    // 各ウェイポイントにおける回転を求めるためのメソッドを用意する
    private void UpdateWaypointRotations()
    {
        var waypointCount = this.m_Waypoints.Length;
        var waypointCacheCount = this.waypointPositions.Count;
        var needsUpdate = waypointCount > waypointCacheCount;
        if (needsUpdate)
        {
            var waypointCountToAdd = waypointCount - waypointCacheCount;
            for (var i = 0; i < waypointCountToAdd; i++)
            {
                this.waypointPositions.Add(new Vector3());
                this.waypointRotations.Add(new Quaternion());
            }
        }
        else
        {
            for (var i = 0; i < waypointCount; i++)
            {
                if (this.m_Waypoints[i].position != this.waypointPositions[i])
                {
                    needsUpdate = true;
                    break;
                }
            }
        }

        if (waypointCount < waypointCacheCount)
        {
            var elementCountToRemove = waypointCacheCount - waypointCount;
            this.waypointPositions.RemoveRange(waypointCount, elementCountToRemove);
            this.waypointRotations.RemoveRange(waypointCount, elementCountToRemove);
        }

        if ((waypointCount == 0) || !needsUpdate)
        {
            return;
        }

        // 最初のウェイポイントは本来のCinemachineSmoothPathと同様に
        // Vector3.upを上方向とした回転を採用し...
        var inversePathRotation = Quaternion.Inverse(this.transform.rotation);
        var previousTangent = inversePathRotation * this.EvaluateTangent(0);
        this.waypointPositions[0] = this.m_Waypoints[0].position;
        this.waypointRotations[0] = Quaternion.LookRotation(previousTangent, Vector3.up);
        for (var i = 1; i < waypointCount; i++)
        {
            // 以降のウェイポイントは一つ前のウェイポイントの回転に対して
            // 前方向の変化から求めた差分回転を合成した回転を採用する
            var tangent = inversePathRotation * this.EvaluateTangent(i);
            this.waypointPositions[i] = this.m_Waypoints[i].position;
            this.waypointRotations[i] = Quaternion.FromToRotation(previousTangent, tangent) * this.waypointRotations[i - 1];
            previousTangent = tangent;
        }
    }

    // CinemachineSmoothPathのGetBoundingIndicesをそのまま持ってくる
    private float GetBoundingIndices(float pos, out int indexA, out int indexB)
    {
        pos = this.StandardizePos(pos);
        var numWaypoints = this.m_Waypoints.Length;
        if (numWaypoints < 2)
        {
            indexA = indexB = 0;
        }
        else
        {
            indexA = Mathf.FloorToInt(pos);
            if (indexA >= numWaypoints)
            {
                pos -= this.MaxPos;
                indexA = 0;
            }

            indexB = indexA + 1;
            if (indexB == numWaypoints)
            {
                if (this.Looped)
                {
                    indexB = 0;
                }
                else
                {
                    --indexB;
                    --indexA;
                }
            }
        }

        return pos;
    }
}
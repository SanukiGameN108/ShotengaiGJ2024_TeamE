using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(CinemachineVerticalLoopPath))]
internal sealed class CinemachineVerticalLoopPathEditor : BaseEditor<CinemachineVerticalLoopPath>
{
    private ReorderableList mWaypointList;

    /// <summary>Get the property names to exclude in the inspector.</summary>
    /// <param name="excluded">Add the names to this list</param>
    protected override void GetExcludedPropertiesInInspector(List<string> excluded)
    {
        base.GetExcludedPropertiesInInspector(excluded);
        excluded.Add(FieldPath(x => x.m_Waypoints));
    }

    void OnEnable()
    {
        mWaypointList = null;
    }


    // ReSharper disable once UnusedMember.Global - magic method called when doing Frame Selected
    public bool HasFrameBounds()
    {
        return Target.m_Waypoints != null && Target.m_Waypoints.Length > 0;
    }

    // ReSharper disable once UnusedMember.Global - magic method called when doing Frame Selected
    public Bounds OnGetFrameBounds()
    {
        Vector3[] wp;
        int selected = mWaypointList == null ? -1 : mWaypointList.index;
        if (selected >= 0 && selected < Target.m_Waypoints.Length)
            wp = new Vector3[1] { Target.m_Waypoints[selected].position };
        else
            wp = Target.m_Waypoints.Select(p => p.position).ToArray();
        return GeometryUtility.CalculateBounds(wp, Target.transform.localToWorldMatrix);
    }

    public override void OnInspectorGUI()
    {
        BeginInspector();
        if (mWaypointList == null)
            SetupWaypointList();

        if (mWaypointList.index >= mWaypointList.count)
            mWaypointList.index = mWaypointList.count - 1;

        // Ordinary properties
        DrawRemainingPropertiesInInspector();

        // Path length
        EditorGUILayout.LabelField("Path Length", Target.PathLength.ToString());

        // Waypoints
        EditorGUI.BeginChangeCheck();
        mWaypointList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }

    void SetupWaypointList()
    {
        mWaypointList = new ReorderableList(
                serializedObject, FindProperty(x => x.m_Waypoints),
                true, true, true, true);

        mWaypointList.drawHeaderCallback = (Rect rect) =>
        { EditorGUI.LabelField(rect, "Waypoints"); };

        mWaypointList.drawElementCallback
            = (Rect rect, int index, bool isActive, bool isFocused) =>
            { DrawWaypointEditor(rect, index); };

        mWaypointList.onAddCallback = (ReorderableList l) =>
        { InsertWaypointAtIndex(l.index); };
    }

    void DrawWaypointEditor(Rect rect, int index)
    {
        // Needed for accessing string names of fields
        CinemachineSmoothPath.Waypoint def = new CinemachineSmoothPath.Waypoint();
        SerializedProperty element = mWaypointList.serializedProperty.GetArrayElementAtIndex(index);

        float hSpace = 3;
        rect.width -= hSpace; rect.y += 1;
        Vector2 numberDimension = GUI.skin.label.CalcSize(new GUIContent("999"));
        Rect r = new Rect(rect.position, numberDimension);
        if (GUI.Button(r, new GUIContent(index.ToString(), "Go to the waypoint in the scene view")))
        {
            if (SceneView.lastActiveSceneView != null)
            {
                mWaypointList.index = index;
                SceneView.lastActiveSceneView.pivot = Target.EvaluatePosition(index);
                SceneView.lastActiveSceneView.size = 4;
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2f;
        GUIContent rollLabel = new GUIContent("Roll");
        Vector2 labelDimension = GUI.skin.label.CalcSize(rollLabel);
        float rollWidth = labelDimension.x + floatFieldWidth;
        r.x += r.width + hSpace; r.width = rect.width - (r.width + hSpace + rollWidth) - (r.height + hSpace);
        EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.position), GUIContent.none);

        r.x += r.width + hSpace; r.width = rollWidth;
        float oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = labelDimension.x;

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.roll), rollLabel);
        EditorGUIUtility.labelWidth = oldWidth;
        EditorGUI.indentLevel = indent;

        r.x += r.width + hSpace; r.height += 1; r.width = r.height;
        GUIContent setButtonContent = EditorGUIUtility.IconContent("d_RectTransform Icon");
        setButtonContent.tooltip = "Set to scene-view camera position";
        if (GUI.Button(r, setButtonContent, GUI.skin.label) && SceneView.lastActiveSceneView != null)
        {
            Undo.RecordObject(Target, "Set waypoint");
            CinemachineSmoothPath.Waypoint wp = Target.m_Waypoints[index];
            Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
            wp.position = Target.transform.InverseTransformPoint(pos);
            Target.m_Waypoints[index] = wp;
        }
    }

    void InsertWaypointAtIndex(int indexA)
    {
        Vector3 pos = Vector3.right;
        float roll = 0;

        // Get new values from the current indexA (if any)
        int numWaypoints = Target.m_Waypoints.Length;
        if (indexA < 0)
            indexA = numWaypoints - 1;
        if (indexA >= 0)
        {
            int indexB = indexA + 1;
            if (Target.m_Looped && indexB >= numWaypoints)
                indexB = 0;
            if (indexB >= numWaypoints)
            {
                Vector3 delta = Vector3.right;
                if (indexA > 0)
                    delta = Target.m_Waypoints[indexA].position - Target.m_Waypoints[indexA - 1].position;
                pos = Target.m_Waypoints[indexA].position + delta;
                roll = Target.m_Waypoints[indexA].roll;
            }
            else
            {
                // Interpolate
                pos = Target.transform.InverseTransformPoint(Target.EvaluatePosition(0.5f + indexA));
                roll = Mathf.Lerp(Target.m_Waypoints[indexA].roll, Target.m_Waypoints[indexB].roll, 0.5f);
            }
        }
        Undo.RecordObject(Target, "Add waypoint");
        var wp = new CinemachineSmoothPath.Waypoint();
        wp.position = pos;
        wp.roll = roll;
        var list = new List<CinemachineSmoothPath.Waypoint>(Target.m_Waypoints);
        list.Insert(indexA + 1, wp);
        Target.m_Waypoints = list.ToArray();
        Target.InvalidateDistanceCache();
        InspectorUtility.RepaintGameView();
        mWaypointList.index = indexA + 1; // select it
    }

    void OnSceneGUI()
    {
        if (mWaypointList == null)
            SetupWaypointList();

        if (Tools.current == Tool.Move)
        {
            Color colorOld = Handles.color;
            var localToWorld = Target.transform.localToWorldMatrix;
            for (int i = 0; i < Target.m_Waypoints.Length; ++i)
            {
                DrawSelectionHandle(i, localToWorld);
                if (mWaypointList.index == i)
                    DrawPositionControl(i, localToWorld, Target.transform.rotation); // Waypoint is selected
            }
            Handles.color = colorOld;
        }
    }

    void DrawSelectionHandle(int i, Matrix4x4 localToWorld)
    {
        if (Event.current.button != 1)
        {
            Vector3 pos = localToWorld.MultiplyPoint(Target.m_Waypoints[i].position);
            float size = HandleUtility.GetHandleSize(pos) * 0.2f;
            Handles.color = Color.white;
            if (Handles.Button(pos, Quaternion.identity, size, size, Handles.SphereHandleCap)
                && mWaypointList.index != i)
            {
                mWaypointList.index = i;
                InspectorUtility.RepaintGameView();
            }
            // Label it
            Handles.BeginGUI();
            Vector2 labelSize = new Vector2(
                    EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight);
            Vector2 labelPos = HandleUtility.WorldToGUIPoint(pos);
            labelPos.y -= labelSize.y / 2;
            labelPos.x -= labelSize.x / 2;
            GUILayout.BeginArea(new Rect(labelPos, labelSize));
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(new GUIContent(i.ToString(), "Waypoint " + i), style);
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }

    void DrawPositionControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
    {
        CinemachineSmoothPath.Waypoint wp = Target.m_Waypoints[i];
        Vector3 pos = localToWorld.MultiplyPoint(wp.position);
        EditorGUI.BeginChangeCheck();
        Handles.color = Target.m_Appearance.pathColor;
        Quaternion rotation = (Tools.pivotRotation == PivotRotation.Local)
            ? localRotation : Quaternion.identity;
        float size = HandleUtility.GetHandleSize(pos) * 0.1f;
        Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);
        pos = Handles.PositionHandle(pos, rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Move Waypoint");
            wp.position = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);
            Target.m_Waypoints[i] = wp;
            Target.InvalidateDistanceCache();
            InspectorUtility.RepaintGameView();
        }
    }

    // �������Ō��DrawGizmos��CinemachinePathEditor.DrawPathGizmo���g���Ă��邽��
    // �ێʂ��̂܂܂ł͓��삵�Ȃ�
    [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(CinemachineSmoothPath))]
    private static void DrawGizmos(CinemachineSmoothPath path, GizmoType selectionType)
    {
        DrawPathGizmo(path,
            (Selection.activeGameObject == path.gameObject)
            ? path.m_Appearance.pathColor : path.m_Appearance.inactivePathColor);
    }

    // �����ŁACinemachinePathEditor.cs����DrawPathGizmo�������Ă��āA��������g��
    private static void DrawPathGizmo(CinemachinePathBase path, Color pathColor)
    {
        // Draw the path
        var colorOld = Gizmos.color;
        Gizmos.color = pathColor;
        var step = 1f / path.m_Resolution;
        var lastPos = path.EvaluatePosition(path.MinPos);
        var lastW = (path.EvaluateOrientation(path.MinPos) * Vector3.right * path.m_Appearance.width) / 2;
        for (var t = path.MinPos + step; t <= (path.MaxPos + (step / 2)); t += step)
        {
            var p = path.EvaluatePosition(t);
            var q = path.EvaluateOrientation(t);
            var w = (q * Vector3.right * path.m_Appearance.width) / 2;
            var w2 = w * 1.2f;
            var p0 = p - w2;
            var p1 = p + w2;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(lastPos - lastW, p - w);
            Gizmos.DrawLine(lastPos + lastW, p + w);
#if false
                // Show the normals, for debugging
                Gizmos.color = Color.red;
                Vector3 y = (q * Vector3.up) * width / 2;
                Gizmos.DrawLine(p, p + y);
                Gizmos.color = pathColor;
#endif
            lastPos = p;
            lastW = w;
        }

        Gizmos.color = colorOld;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(RoutePath))]
public class RoutePathEditor : Editor
{
    private int crossDataCount = 0;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        RoutePath root = target as RoutePath;
        var crossData = root.crossData;

        serializedObject.Update();

        var crossDataArray = serializedObject.FindProperty("m_crossData");
        crossDataCount = crossDataArray.arraySize;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        root.foldout = EditorGUILayout.Foldout(root.foldout, "�؂�ւ��|�C���g");
        GUILayout.FlexibleSpace();
        crossDataCount = EditorGUILayout.IntField(crossDataCount, GUILayout.Width(48f));
        if (crossDataCount != crossDataArray.arraySize)
        {
            crossDataArray.arraySize = crossDataCount;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        EditorGUILayout.EndHorizontal();
        if (crossData != null)
        {
            if (root.lastCount < crossData.Length)
            {
                int diff = crossData.Length - root.lastCount;
                for (int i = 0; i < diff; i++)
                {
                    int index = crossData.Length - i - 1;
                    crossData[index].colSize = 15f;
                }
                root.lastCount = crossData.Length;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }
        if (root.foldout && crossData != null)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < crossData.Length; i++)
            {
                var data = crossData[i];
                var str = "�|�C���g " + i.ToString() + "�F";
                if (data.forwardOn)
                {
                    if (data.forward.connectPath.on || data.forward.switchPath.on)
                    {
                        str += "[��] ";
                        if (data.forward.connectPath.on) str += "����:" + data.forward.connectPath.no.ToString();
                        if (data.forward.switchPath.on) str += "�A�N�V����:" + data.forward.switchPath.no.ToString();
                        if (data.reverseOn && (data.reverse.connectPath.on || data.reverse.switchPath.on))
                        {
                            str += "�@|�@";
                        }
                    }
                }
                if (data.reverseOn)
                {
                    if (data.reverse.connectPath.on || data.reverse.switchPath.on)
                    {
                        str += "[�t] ";
                        if (data.reverse.connectPath.on) str += "����:" + data.reverse.connectPath.no.ToString();
                        if (data.reverse.switchPath.on) str += "�A�N�V����:" + data.reverse.switchPath.no.ToString();
                    }
                }
                data.foldout = EditorGUILayout.Foldout(data.foldout, str);
                if (data.foldout)
                {
                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical();
                    {
                        data.pos = EditorGUILayout.Slider("�ʒu", data.pos, 0f, 1f);
                        data.colSize = EditorGUILayout.FloatField("�T�C�Y", data.colSize);
                        // �����[�g
                        {
                            EditorGUIUtility.labelWidth = 160f;
                            data.forwardOn = EditorGUILayout.Toggle("�����[�g���̐؂�ւ���", data.forwardOn);
                            EditorGUIUtility.labelWidth = labelWidth;
                            if (data.forwardOn)
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.EndVertical();
                                EditorGUI.indentLevel++;
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        data.forward.connectPath.on = EditorGUILayout.Toggle("", data.forward.connectPath.on, GUILayout.Width(16f));
                                        EditorGUIUtility.labelWidth = 32f;
                                        EditorGUILayout.LabelField("�����F");
                                        if (data.forward.connectPath.on)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            data.forward.connectPath.no = EditorGUILayout.IntField("", data.forward.connectPath.no);
                                            EditorGUIUtility.labelWidth = 96f;
                                            data.forward.connectPath.isForwardRoute = EditorGUILayout.Toggle("�����[�g", data.forward.connectPath.isForwardRoute);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUIUtility.labelWidth = labelWidth;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        data.forward.switchPath.on = EditorGUILayout.Toggle("", data.forward.switchPath.on, GUILayout.Width(16f));
                                        EditorGUIUtility.labelWidth = 32f;
                                        EditorGUILayout.LabelField("�A�N�V�����F");
                                        if (data.forward.switchPath.on)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            data.forward.switchPath.no = EditorGUILayout.IntField("", data.forward.switchPath.no);
                                            EditorGUIUtility.labelWidth = 96f;
                                            data.forward.switchPath.isForwardRoute = EditorGUILayout.Toggle("�����[�g", data.forward.switchPath.isForwardRoute);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUIUtility.labelWidth = labelWidth;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUI.indentLevel--;
                                EditorGUILayout.Space();
                            }
                        }

                        // �t���[�g
                        {
                            EditorGUIUtility.labelWidth = 160f;
                            data.reverseOn = EditorGUILayout.Toggle("�t���[�g���̐؂�ւ���", data.reverseOn);
                            EditorGUIUtility.labelWidth = labelWidth;
                            if (data.reverseOn)
                            {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.EndVertical();
                                EditorGUI.indentLevel++;
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        data.reverse.connectPath.on = EditorGUILayout.Toggle("", data.reverse.connectPath.on, GUILayout.Width(16f));
                                        EditorGUIUtility.labelWidth = 32f;
                                        EditorGUILayout.LabelField("�����F");
                                        if (data.reverse.connectPath.on)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            data.reverse.connectPath.no = EditorGUILayout.IntField("", data.reverse.connectPath.no);
                                            EditorGUIUtility.labelWidth = 96f;
                                            data.reverse.connectPath.isForwardRoute = EditorGUILayout.Toggle("�����[�g", data.reverse.connectPath.isForwardRoute);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUIUtility.labelWidth = labelWidth;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        data.reverse.switchPath.on = EditorGUILayout.Toggle("", data.reverse.switchPath.on, GUILayout.Width(16f));
                                        EditorGUIUtility.labelWidth = 32f;
                                        EditorGUILayout.LabelField("�A�N�V�����F");
                                        if (data.reverse.switchPath.on)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            data.reverse.switchPath.no = EditorGUILayout.IntField("", data.reverse.switchPath.no);
                                            EditorGUIUtility.labelWidth = 96f;
                                            data.reverse.switchPath.isForwardRoute = EditorGUILayout.Toggle("�����[�g", data.reverse.switchPath.isForwardRoute);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUIUtility.labelWidth = labelWidth;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
            }
            EditorGUI.indentLevel--;
        }

        //base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(root);
            var scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

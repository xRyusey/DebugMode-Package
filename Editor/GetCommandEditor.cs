using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GetDebugCommand), true), Serializable]
public class GetCommandEditor : Editor
{
    private string commandId;
    private string commandDescription;
    private string commandFormat;

    private int parameterNum;
    private string type;

    private MonoBehaviour[] scripts;
    private MethodInfo[] infos;

    private List<string> scriptNames, functionNames;
    private int selectedScript, selectedFunction;

    private void OnEnable()
    {
        commandId = serializedObject.FindProperty("commandId").stringValue;
        commandFormat = serializedObject.FindProperty("commandFormat").stringValue;
        commandDescription = serializedObject.FindProperty("commandDescription").stringValue;

        parameterNum = serializedObject.FindProperty("parameterNum").intValue;
        type = serializedObject.FindProperty("type").stringValue;

        scripts = ((GetDebugCommand)target).gameObject.GetComponents<MonoBehaviour>();

        scriptNames = new List<string>(scripts.Length + 1) { "None" };

        for (int i = 1; i < scripts.Length + 1; i++)
        {
            if (target != scripts[i - 1])
            {
                scriptNames.Add(scripts[i - 1].GetType().Name);
            }
        }

        if (serializedObject.FindProperty("script").objectReferenceValue != null)
        {
            selectedScript = scriptNames.IndexOf(((MonoBehaviour)serializedObject.FindProperty("script").objectReferenceValue).GetType().Name);
            if (selectedScript == -1) selectedScript = 0;
        }
        else
        {
            selectedScript = 0;
        }

        if (serializedObject.FindProperty("functionName").stringValue != "")
        {
            functionNames = ((MonoBehaviour)serializedObject.FindProperty("script").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Where(m => m.ReturnType == typeof(void) && (m.GetParameters().Length == 0 || (m.GetParameters().Length > 0 &&
                (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool))) ||
                (m.GetParameters().Length > 1 && (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool)) &&
                m.GetParameters()[0].ParameterType == m.GetParameters()[1].ParameterType))).Select(m => m.Name + "()").ToList();

            selectedFunction = functionNames.IndexOf(serializedObject.FindProperty("functionName").stringValue + "()");
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        commandId = EditorGUILayout.TextField("Id", commandId);
        serializedObject.FindProperty("commandId").stringValue = commandId;

        commandDescription = EditorGUILayout.TextField("Description", commandDescription);
        serializedObject.FindProperty("commandDescription").stringValue = commandDescription;

        commandFormat = EditorGUILayout.TextField("Format", commandFormat);
        serializedObject.FindProperty("commandFormat").stringValue = commandFormat;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();

        selectedScript = EditorGUILayout.Popup("Script", selectedScript, scriptNames.ToArray());
        if (selectedScript == 0) return;

        var script = scripts[selectedScript - 1];
        serializedObject.FindProperty("script").objectReferenceValue = script;

        if (EditorGUI.EndChangeCheck())
        {
            selectedFunction = 0;

            functionNames = ((MonoBehaviour)serializedObject.FindProperty("script").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Where(m => m.ReturnType == typeof(void) && (m.GetParameters().Length == 0 || (m.GetParameters().Length > 0 &&
                (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool))) ||
                (m.GetParameters().Length > 1 && (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool)) &&
                m.GetParameters()[0].ParameterType == m.GetParameters()[1].ParameterType))).Select(m => m.Name + "()").ToList();

            infos = ((MonoBehaviour)serializedObject.FindProperty("script").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Where(m => m.ReturnType == typeof(void) && (m.GetParameters().Length == 0 || (m.GetParameters().Length > 0 &&
                (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool))) ||
                (m.GetParameters().Length > 1 && (m.GetParameters()[0].ParameterType == typeof(int) || m.GetParameters()[0].ParameterType == typeof(double) ||
                m.GetParameters()[0].ParameterType == typeof(float) || m.GetParameters()[0].ParameterType == typeof(bool)) &&
                m.GetParameters()[0].ParameterType == m.GetParameters()[1].ParameterType))).ToArray();

            if (infos.Length > 0)
            {
                parameterNum = infos[selectedFunction].GetParameters().Length;
                if (parameterNum > 0)
                {
                    type = infos[selectedFunction].GetParameters()[0].ParameterType.ToString();
                    serializedObject.FindProperty("type").stringValue = type;
                }
                serializedObject.FindProperty("parameterNum").intValue = parameterNum;
            }

            if (functionNames.Count > 0)
                ((GetDebugCommand)target).functionName = functionNames[0];
        }

        EditorGUI.BeginChangeCheck();

        selectedFunction = EditorGUILayout.Popup("Function", selectedFunction, functionNames.ToArray());

        if (EditorGUI.EndChangeCheck())
        {
            if (functionNames.Count > 0)
            {
                serializedObject.FindProperty("functionName").stringValue = functionNames[selectedFunction].Substring(0, functionNames[selectedFunction].Length - 2);
                if (infos.Length > 0)
                {
                    parameterNum = infos[selectedFunction].GetParameters().Length;
                    if (parameterNum > 0)
                    {
                        type = infos[selectedFunction].GetParameters()[0].ParameterType.ToString();
                        serializedObject.FindProperty("type").stringValue = type;
                    }
                    serializedObject.FindProperty("parameterNum").intValue = parameterNum;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

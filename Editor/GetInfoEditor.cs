using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GetDebugInfo), true), Serializable]
public class GetInfoEditor : Editor
{
    private string infoId;
    private string infoDescription;

    private int getterNum;
    private int selectedScript1, selectedScript2;
    private int fieldOrFunction1, fieldOrFunction2;
    private int selectedField1, selectedField2;
    private int selectedFunction1, selectedFunction2;

    private MonoBehaviour[] scripts;
    private List<string> names1, names2;
    private List<string> fieldNames1, fieldNames2;
    private List<string> functionNames1, functionNames2;

    void OnEnable()
    {
        infoId = serializedObject.FindProperty("infoId").stringValue;
        infoDescription = serializedObject.FindProperty("infoDescription").stringValue;

        getterNum = serializedObject.FindProperty("getterNum").intValue;

        scripts = ((GetDebugInfo)target).gameObject.GetComponents<MonoBehaviour>();

        names1 = new List<string>(scripts.Length + 1) { "None" };
        names2 = new List<string>(scripts.Length + 1) { "None" };

        for (int i = 1; i < scripts.Length + 1; i++)
        {
            if (target != scripts[i - 1])
            {
                names1.Add(scripts[i - 1].GetType().Name);
                names2.Add(scripts[i - 1].GetType().Name);
            }
        }

        if (serializedObject.FindProperty("script1").objectReferenceValue != null)
        {
            selectedScript1 = names1.IndexOf(((MonoBehaviour)serializedObject.FindProperty("script1").objectReferenceValue).GetType().Name);
            if (selectedScript1 == -1) selectedScript1 = 0;
        }
        else
        {
            selectedScript1 = 0;
        }

        if (serializedObject.FindProperty("script2").objectReferenceValue != null)
        {
            selectedScript2 = names2.IndexOf(((MonoBehaviour)serializedObject.FindProperty("script2").objectReferenceValue).GetType().Name);
            if (selectedScript2 == -1) selectedScript2 = 0;
        }
        else
        {
            selectedScript2 = 0;
        }

        if (serializedObject.FindProperty("fieldName1").stringValue != "")
        {
            fieldNames1 = ((MonoBehaviour)serializedObject.FindProperty("script1").objectReferenceValue).GetType().GetFields().Where(m => m.FieldType != typeof(void) && m.FieldType.IsValueType).Select(f => f.Name).ToList();
            selectedField1 = fieldNames1.IndexOf(serializedObject.FindProperty("fieldName1").stringValue);
            fieldOrFunction1 = 0;
        }
        else if (serializedObject.FindProperty("functionName1").stringValue != "")
        {
            functionNames1 = ((MonoBehaviour)serializedObject.FindProperty("script1").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Where(m => m.ReturnType != typeof(void) && m.ReturnType.IsValueType).Select(m => m.Name + "()").ToList();
            selectedFunction1 = functionNames1.IndexOf(serializedObject.FindProperty("functionName1").stringValue + "()");
            fieldOrFunction1 = 1;
        }

        if (serializedObject.FindProperty("fieldName2").stringValue != "")
        {
            fieldNames2 = ((MonoBehaviour)serializedObject.FindProperty("script2").objectReferenceValue).GetType().GetFields().Where(m => m.FieldType != typeof(void) && m.FieldType.IsValueType).Select(f => f.Name).ToList();
            selectedField2 = fieldNames2.IndexOf(serializedObject.FindProperty("fieldName2").stringValue);
            fieldOrFunction2 = 0;
        }
        else if (serializedObject.FindProperty("functionName2").stringValue != "")
        {
            functionNames2 = ((MonoBehaviour)serializedObject.FindProperty("script2").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Where(m => m.ReturnType != typeof(void) && m.ReturnType.IsValueType).Select(m => m.Name + "()").ToList();
            selectedFunction2 = functionNames2.IndexOf(serializedObject.FindProperty("functionName2").stringValue + "()");
            fieldOrFunction2 = 1;
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        infoId = EditorGUILayout.TextField("Id", infoId);
        serializedObject.FindProperty("infoId").stringValue = infoId;

        infoDescription = EditorGUILayout.TextField("Description", infoDescription);
        serializedObject.FindProperty("infoDescription").stringValue = infoDescription;

        EditorGUI.BeginChangeCheck();

        getterNum = EditorGUILayout.IntPopup("Number of getters ", getterNum, new string[2] { "1", "2" }, new int[2] { 1, 2 });
        serializedObject.FindProperty("getterNum").intValue = getterNum;

        EditorGUILayout.Space();

        selectedScript1 = EditorGUILayout.Popup("Script", selectedScript1, names1.ToArray());
        if (selectedScript1 == 0) return;

        var script = scripts[selectedScript1 - 1];
        serializedObject.FindProperty("script1").objectReferenceValue = script;

        fieldOrFunction1 = EditorGUILayout.Popup("Field or Function", fieldOrFunction1, new string[2] { "Field", "Function" });

        if (EditorGUI.EndChangeCheck())
        {
            selectedField1 = 0;
            selectedFunction1 = 0;
            if (fieldOrFunction1 == 0)
            {
                fieldNames1 = ((MonoBehaviour)serializedObject.FindProperty("script1").objectReferenceValue).GetType().GetFields().Where(m => m.FieldType != typeof(void) && m.FieldType.IsValueType).Select(f => f.Name).ToList();

                ((GetDebugInfo)target).functionName1 = null;
                if (fieldNames1.Count > 0) ((GetDebugInfo)target).fieldName1 = fieldNames1[selectedField1];
                selectedFunction1 = 0;
            }
            else
            {
                functionNames1 = ((MonoBehaviour)serializedObject.FindProperty("script1").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                    Where(m => m.ReturnType != typeof(void) && m.ReturnType.IsValueType).Select(m => m.Name + "()").ToList();

                ((GetDebugInfo)target).fieldName1 = null;
                if (functionNames1.Count > 0) ((GetDebugInfo)target).functionName1 = functionNames1[0];
                selectedField1 = 0;
            }
        }

        if (fieldOrFunction1 == 0)
        {
            selectedField1 = EditorGUILayout.Popup("Field", selectedField1, fieldNames1.ToArray());
            if (fieldNames1.Count > 0) serializedObject.FindProperty("fieldName1").stringValue = fieldNames1[selectedField1].Substring(0, fieldNames1[selectedField1].Length);
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            selectedFunction1 = EditorGUILayout.Popup("Function", selectedFunction1, functionNames1.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                ((GetDebugInfo)target).fieldName1 = null;
                if (functionNames1.Count > 0) serializedObject.FindProperty("functionName1").stringValue = functionNames1[selectedFunction1].Substring(0, functionNames1[selectedFunction1].Length - 2);
            }
        }

        EditorGUILayout.Space();

        if (getterNum == 2)
        {
            EditorGUI.BeginChangeCheck();
            selectedScript2 = EditorGUILayout.Popup("Script", selectedScript2, names2.ToArray());
            if (selectedScript2 == 0) return;

            script = scripts[selectedScript2 - 1];
            serializedObject.FindProperty("script2").objectReferenceValue = script;

            fieldOrFunction2 = EditorGUILayout.Popup("Field or Function", fieldOrFunction2, new string[2] { "Field", "Function" });

            if (EditorGUI.EndChangeCheck())
            {
                selectedField2 = 0;
                selectedFunction2 = 0;
                if (fieldOrFunction2 == 0)
                {
                    fieldNames2 = ((MonoBehaviour)serializedObject.FindProperty("script2").objectReferenceValue).GetType().GetFields().Where(m => m.FieldType != typeof(void) && m.FieldType.IsValueType).Select(f => f.Name).ToList();

                    ((GetDebugInfo)target).functionName2 = null;
                    if (fieldNames2.Count > 0) ((GetDebugInfo)target).fieldName2 = fieldNames2[selectedField2];
                }
                else
                {
                    functionNames2 = ((MonoBehaviour)serializedObject.FindProperty("script2").objectReferenceValue).GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                        Where(m => m.ReturnType != typeof(void) && m.ReturnType.IsValueType).Select(m => m.Name + "()").ToList();

                    ((GetDebugInfo)target).fieldName2 = null;
                    if (functionNames2.Count > 0) ((GetDebugInfo)target).functionName2 = functionNames2[0];
                }
            }

            if (fieldOrFunction2 == 0)
            {
                selectedField2 = EditorGUILayout.Popup("Field", selectedField2, fieldNames2.ToArray());
                if (fieldNames2.Count > 0) serializedObject.FindProperty("fieldName2").stringValue = fieldNames2[selectedField2].Substring(0, fieldNames2[selectedField2].Length);
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                selectedFunction2 = EditorGUILayout.Popup("Function", selectedFunction2, functionNames2.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    ((GetDebugInfo)target).fieldName2 = null;
                    if (functionNames2.Count > 0) serializedObject.FindProperty("functionName2").stringValue = functionNames2[selectedFunction2].Substring(0, functionNames2[selectedFunction2].Length - 2);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

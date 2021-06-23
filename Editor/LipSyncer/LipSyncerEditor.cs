using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LipSyncer))]
public abstract class LipSyncerEditor : Editor
{
    protected SerializedProperty parentObject, sourceAudio, sourceAudioScript, phonemeList, animationName, extendG, extendH, extendX, phonetic;
    protected bool[] arrayExpanded = new bool[3];

    bool extendedShapesGroup = true;

    protected virtual void OnEnable()
    {
        parentObject = serializedObject.FindProperty("parentObject");
        sourceAudio = serializedObject.FindProperty("sourceAudio");
        sourceAudioScript = serializedObject.FindProperty("sourceAudioScript");
        phonemeList = serializedObject.FindProperty("phonemeList");
        animationName = serializedObject.FindProperty("animationName");
        extendG = serializedObject.FindProperty("extendG");
        extendH = serializedObject.FindProperty("extendH");
        extendX = serializedObject.FindProperty("extendX");
        phonetic = serializedObject.FindProperty("phonetic");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(parentObject);
        EditorGUILayout.PropertyField(sourceAudio);
        EditorGUILayout.PropertyField(sourceAudioScript);
        EditorGUILayout.Space();

        extendedShapesGroup = EditorGUILayout.BeginToggleGroup("Extended Mouth Shapes", extendedShapesGroup);
        extendG.boolValue = EditorGUILayout.Toggle("G", extendG.boolValue);
        extendH.boolValue = EditorGUILayout.Toggle("H", extendH.boolValue);
        extendX.boolValue = EditorGUILayout.Toggle("X", extendX.boolValue);

        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(phonetic);
        EditorGUILayout.Space();
    }

    public virtual void LowerGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(animationName);
        EditorGUILayout.PropertyField(phonemeList);

        EditorGUILayout.Space();

        if (GUILayout.Button("Analyze Audio And Generate Animation"))
        {
            LipSyncer ls = target as LipSyncer;
            ls.RhubarbAnalysis();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Animation From Text"))
        {
            LipSyncer ls = target as LipSyncer;
            ls.GenerateAnimation();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void ArrayGUI(SerializedObject obj, string name, int arrayNum)
    {
        arrayExpanded[arrayNum] = EditorGUILayout.Foldout(arrayExpanded[arrayNum], FormattedString(name));

        if (arrayExpanded[arrayNum])
        {
            int size = obj.FindProperty(name + ".Array.size").intValue;

            EditorGUI.indentLevel = 3;

            int newSize = EditorGUILayout.IntField("Size", size);

            if (newSize != size)
                obj.FindProperty(name + ".Array.size").intValue = newSize;

            for (int i = 0; i < newSize; i++)
            {
                var prop = obj.FindProperty(string.Format("{0}.Array.data[{1}]", name, i));
                EditorGUILayout.PropertyField(prop);
            }
        }

        EditorGUI.indentLevel = 0;
    }

    string FormattedString(string s)
    {
        string resultString = "";
        char[] c = s.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (i == 0)
            {
                if (c[i] >= 61)
                    resultString += (char)(c[i] - 32);
            }
            else
            {
                if (c[i] >= 41 && c[i] <= 90)
                {
                    resultString += ' ';
                }
                resultString += c[i];
            }
        }
        return resultString;
    }
}
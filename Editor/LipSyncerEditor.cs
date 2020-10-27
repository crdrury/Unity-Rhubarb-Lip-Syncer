using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LipSyncer))]
public class LipSyncerEditor : Editor
{
    SerializedProperty parentObject, mouthObjects, animateBlinks, blinkObjects, animateEyebrows, eyebrowObjects, sourceAudio, sourceAudioScript, phenomeList, animationName, blinkMin, blinkMax, blinkLength;
    bool[] arrayExpanded = new bool[3];

    private void OnEnable()
    {

        parentObject = serializedObject.FindProperty("parentObject");
        mouthObjects = serializedObject.FindProperty("mouthObjects");
        animateBlinks = serializedObject.FindProperty("animateBlinks");
        blinkObjects = serializedObject.FindProperty("blinkObjects");
        animateEyebrows = serializedObject.FindProperty("animateEyebrows");
        eyebrowObjects = serializedObject.FindProperty("eyebrowObjects");
        sourceAudio = serializedObject.FindProperty("sourceAudio");
        sourceAudioScript = serializedObject.FindProperty("sourceAudioScript");
        phenomeList = serializedObject.FindProperty("phenomeList");
        animationName = serializedObject.FindProperty("animationName");
        blinkMin = serializedObject.FindProperty("blinkMin");
        blinkMax = serializedObject.FindProperty("blinkMax");
        blinkLength = serializedObject.FindProperty("blinkLength");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(parentObject);
        ArrayGUI(serializedObject, "mouthObjects", 0);
        EditorGUILayout.PropertyField(sourceAudio);
        EditorGUILayout.PropertyField(sourceAudioScript);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(animateBlinks);
        EditorGUI.EndChangeCheck();
        EditorGUI.BeginDisabledGroup(!animateBlinks.boolValue);
        ArrayGUI(serializedObject, "blinkObjects", 1);
        EditorGUILayout.PropertyField(blinkMin);
        EditorGUILayout.PropertyField(blinkMax);
        EditorGUILayout.PropertyField(blinkLength);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(animateEyebrows);
        EditorGUI.EndChangeCheck();
        EditorGUI.BeginDisabledGroup(!animateEyebrows.boolValue);
        ArrayGUI(serializedObject, "eyebrowObjects", 2);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(animationName);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(phenomeList);
        EditorGUI.EndChangeCheck();

        EditorGUILayout.Space();

        if (GUILayout.Button("Analyze Audio And Generate Animation"))
        {
            LipSyncer ls = target as LipSyncer;
            ls.RhubarbAnalysis();
        }

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(phenomeList.objectReferenceValue == null);
        if (GUILayout.Button("Generate Animation From Text"))
        {
            LipSyncer ls = target as LipSyncer;
            ls.GenerateAnimation();
        }
        EditorGUI.EndDisabledGroup();

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
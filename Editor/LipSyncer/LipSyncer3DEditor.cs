using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LipSyncer3D))]
public class LipSyncer3DEditor : LipSyncerEditor
{
    SerializedProperty mouthObjects, mouthIntensity, blinkObjects, blinkIntensity, eyebrowObjects, eyebrowIntensityMin, eyebrowIntensityMax, animateBlinks, blinkMin, blinkMax, blinkLength, animateEyebrows, AVSync;

    protected override void OnEnable()
    {
        base.OnEnable();
        mouthObjects = serializedObject.FindProperty("mouthObjects");
        mouthIntensity = serializedObject.FindProperty("mouthIntensity");
        blinkObjects = serializedObject.FindProperty("blinkObjects");
        blinkIntensity = serializedObject.FindProperty("blinkIntensity");
        eyebrowObjects = serializedObject.FindProperty("eyebrowObjects");
        eyebrowIntensityMin = serializedObject.FindProperty("eyebrowIntensityMin");
        eyebrowIntensityMax = serializedObject.FindProperty("eyebrowIntensityMax");
        animateBlinks = serializedObject.FindProperty("animateBlinks");
        blinkMin = serializedObject.FindProperty("blinkMin");
        blinkMax = serializedObject.FindProperty("blinkMax");
        blinkLength = serializedObject.FindProperty("blinkLength");
        animateEyebrows = serializedObject.FindProperty("animateEyebrows");
        AVSync = serializedObject.FindProperty("AVSync");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ArrayGUI(serializedObject, "mouthObjects", 0);
        EditorGUILayout.PropertyField(mouthIntensity);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(animateBlinks);
        GUI.enabled = animateBlinks.boolValue;
        ArrayGUI(serializedObject, "blinkObjects", 1);
        EditorGUILayout.PropertyField(blinkMin);
        EditorGUILayout.PropertyField(blinkMax);
        EditorGUILayout.PropertyField(blinkLength);
        EditorGUILayout.PropertyField(blinkIntensity);
        EditorGUILayout.Space();
        GUI.enabled = true;

        EditorGUILayout.PropertyField(animateEyebrows);
        GUI.enabled = animateEyebrows.boolValue;
        ArrayGUI(serializedObject, "eyebrowObjects", 2);
        EditorGUILayout.PropertyField(eyebrowIntensityMin);
        EditorGUILayout.PropertyField(eyebrowIntensityMax);
        GUI.enabled = true;

//        EditorGUILayout.Space();
//        EditorGUILayout.PropertyField(AVSync);

        LowerGUI();
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
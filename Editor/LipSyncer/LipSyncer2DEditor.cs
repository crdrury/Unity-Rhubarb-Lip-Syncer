using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LipSyncer2D))]
public class LipSyncer2DEditor : LipSyncerEditor
{
    SerializedProperty mouthRenderer;

    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();
        mouthRenderer = serializedObject.FindProperty("mouthRenderer");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(mouthRenderer);

        LowerGUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

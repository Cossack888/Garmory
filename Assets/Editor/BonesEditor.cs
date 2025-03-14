using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bones))]
public class BonesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Bones script = (Bones)target;

        if (GUILayout.Button("Apply Body Parts"))
        {
            script.AttachBodyParts();
            EditorUtility.SetDirty(script.gameObject);
        }
    }
}

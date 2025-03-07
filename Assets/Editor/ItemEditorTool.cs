using UnityEditor;
using UnityEngine;

public class ItemEditorTool : EditorWindow
{
    private GameObject selectedObject;
    private Item selectedItem;

    [MenuItem("Tools/Item Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemEditorTool>("Item Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Editor Tool", EditorStyles.boldLabel);

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Source GameObject", selectedObject, typeof(GameObject), true);
        selectedItem = (Item)EditorGUILayout.ObjectField("Target Item", selectedItem, typeof(Item), false);

        if (selectedObject == null || selectedItem == null)
        {
            EditorGUILayout.HelpBox("Please select both a GameObject and an Item asset.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Copy Values from GameObject"))
        {
            CopyValues();
        }
    }

    private void CopyValues()
    {
        if (selectedObject == null || selectedItem == null)
        {
            Debug.LogWarning("Both a GameObject and an Item asset must be selected.");
            return;
        }
        selectedItem.position = selectedObject.transform.localPosition;
        selectedItem.rotation = selectedObject.transform.localEulerAngles;
        selectedItem.scale = selectedObject.transform.localScale;
        EditorUtility.SetDirty(selectedItem);
        AssetDatabase.SaveAssets();

        Debug.Log("Item values updated from GameObject: " + selectedObject.name);
    }
}

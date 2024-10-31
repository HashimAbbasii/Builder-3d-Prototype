using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ModelsSO", menuName = "ScriptableObjects/ModelsSO", order = 1)]
public class ModelsSO : ScriptableObject
{
    public List<ObjectSO> models;

#if UNITY_EDITOR
    [ContextMenu("Fill Models")]
    public void FillModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:ObjectSO");
        models = new List<ObjectSO>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ObjectSO obj = AssetDatabase.LoadAssetAtPath<ObjectSO>(path);

            if (obj != null)
            {
                models.Add(obj);
                Debug.Log("Found ScriptableObject: " + obj.name + " at path: " + path);
            }
        }

        Debug.Log("Total ScriptableObjects found: " + models.Count);
    }
#endif
}

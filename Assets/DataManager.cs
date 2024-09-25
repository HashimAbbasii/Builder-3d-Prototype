using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SaveData saveData;
    
    [ContextMenu("Make Object List")]
    public void MakeObjectList()
    {
        var sm = ManagerHandler.Instance.spawningManager;

        foreach (var fs in sm.floorsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectName = fs.name.Contains("(Clone)") ? fs.name.Split('(')[0].Trim() : fs.name,
                objectID = 0,
                objectPosition = fs.transform.position,
                objectRotation = fs.transform.rotation,
                objectScale = fs.transform.localScale
            };
            saveData.objectInfos.Add(oi);
        }

        foreach (var ws in sm.wallsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectName = ws.name.Contains("(Clone)") ? ws.name.Split('(')[0].Trim() : ws.name,
                objectID = 1,
                objectPosition = ws.transform.position,
                objectRotation = ws.transform.rotation,
                objectScale = ws.transform.localScale
            };
            saveData.objectInfos.Add(oi);
        }

        foreach (var ms in sm.modelsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectName = ms.name.Contains("(Clone)") ? ms.name.Split('(')[0].Trim() : ms.name,
                objectID = ms.GetComponent<ObjectType>().objectID,
                objectPosition = ms.transform.position,
                objectRotation = ms.transform.rotation,
                objectScale = ms.transform.localScale
            };
            saveData.objectInfos.Add(oi);
        }
    }
}

[Serializable]
public class SaveData
{
    public List<ObjectInfo> objectInfos = new();
}

[Serializable]
public class ObjectInfo
{
    public string objectName;
    public int objectID;
    public Vector3 objectPosition;
    public Quaternion objectRotation;
    public Vector3 objectScale;
}
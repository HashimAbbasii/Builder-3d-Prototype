using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SaveData saveData;

    public string json;
    
    [ContextMenu("Make Object List")]
    public void MakeObjectList()
    {
        saveData.objectInfos.Clear();
        var sm = ManagerHandler.Instance.spawningManager;

        foreach (var fs in sm.floorsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectItem = fs,
                objectName = fs.name.Contains("(Clone)") ? fs.name.Split('(')[0].Trim() : fs.name,
                objectID = 0,
                objectPosition = new JsonSavableVector3(fs.transform.position),
                objectRotation = new JsonSavableVector3(fs.transform.rotation.eulerAngles),
                objectScale = new JsonSavableVector3(fs.transform.localScale)
            };
            saveData.objectInfos.Add(oi);
        }

        foreach (var ws in sm.wallsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectItem = ws,
                objectName = ws.name.Contains("(Clone)") ? ws.name.Split('(')[0].Trim() : ws.name,
                objectID = 1,
                objectPosition = new JsonSavableVector3(ws.transform.position),
                objectRotation = new JsonSavableVector3(ws.transform.rotation.eulerAngles),
                objectScale = new JsonSavableVector3(ws.transform.localScale)
            };
            saveData.objectInfos.Add(oi);
        }

        foreach (var ms in sm.modelsSpawned)
        {
            var oi = new ObjectInfo
            {
                objectItem = ms,
                objectName = ms.name.Contains("(Clone)") ? ms.name.Split('(')[0].Trim() : ms.name,
                objectID = ms.GetComponent<ObjectType>().objectID,
                objectPosition = new JsonSavableVector3(ms.transform.position),
                objectRotation = new JsonSavableVector3(ms.transform.rotation.eulerAngles),
                objectScale = new JsonSavableVector3(ms.transform.localScale)
            };
            saveData.objectInfos.Add(oi);
        }

        foreach (var oi in saveData.objectInfos)
        {
            if (oi.objectItem.transform.parent != null)
            {
                oi.childOf = saveData.objectInfos.FindIndex(x => x.objectItem == oi.objectItem.transform.parent.gameObject);
            }
        }

        json = JsonConvert.SerializeObject(saveData);
        
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(path, json);
    }
    
    [ContextMenu("Load Object List")]
    public void LoadObjectList()
    {
        saveData = JsonConvert.DeserializeObject<SaveData>(json);

        foreach (var oi in saveData.objectInfos)
        {
            if (oi.objectID == 0)
            {
                var floor = Instantiate(ManagerHandler.Instance.spawningManager.floorPrefab);
                oi.objectItem = floor;
                ManagerHandler.Instance.spawningManager.floorsSpawned.Add(floor);
                //floor.transform.position = oi.objectPosition.ToVector3();
                //floor.transform.rotation = Quaternion.Euler(oi.objectRotation.ToVector3());
                //if (oi.childOf == -1) floor.transform.localScale = oi.objectScale.GetVector3();
            }
            else if (oi.objectID == 1)
            {
                var wall = Instantiate(ManagerHandler.Instance.spawningManager.wallPrefab);
                oi.objectItem = wall;
                ManagerHandler.Instance.spawningManager.wallsSpawned.Add(wall);
                //wall.transform.position = oi.objectPosition.ToVector3();
                //wall.transform.rotation = Quaternion.Euler(oi.objectRotation.ToVector3());
                //if (oi.childOf == -1) wall.transform.localScale = oi.objectScale.GetVector3();
            }
            else
            {
                var model = Instantiate(ManagerHandler.Instance.spawningManager.modelPrefabs[oi.objectID - 2]);
                oi.objectItem = model;
                ManagerHandler.Instance.spawningManager.modelsSpawned.Add(model);
                //model.transform.position = oi.objectPosition.ToVector3();
                //model.transform.rotation = Quaternion.Euler(oi.objectRotation.ToVector3());
                //if (oi.childOf == -1) model.transform.localScale = oi.objectScale.GetVector3();
            }
        }

        foreach (var childObj in saveData.objectInfos.Where(t => t.childOf != -1))
        {
            //...........Check if Child Index not Equal to -1 than it place a Parent ..................//

            childObj.objectItem.transform.SetParent(saveData.objectInfos[childObj.childOf].objectItem.transform);
        }

        foreach (var toTransform in saveData.objectInfos)
        {
            toTransform.objectItem.transform.localScale = toTransform.objectScale.ToVector3();
            toTransform.objectItem.transform.position = toTransform.objectPosition.ToVector3();
            toTransform.objectItem.transform.rotation = Quaternion.Euler(toTransform.objectRotation.ToVector3());
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
    [JsonIgnore] public GameObject objectItem;
    public string objectName;
    public int objectID;
    public int childOf = -1;
    public JsonSavableVector3 objectPosition = new();
    public JsonSavableVector3 objectRotation;
    public JsonSavableVector3 objectScale = new();
}

[Serializable]
public class JsonSavableVector3
{
    public float x;
    public float y;
    public float z;
    
    public JsonSavableVector3()
    {
        x = 0;
        y = 0;
        z = 0;
    }
    
    public JsonSavableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public JsonSavableVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
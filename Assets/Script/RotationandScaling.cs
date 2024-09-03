using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationandScaling : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject parentObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleRotation()
    {
        parentObject = GetTopParent(gameObject);
        Debug.Log("Object Name" + parentObject.name);
        parentObject.transform.Rotate(Vector3.up, 90f);
    }

    private GameObject GetTopParent(GameObject obj)
    {
        // While the object has a parent, move up the hierarchy
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
        }

        return obj;
    }

}

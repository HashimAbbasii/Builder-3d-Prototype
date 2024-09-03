using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHander : MonoBehaviour
{
    public GameObject Floor;
    public bool floorInstantiate = false;
    [SerializeField]
    public GameObject instantiatedFloor; // To keep track of the instantiated floor
   
    // Start is called before the first frame update
    void Start()
    {
        floorInstantiate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && floorInstantiate)
        {
            FloorDataInstantiate();
        }
    }

    public void FloorAdjustment()
    {
        floorInstantiate = true;
    }

    public void FloorDataInstantiate()
    {
        if (instantiatedFloor == null) // Check if the floor has already been instantiated
        {
            Vector3 floorPos = Floor.transform.position;
            instantiatedFloor = Instantiate(Floor, floorPos, Quaternion.identity); // Store the reference to the instantiated floor

            floorInstantiate = false; // Disable further instantiation
        }
    }
}

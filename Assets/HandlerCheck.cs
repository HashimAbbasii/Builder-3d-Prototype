using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerCheck : MonoBehaviour
{
    public VariableJoystick joystick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log(joystick.Vertical);
        }
    }
}

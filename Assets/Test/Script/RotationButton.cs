using UnityEngine;
using UnityEngine.UI;

public class RotationButton : MonoBehaviour
{
    public Button button;  // Assign the UI button in the Inspector
    public float rotationAngle; // The angle by which to rotate

    private ObjectManipulator manipulator;

    void Start()
    {
        manipulator = FindObjectOfType<ObjectManipulator>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (manipulator != null)
        {
            manipulator.RotateObject(rotationAngle);
        }
    }
}
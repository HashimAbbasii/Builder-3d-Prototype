using System.Collections;
using UnityEngine;

public class FocusCameraSelection : MonoBehaviour
{
    public Transform cameraTransform; // The Camera's transform will be assigned at runtime
    public Vector3 focusPosition = new Vector3(1000, 1000, 1000); // Camera's temporary position
    public Vector3 originalCameraPosition; // To store the camera's initial position
    private Quaternion originalCameraRotation; // To store the camera's initial rotation
    private bool isFocused = false; // To track focus state
    public ReferenceContain ReferenceContain;
    public Vector3 startPositionForCamera;
    public Quaternion startPositionForCameraRotation;

    private void Awake()
    {
         
}
    void Start()
    {
        ReferenceContain = FindObjectOfType<ReferenceContain>();
        startPositionForCamera = ReferenceContain.cameraReference.position;
        // Find the main camera in the scene at runtime
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            ReferenceContain.cameraReference = mainCamera.transform;
            // Save the original position and rotation of the camera
            originalCameraPosition = ReferenceContain.cameraReference.position; // Save this in Start()
            originalCameraRotation = ReferenceContain.cameraReference.rotation;
        }
        else
        {
            Debug.LogError("No Main Camera found in the scene!");
            return;
        }
    }

    public void FocusCamera()
    {
        ReferenceContain = FindObjectOfType<ReferenceContain>();
        if (!isFocused)
        {
            Debug.Log("Focusing Camera");

            // Save the current camera position and rotation
            originalCameraPosition = ReferenceContain.cameraReference.position;
            originalCameraRotation = ReferenceContain.cameraReference.rotation;

            // Move the camera to the focus position
            ReferenceContain.cameraReference.position = focusPosition;

            // Position the object in front of the camera
            Vector3 objectPosition = ReferenceContain.cameraReference.position + ReferenceContain.cameraReference.forward * 5f; // Adjust the distance as needed
            transform.position = objectPosition;

            // Rotate the camera to look at the object
            ReferenceContain.cameraReference.LookAt(transform.position);
            isFocused = true;
        }
    }

    public void ResetCameraAfterTextureSelection()
    {
        if (isFocused)
        {
            Debug.Log("Resetting Camera");
            StartCoroutine(SmoothTransitionBack());
        }
    }

    private IEnumerator SmoothTransitionBack()
    {
        float duration = 1.5f; // Duration of the transition
        float elapsedTime = 0f;

        Vector3 startPosition = startPositionForCamera;
        Quaternion startRotation = startPositionForCameraRotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate position and rotation
            ReferenceContain.cameraReference.position = Vector3.Lerp(startPosition, originalCameraPosition, t);
            ReferenceContain.cameraReference.rotation = Quaternion.Slerp(startRotation, originalCameraRotation, t);

            Debug.Log($"Camera Position: {ReferenceContain.cameraReference.position}"); // Debug camera position
            Debug.Log($"Camera Rotation: {ReferenceContain.cameraReference.rotation}"); // Debug camera rotation

            yield return null;
        }

        ReferenceContain.cameraReference.position = originalCameraPosition;
        ReferenceContain.cameraReference.rotation = originalCameraRotation;
        isFocused = false;
        Debug.Log("Camera reset complete");
    }

}

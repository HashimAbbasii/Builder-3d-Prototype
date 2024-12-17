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
        //startPositionForCamera = ReferenceContain.cameraReference.position;
        // Find the main camera in the scene at runtime
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            ReferenceContain.cameraReference = mainCamera.transform;
            // Save the original position and rotation of the camera
            originalCameraPosition = ReferenceContain.parentTransform.position; // Save this in Start()
            originalCameraRotation = ReferenceContain.parentTransform.rotation;
           
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
          

            // Save the current camera position and rotation
            originalCameraPosition = ReferenceContain.parentTransform.position;
            originalCameraRotation = ReferenceContain.parentTransform.rotation;

            // Move the camera to the focus position
            ReferenceContain.parentTransform.position = focusPosition;

            // Position the object in front of the camera
            Vector3 objectPosition = ReferenceContain.parentTransform.position + ReferenceContain.parentTransform.forward * 5f; // Adjust the distance as needed
            transform.position = objectPosition;

            // Rotate the camera to look at the object
            ReferenceContain.cameraReference.LookAt(transform.position);
            isFocused = true;
        }
    }

    public void ResetCameraPosition()
    {
        if (isFocused)
        {

            SpawningManager spawningManager = FindObjectOfType<SpawningManager>();
           
            spawningManager._previewObject.transform.position = spawningManager.objectOriginalPos;
          Debug.Log("hmmm");
             SpawningManager spawnManager = FindObjectOfType<SpawningManager>();
           originalCameraPosition = spawnManager.cameraTransform;
            Debug.Log("CAMERA RESET"+originalCameraPosition);
            originalCameraRotation = spawnManager.cameraRotation;
           
           
            StartCoroutine(SmoothTransitionBack());
            
        }
    }

    IEnumerator SmoothTransitionBack()
    {
        float duration = 0.5f; // Duration of the transition
        float elapsedTime = 0f;
        Vector3 startPos = ReferenceContain.parentTransform.position;
        Quaternion startRotation = ReferenceContain.parentTransform.rotation;
        Debug.Log("startPos" + startPos);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            //         Interpolate position and rotation
            ReferenceContain.parentTransform.position = Vector3.Lerp(startPos, originalCameraPosition, t);
            ReferenceContain.parentTransform.rotation = Quaternion.Slerp(startRotation, originalCameraRotation, t);

            yield return null;
        }


        ReferenceContain.parentTransform.position = originalCameraPosition;
        ReferenceContain.parentTransform.rotation = originalCameraRotation;
        Transform firstChild = ReferenceContain.parentTransform.GetChild(0);
        Debug.Log(firstChild.name);
        firstChild.localRotation = Quaternion.Euler(0, 0, 0);
        ReferenceContain referenceContain = FindObjectOfType<ReferenceContain>();
        referenceContain.spawnModel.Clear();
        SelectableObject selectableObject = FindObjectOfType<SelectableObject>();
        selectableObject.ClearChildren(selectableObject.modelVariantScrollContentParent.transform);

        isFocused = false;
        //    Debug.Log("Camera reset complete");
        ObjectManipulator objectManipulator = FindObjectOfType<ObjectManipulator>();
        objectManipulator.DeselectObject();
        
    }

    public void ResetCameraAfterTextureSelection()
    {

        
        //if (isFocused)
        //{
            originalCameraPosition = startPositionForCamera;
        //    originalCameraRotation = startPositionForCameraRotation;
        //    Debug.Log("Resetting Camera");
           // StartCoroutine(SmoothTransitionBack());
       // }
    }

    //private IEnumerator SmoothTransitionBack()
    //{
    //    Debug.Log("originalCameraPosition"+originalCameraPosition);
    //    float duration = 0.5f; // Duration of the transition
    //    float elapsedTime = 0f;

    //    Vector3 startPosition = ReferenceContain.cameraReference.position;
    //    Quaternion startRotation = ReferenceContain.cameraReference.rotation;

    //    while (elapsedTime < duration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float t = elapsedTime / duration;

    //        // Interpolate position and rotation
    //        ReferenceContain.cameraReference.position = Vector3.Lerp(startPosition, originalCameraPosition, t);
    //        ReferenceContain.cameraReference.rotation = Quaternion.Slerp(startRotation, originalCameraRotation, t);

    //        Debug.Log($"Camera Position: {ReferenceContain.cameraReference.position}"); // Debug camera position
    //        Debug.Log($"Camera Rotation: {ReferenceContain.cameraReference.rotation}"); // Debug camera rotation

    //        yield return null;
    //    }

    //    ReferenceContain.cameraReference.position = originalCameraPosition;
    //    ReferenceContain.cameraReference.rotation = originalCameraRotation;
    //    isFocused = false;
    //    Debug.Log("Camera reset complete");
    //}

}

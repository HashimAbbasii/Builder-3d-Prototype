using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionHandler : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private SpawningManager spawningManager;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0, 5);  // Adjust to set distance from camera
    [SerializeField] private List<Sprite> textureSprites;  // List of available textures
    [SerializeField] private Button textureSelectionButton;  // The button to trigger texture change

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTextureSelected = false;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Setup button listener
        textureSelectionButton.onClick.AddListener(OnTextureSelection);
    }

    public void OnObjectSelected()
    {
        // Set camera to a distant position
        cameraManager.MoveCameraToPosition(new Vector3(1000, 1000, 1000));

        // Spawn object in front of camera
        SpawnObjectInFront();
    }

    private void SpawnObjectInFront()
    {
        if (objectToSpawn != null)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(1000, 1000, 1000) + spawnOffset, Quaternion.identity);
            spawnedObject.SetActive(true);  // Ensure the object is visible
        }
    }

    private void OnTextureSelection()
    {
        if (!isTextureSelected && textureSprites.Count > 0)
        {
            // Change texture (this assumes you have a method for changing texture)
            ChangeObjectTexture(textureSprites[0]);  // Example: picking the first texture in the list

            // Set texture as selected and return object to its original position
            isTextureSelected = true;
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            // Move the camera back to the object
           // cameraManager.FocusOnObject(transform);
        }
    }

    private void ChangeObjectTexture(Sprite newTexture)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && newTexture != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = newTexture.texture;
            renderer.material = material;
        }
    }
}

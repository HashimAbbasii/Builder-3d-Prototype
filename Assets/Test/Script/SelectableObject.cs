using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableObject : ObjectType
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private SpawningManager spawningManager;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0, 5); // Distance from camera
    [SerializeField] private List<Button> textureSelectionButtons; // Buttons for texture selection
    [SerializeField] private List<Sprite> textureSprites; // Textures for objects

    public int ObjectID; // Unique identifier for each object
    public ModelType modelType;
    public FurnitureType furnitureType;
    public EvidenceType evidenceType;
    public bool canPlaceObjectsOnIt;
    public bool canBePlacedOnObject;
    public float heightOffset = 0.0f; // Height offset for placing objects
    public Renderer objectRenderer;
    public List<Texture> modelTextures;

    //public List<Texture> ModelMaterials;


    public Vector3 OriginalScale { get; private set; }
    public GameObject modelVariantScrollContentParent;

    [Header("Model Variants")]
    public List<Material> modelMaterials;

    [Header("Image Texture")]
    public List<Sprite> modelImages;

    public ObjectManipulator manipulator;
    public Transform selectChildForSelection;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTextureSelected = false;
    public Transform parentReference;
    public Transform parentAcheivedReference;
    //[Header("Focus Camera")]
   // public FocusCameraSelection focusCameraSelection;



    private void Awake()
    {
        // Initialize variables
        cameraManager = FindObjectOfType<CameraManager>();
        spawningManager = FindObjectOfType<SpawningManager>();
        manipulator = FindObjectOfType<ObjectManipulator>();
        modelVariantScrollContentParent = spawningManager.ModelVariant;
        OriginalScale = transform.localScale;

        // Store original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Assign a unique ObjectID
        AssignUniqueID();
        //focusCameraSelection=FindObjectOfType<FocusCameraSelection>();

        // Setup texture selection buttons
        //if (textureSelectionButtons != null)
        //{
        //    for (int i = 0; i < textureSelectionButtons.Count; i++)
        //    {
        //        int index = i; // Prevent closure issue
        //        textureSelectionButtons[i].onClick.AddListener(() => OnTextureSelection(index));
        //    }
        //}

      //  MakeChildofScroll();
    }

    private void AssignUniqueID()
    {
        ObjectID = GetInstanceID(); // Using Unity's instance ID as a unique identifier
        Debug.Log($"ObjectID assigned: {ObjectID}");
    }

    public void OnObjectSelected()
    {
        // Move camera to distant position
        cameraManager.MoveCameraToPosition(new Vector3(1000, 1000, 1000));

        // Spawn object in front of camera
        SpawnObjectInFront();
    }

    private void SpawnObjectInFront()
    {
        if (objectToSpawn != null)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(1000, 1000, 1000) + spawnOffset, Quaternion.identity);
            spawnedObject.SetActive(true); // Ensure visibility
        }
    }

    private void OnTextureSelection(int textureIndex)
    {
        if (!isTextureSelected && textureSprites.Count > 0 && textureIndex < textureSprites.Count)
        {
            // Change texture of the object
            ChangeObjectTexture(textureSprites[textureIndex]);

            // Reset object position and rotation
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            // Move camera to focus on object
            //cameraManager.FocusOnObject(transform);

            isTextureSelected = true; // Mark texture as selected
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

    public void MakeChildofScroll()
    {
        Debug.Log("ssdsafa");
        if (modelVariantScrollContentParent == null || modelMaterials.Count == 0)
        {
            Debug.LogWarning("Model variant or materials are not set.");
            return;
        }

        for (int i = modelVariantScrollContentParent.transform.childCount - 1; i >= 0; i--)
        {
            Debug.Log("sdsdsdsd");
            Destroy(modelVariantScrollContentParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < modelMaterials.Count; i++)
        {
            if (modelMaterials[i] != null)
            {
                
                GameObject materialObject = new GameObject($"Material_{i}");
               // parentReference = materialObject;
               // parentAcheivedReference=materialObject.transform;
               // parentReference = parentAcheivedReference.parent;
                // transform.SetParent(parentReference);
                materialObject.transform.SetParent(modelVariantScrollContentParent.transform, false);

                RectTransform rectTransform = materialObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100);
                rectTransform.anchoredPosition = new Vector2(0, -i * 120);

                Image image = materialObject.AddComponent<Image>();
                image.sprite = modelImages[i];

                Button button = materialObject.AddComponent<Button>();
                textureSelectionButtons.Add(button);
                int index = i;
                button.onClick.AddListener(() => spawningManager.TextureImplementation(index));
            }
            else
            {
                Debug.LogWarning($"Material at index {i} is null. Skipping.");
            }
        }
      
        selectChildForSelection = transform.GetChild(0);
       // manipulator.SetSelectedObject(selectChildForSelection);
        // cameraManager.FocusOnObject(selectChildForSelection);
        FocusCameraSelection focusCameraSelection = FindObjectOfType<FocusCameraSelection>();
        focusCameraSelection.FocusCamera();
       
        
    }

   public void ClearChildren(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("Parent is null! Cannot clear children.");
            return;
        }

        // Loop through all children and destroy them
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject); // Destroy the child GameObject
        }

        Debug.Log("All children of " + parent.name + " have been deleted.");
    }

}

public enum ModelType
{
    Furniture,
    Evidence
}

public enum FurnitureType
{
    None,
    Chair,
    Table,
    Bed,
    Carpet
}

public enum EvidenceType
{
    None,
    Blood,
    DeadBody,
    Knife,
    Pen,
    Line
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class SelectableObject : ObjectType
{
    public ModelType modelType;
    public FurnitureType furnitureType;
    public EvidenceType evidenceType;
    public bool canPlaceObjectsOnIt;
    public bool canBePlacedOnObject;
    public float heightOffset = 0.0f; // The height offset to place objects on this surface
    public Renderer objectRenderer;
    public List<GameObject> ModelVariants;
    public Vector3 OriginalScale { get; private set; }
    public GameObject modelVariantScrollContentParent;

    [Header("Model Variants")]
    public List<Material> modelMaterials;

    public SpawningManager spawningManager;


    [Header("Image Texture")]
    public List<Sprite> modelImages;

    private void Start()
    {
        //modelVariant = 
        Debug.Log("When its Run");
        spawningManager=FindObjectOfType<SpawningManager>();
        modelVariantScrollContentParent = spawningManager.ModelVariant;
        OriginalScale = transform.localScale;
        MakeChildofScroll();


    }
    public void MakeChildofScroll()
    {


        if (modelVariantScrollContentParent == null || modelMaterials.Count == 0)
        {
            Debug.LogWarning("Model variant or materials are not set.");
            return;
        }

        for (int i = modelVariantScrollContentParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(modelVariantScrollContentParent.transform.GetChild(i).gameObject);
        }

        // Check if a parent Canvas exists; create one if needed
        Canvas parentCanvas = modelVariantScrollContentParent.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            // If no Canvas exists, create one
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            parentCanvas = canvasObject.GetComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Set the Canvas as the parent of the modelVariant
            modelVariantScrollContentParent.transform.SetParent(canvasObject.transform, false);
        }

        // Loop through the materials
        for (int i = 0; i < modelMaterials.Count; i++)
        {
            if (modelMaterials[i] != null)
            {
                Debug.Log($"Material {i}: {modelMaterials[i]}");

                // Create a new GameObject to represent the material
                GameObject materialObject = new GameObject($"Material_{i}");

                // Set the material object as a child of the model variant
                materialObject.transform.SetParent(modelVariantScrollContentParent.transform, false);

                // Add a RectTransform (required for UI components)
                RectTransform rectTransform = materialObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100); // Set the size of the UI element
                rectTransform.anchoredPosition = new Vector2(0, -i * 120); // Stack vertically

                // Add an Image component for UI representation
                Image image = materialObject.AddComponent<Image>();

                // Assign a sprite to the Image component
                image.sprite=modelImages[i];




                Sprite exampleSprite = Resources.Load<Sprite>($"Sprites/Material_{i}"); // Adjust path as needed

                Button button = materialObject.AddComponent<Button>();

                // Add OnClick event listener
                int index = i; // Local variable to avoid closure issue

                // Optional: Customize the button
                ColorBlock buttonColors = button.colors;
                buttonColors.normalColor = Color.white;
                buttonColors.highlightedColor = Color.gray;
                buttonColors.pressedColor = Color.green;
                button.colors = buttonColors;
                button.onClick.AddListener(() => spawningManager.ChangeFloorTexture(index));


                if (exampleSprite != null)
                {
                    image.sprite = exampleSprite;
                }
                else
                {
                    Debug.LogWarning($"Sprite not found for Material_{i}. Ensure the sprite is in the correct path.");
                }
            }
            else
            {
                Debug.LogWarning($"Material at index {i} is null. Skipping.");
            }
        }
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
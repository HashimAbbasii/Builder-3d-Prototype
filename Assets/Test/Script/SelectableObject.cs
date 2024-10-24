using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    private void Start()
    {
        OriginalScale = transform.localScale;
       
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
    Knife
}
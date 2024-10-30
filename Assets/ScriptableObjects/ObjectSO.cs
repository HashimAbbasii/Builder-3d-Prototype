using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectSO", menuName = "ScriptableObjects/ObjectSO", order = 2)]
public class ObjectSO : ScriptableObject
{
    public GameObject gameModel;
    public SurfaceType surfaceType;
    public ModelType modelType;
    public FurnitureType furnitureType;
    public EvidenceType evidenceType;
    public float heightOffset = 0.0f; // The height offset to place objects on this surface

    public bool canPlaceObjectsOnIt;
    public bool canBePlacedOnObject;
}

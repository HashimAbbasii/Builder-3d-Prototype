using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : ObjectType
{
    public bool canPlaceObjectsOnIt;
    public float heightOffset = 0.0f; // The height offset to place objects on this surface

    public Vector3 OriginalScale { get; private set; }

    private void Start()
    {
        OriginalScale = transform.localScale;
    }
}

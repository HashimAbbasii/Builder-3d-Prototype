using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ManagerHandler : MonoBehaviour
{
    #region Singleton

    public static ManagerHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Fields (Set in Inspector)
    public ObjectManipulator objectManipulator;
    public SpawningManager spawningManager;
    public CalculateDistance calculateDistance;
    public StarterManagement starterManagement;

    #endregion
}

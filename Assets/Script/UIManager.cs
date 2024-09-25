using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasHandler canvasHandler;
    public ModelSpawnButton modelButtonPrefab;
    
    public GameObject startPanel;
    
    private void Start()
    {
        // startPanel.SetActive(true);  
    }
    
    public void HandleSceneManagement(int index)
    {
        SceneManager.LoadScene(index);
    }
}

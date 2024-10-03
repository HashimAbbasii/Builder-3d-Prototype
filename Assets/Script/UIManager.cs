using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public CanvasHandler canvasHandler;
    public ModelSpawnButton modelButtonPrefab;
    
    public GameObject startPanel;
    public GameObject GamePanelPause;
    
    private void Start()
    {
        // startPanel.SetActive(true);  
    }
    
    public void HandleSceneManagement(int index)
    {
        SceneManager.LoadScene(index);
    }


    public void Return()
    {
        GamePanelPause.gameObject.SetActive(false);
    }
    public void Restart(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void Exit()
    {
        // Quit the application
#if UNITY_EDITOR
        // If running in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running in a standalone build
            Application.Quit();
#endif
    }
}

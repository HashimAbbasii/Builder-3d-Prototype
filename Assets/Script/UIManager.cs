using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public GameObject startPanel;
    void Start()
    {
        startPanel.SetActive(true);  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandleSceneManagement(int index)
    {
        SceneManager.LoadScene(index);
    }
}

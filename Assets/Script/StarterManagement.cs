using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarterManagement : MonoBehaviour
{

    [SerializeField] private Transform plotSelection;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator StartingPanel;
    [SerializeField] private GameObject StopingPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlotSize(int index)
    {
        if (index == 0)
        {
            mainCamera.transform.position = new Vector3(0, 19.27f, -5f);
            mainCamera.transform.rotation = Quaternion.Euler(new Vector3(79.6f, 0f, 0f));
            mainCamera.transform.localScale = new Vector3(1.2f,1,1);

            plotSelection.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
            StartingPanel.enabled = true;
            StartCoroutine(Endpanel());
        }
        else if(index == 1)
        {
            mainCamera.transform.position = new Vector3(0, 19.27f, -5f);
            mainCamera.transform.rotation = Quaternion.Euler(new Vector3(79.6f, 0f, 0f));
            mainCamera.transform.localScale = new Vector3(1.2f, 1, 1);

            plotSelection.transform.localScale = new Vector3(2f, 2f, 2f);
            StartingPanel.enabled = true;
            StartCoroutine(Endpanel());
        }
        else
        {
            mainCamera.transform.position = new Vector3(0, 19.27f, -5f);
            mainCamera.transform.rotation = Quaternion.Euler(new Vector3(79.6f, 0f, 0f));
            mainCamera.transform.localScale = new Vector3(1.2f, 1, 1);
            //...........Plot Allignment.....................//
            plotSelection.transform.position = new Vector3(1.4f, 0f, -2.4f);
            plotSelection.transform.rotation = Quaternion.Euler(new Vector3(0, 0f, 0f));
            plotSelection.transform.localScale = new Vector3(1.9f, 1.5f, 1.1f);


           
            StartingPanel.enabled = true;
            StartCoroutine(Endpanel());
        }
    }

    IEnumerator Endpanel ()
    {

        //StartingPanel
        yield return new WaitForSecondsRealtime(1.5f);
        StopingPanel.gameObject.SetActive(false);

    }
}

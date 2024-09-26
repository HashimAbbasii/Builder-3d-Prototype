using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    #region Set on Inspector (Public Field)
    public GameObject arrowIndicate;
    public Button FloorButton;
    public Button WallButton;
    public Button ChairButton;
    public Button KnifeButton;

    #endregion

    #region 
    private Animator ArrowAnimator;


    #endregion 
    // Start is called before the first frame update
    void Start()
    {
        //ArrowAnimator=GetComponent<Animator>();
        //FloorButton.onClick.AddListener();
        //WallButton.onClick.AddListener();
        //ChairButton.onClick.AddListener();
        //KnifeButton.onClick.AddListener();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HandleArrowIndication()
    {
        yield return null;
        

    }
    public void HandleWallIndication()
    {

    }
    public void HandleWallFloorIndication()
    
    {

    }
    public void HandleChairIndication() 
    {

    }
    public void HandleKnifeIndication() 
    
    {

    }
}

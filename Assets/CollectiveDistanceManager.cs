using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectiveDistanceManager : MonoBehaviour
{
    public EssentialDistanceManager essentialDistanceManager;
    
    public ObjectDistanceHandler objectDistanceHandler;

    public ButtonWithTextTMP objectDistanceHandlerButton;

    public Color textUnselectedColor = new Color(1f, 1f, 1f);
    public Color textSelectedColor = new Color(0f, 0.533f, 0.494f);
    
    private Color _currentTextColor;

    private void Start()
    {
        _currentTextColor = textUnselectedColor;
        objectDistanceHandlerButton.buttonText.color = _currentTextColor;
    }

    public void ToggleObjectDistanceHandlerScript()
    {
        _currentTextColor = _currentTextColor == textUnselectedColor ? textSelectedColor : textUnselectedColor;
        objectDistanceHandlerButton.buttonText.color = _currentTextColor;
        objectDistanceHandler.ToggleScript();
    }
}

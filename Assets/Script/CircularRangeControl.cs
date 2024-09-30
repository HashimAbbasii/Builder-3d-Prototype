using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// A circular range / slider control for Unity UI
/// By Pietro Polsinelli https://twitter.com/ppolsinelli
/// </summary>
public class CircularRangeControl : MonoBehaviour
{
    public ObjectManipulator objectManipulator;
    public State circularButtonState = State.NOT_DRAGGING;
    public Transform origin; //center of rotation
    public Image imageSelected; //drag here the image of type filled radial top
    public TextMeshProUGUI angle; //value textual feedback
    public TextMeshProUGUI detectedValue; //value textual feedback

    public Toggle deltaChanges; // if on the value can only change by a delta

    public int scaleValue = 360; //value scale to use

    private int _currentValue;
    
    
    public enum State
    {
        NOT_DRAGGING,
        DRAGGING,
    }

    public void ResetCircular()
    {
        _currentValue = 0;
        angle.text = "" + (int)(_currentValue * scaleValue / 360f);
        imageSelected.fillAmount = _currentValue / 360f;
        objectManipulator.SetRotation(_currentValue);
        
    }
    
    public void DragOnCircularArea(bool isClick)
    {
        //we ignore the click event due to dragging in order to 
        //ignore beyond max set with drag and enable it on click / touch
        if (isClick && circularButtonState == State.DRAGGING)         {
            circularButtonState = State.NOT_DRAGGING;
            return;
        }

        if (isClick)
        {
            circularButtonState = State.NOT_DRAGGING;
        }
        else
        {
            circularButtonState = State.DRAGGING;
        }

        var f = Vector3.Angle(Vector3.up, Input.mousePosition - origin.position);
        var onTheRight = Input.mousePosition.x > origin.position.x;
        var detectedValue = onTheRight ? (int)f : 180 + (180 - (int)f);

        if (detectedValue > 350)
        {
            detectedValue = 360;
        }
        else if (_currentValue == 360 && detectedValue < 10)
        {
            detectedValue = 360;
        }
        else if (_currentValue == 0 && detectedValue > 350)
        {
            detectedValue = 0;
        }
        else if (detectedValue < 10)
        {
            detectedValue = 0;
        }

        if (!isClick && deltaChanges.isOn)
        {
            if (detectedValue <= _currentValue && Mathf.Abs(_currentValue - detectedValue) > 180)
            {
                detectedValue = _currentValue;
            }
            else if (_currentValue == 0 && detectedValue > 270)
            {
                detectedValue = _currentValue;
            }
        }

        _currentValue = detectedValue;
        angle.text = "" + (int)(_currentValue * scaleValue / 360f);
        imageSelected.fillAmount = _currentValue / 360f;
        objectManipulator.SetRotation(_currentValue);
    }


}
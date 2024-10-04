using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonWithTextTMP : Button
{
    public TMP_Text buttonText;
    
    protected override void Awake()
    {
        base.Awake();
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string newText)
    {
        if (buttonText != null)
        {
            buttonText.text = newText;
        }
    }
}


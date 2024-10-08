using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ButtonWithTextTMP), true)]
[CanEditMultipleObjects]
public class ButtonWithTextTMPEditor : ButtonEditor
{
    SerializedProperty m_TextProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_TextProperty = serializedObject.FindProperty("buttonText");
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_TextProperty, new GUIContent("Text"));
        serializedObject.ApplyModifiedProperties();
    }
    
    [MenuItem("GameObject/UI/Button (WWT)", false, 10)]
    public static void CreateButtonGameObject(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        
        // Create a new GameObject
        var buttonGO = new GameObject("Button (WTT)");
        var rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 30);
        
        var childText = new GameObject("Text (TMP)");
        childText.AddComponent<RectTransform>();
        childText.transform.SetParent(buttonGO.transform);

        var image = buttonGO.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        
        var button = buttonGO.AddComponent<ButtonWithTextTMP>();
        button.targetGraphic = image;
        
        var text = childText.AddComponent<TextMeshProUGUI>();
        text.text = "Button (WTT)";
        text.fontSize = 24;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;
        
        var textRectTransform = childText.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.sizeDelta = Vector2.zero;

        buttonGO.GetComponent<ButtonWithTextTMP>().buttonText = text;

        buttonGO.transform.parent = parent.transform;
        rectTransform.localScale = Vector3.one;
        // Add the GameObject to the current selection
        Selection.activeObject = buttonGO;
    }
}


#endif

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


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


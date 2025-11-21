using System;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] TextMeshProUGUI degreesOfSeparationText;
    [SerializeField] TextMeshProUGUI hoverableText;
    
    [Header("Data")]
    [SerializeField] NodeTextData nodeTextData;

    void Awake()
    {
        InitializeComponents();
    }

    void OnValidate()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (degreesOfSeparationText == null)
        {
            Transform degreesCanvas = transform.Find("DegreesCanvas");

            if (degreesCanvas != null)
            {
                Transform degreesTextTransform = degreesCanvas.Find("DegreesText");

                if (degreesTextTransform != null)
                {
                    degreesOfSeparationText = degreesTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (hoverableText == null)
        {
            Transform degreesCanvas = transform.Find("DegreesCanvas");

            if (degreesCanvas != null)
            {
                Transform hoverTextTransform = degreesCanvas.Find("HoverText");
                
                if (hoverTextTransform != null)
                {
                    hoverableText = hoverTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        UpdateHoverableText();
        SetHoverTextVisibility(false);
    }

    public void SetDegreesText(string text)
    {
        if (degreesOfSeparationText != null)
        {
            degreesOfSeparationText.text = text;
        }
    }

    public void SetDegreesTextVisibility(bool isVisible)
    {
        if (degreesOfSeparationText != null)
        {
            degreesOfSeparationText.enabled = isVisible;
        }
    }

    public void UpdateHoverableText()
    {
        if (hoverableText != null)
        {
            hoverableText.text = (nodeTextData != null) ? nodeTextData.nodeText : string.Empty;
        }
    }
    
    public void SetHoverTextVisibility(bool isVisible)
    {
        if (hoverableText != null) 
        {
            hoverableText.enabled = isVisible;
        }
    }

    public string GetHoverText()
    {
        return (nodeTextData != null) ? nodeTextData.nodeText : string.Empty;
    }

    public bool HasHoverText()
    {
        return nodeTextData != null && !string.IsNullOrEmpty(nodeTextData.nodeText);
    }
}

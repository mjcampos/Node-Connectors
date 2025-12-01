using System;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] TextMeshProUGUI degreesOfSeparationText;

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
}

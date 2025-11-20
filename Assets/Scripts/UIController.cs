using System;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI degreesOfSeparationText;

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
            degreesOfSeparationText = GetComponentInChildren<TextMeshProUGUI>();
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

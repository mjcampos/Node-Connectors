using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] TextMeshProUGUI degreesOfSeparationText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    void Awake()
    {
        InitializeComponents();
        HideHoverInfo();
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
                
                Transform titleTextTransform = degreesCanvas.Find("TitleText");

                if (titleTextTransform != null)
                {
                    titleText = titleTextTransform.GetComponent<TextMeshProUGUI>();
                }
                
                Transform descriptionTextTransform = degreesCanvas.Find("DescriptionText");

                if (descriptionTextTransform != null)
                {
                    descriptionText = descriptionTextTransform.GetComponent<TextMeshProUGUI>();
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

    public void ShowHoverInfo(string title, string description)
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.enabled = true;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
            descriptionText.enabled = true;
        }
    }

    public void HideHoverInfo()
    {
        if (titleText != null)
        {
            titleText.enabled = false;
        }

        if (descriptionText != null)
        {
            descriptionText.enabled = false;
        }
    }
}

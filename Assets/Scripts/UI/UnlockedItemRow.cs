using TMPro;
using UnityEngine;

public class UnlockedItemRow : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] int maxTextLength = 20;
    
    [Space(10)]
    
    [SerializeField] TextMeshProUGUI idText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    void OnValidate()
    {
        if (idText == null)
        {
            Transform idTransform = transform.Find("ID Text");
            if (idTransform != null)
                idText = idTransform.GetComponent<TextMeshProUGUI>();
        }

        if (titleText == null)
        {
            Transform titleTransform = transform.Find("Title Text");
            if (titleTransform != null)
                titleText = titleTransform.GetComponent<TextMeshProUGUI>();
        }

        if (descriptionText == null)
        {
            Transform descTransform = transform.Find("Description Text");
            if (descTransform != null)
                descriptionText = descTransform.GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetData(string id, string title, string description)
    {
        if (idText != null)
            idText.text = TruncateText(id, maxTextLength);

        if (titleText != null)
            titleText.text = TruncateText(title, maxTextLength);

        if (descriptionText != null)
            descriptionText.text = TruncateText(description, maxTextLength);
    }

    string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength) + "...";
    }
}

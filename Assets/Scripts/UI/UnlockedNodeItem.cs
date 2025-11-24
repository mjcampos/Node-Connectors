using UnityEngine;
using TMPro;

public class UnlockedNodeItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI nodeName;
    [SerializeField] TextMeshProUGUI nodeDescription;

    public void Initialize(string name, string description)
    {
        if (nodeName != null)
        {
            nodeName.text = name;
        }

        if (nodeDescription != null)
        {
            nodeDescription.text = description;
        }
    }

    void OnValidate()
    {
        if (nodeName == null)
        {
            Transform nameTransform = transform.Find("NodeName");
            if (nameTransform != null)
            {
                nodeName = nameTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        if (nodeDescription == null)
        {
            Transform descTransform = transform.Find("NodeDescription");
            if (descTransform != null)
            {
                nodeDescription = descTransform.GetComponent<TextMeshProUGUI>();
            }
        }
    }
}

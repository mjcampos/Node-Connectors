using UnityEngine;

[CreateAssetMenu(fileName = "HoverableTextData", menuName = "Scriptable Objects/HoverableTextData")]
public class NodeTextData : ScriptableObject
{
    [TextArea] public string nodeText;
}

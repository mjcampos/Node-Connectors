using Helpers;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeDataSO", menuName = "Scriptable Objects/NodeDataSO")]
public class NodeDataSO : ScriptableObject
{
    [Header("Node Information")]
    public string title;
    [TextArea(3, 10)]
    public string description;
    
    [Header("State Sprites")]
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
    public Sprite visibleSprite;
    public Sprite nonHoverableSprite;
    public Sprite hiddenSprite;
    
    void OnValidate()
    {
        if (unlockedSprite == null || lockedSprite == null || visibleSprite == null || 
            nonHoverableSprite == null || hiddenSprite == null)
        {
            Debug.LogWarning($"NodeDataSO '{name}' is missing one or more sprite assignments!", this);
        }
    }

    public Sprite GetSpriteForState(NodeState state)
    {
        return state switch
        {
            NodeState.Unlocked => unlockedSprite,
            NodeState.Locked => lockedSprite,
            NodeState.Visible => visibleSprite,
            NodeState.NonHoverable => nonHoverableSprite,
            NodeState.Hidden => hiddenSprite,
            _ => null
        };
    }
}

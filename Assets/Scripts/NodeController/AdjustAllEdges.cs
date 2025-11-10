using UnityEngine;

public class AdjustAllEdges : MonoBehaviour
{
    public void TriggerEdgeAdjuster()
    {
        foreach (Transform child in transform)
        {
            AdjacentNodes adjacentNodes = child.GetComponent<AdjacentNodes>();
            
            adjacentNodes?.AdjustEdges();
        }
    }
}

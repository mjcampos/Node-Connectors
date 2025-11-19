using Helpers;
using UnityEngine;

public class NodeGraphController : MonoBehaviour
{
    public void TriggerEdgeAdjuster()
    {
        foreach (Transform child in transform)
        {
            AdjacentNodes adjacentNodes = child.GetComponent<AdjacentNodes>();
            
            adjacentNodes?.AdjustEdges();
        }
    }

    public void TriggerNodeSettingsAdjuster()
    {
        foreach (Transform child in transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();

            if (nsm.state == NodeState.Unlocked)
            {
                nsm.Ripple();
            }
        }
    }
}

using System;
using Helpers;
using UnityEngine;

public class NodeGraphController : MonoBehaviour
{
    [Header("Visibility Settings")]
    [Tooltip("Maximum degrees from Unlocked nodes where nodes remain hoverable")]
    [Range(1, 10)]
    [SerializeField] int visibilityDegreeThreshold = 2;

    int _previousThreshold = -1;

    void OnValidate()
    {
        if (_previousThreshold == -1)
        {
            _previousThreshold = visibilityDegreeThreshold;
            return;
        }

        if (_previousThreshold != visibilityDegreeThreshold)
        {
            _previousThreshold = visibilityDegreeThreshold;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                TriggerNodeSettingsAdjuster();
            }
#endif
        }
    }

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

            if (nsm != null && nsm.state == NodeState.Unlocked)
            {
                nsm.Ripple();
            }
        }
    }

    public int GetVisibilityDegreeThreshold()
    {
        return visibilityDegreeThreshold;
    }
}

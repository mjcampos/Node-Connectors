using System;
using Helpers;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class NodeGraphController : MonoBehaviour
{
    [Header("Visibility Settings")]
    
    [Tooltip("Maximum degrees from Unlocked nodes where nodes remain hoverable (Visible)")]
    [Range(1, 10)]
    [SerializeField] int hoverableRange = 2;
    
    [Tooltip("Maximum degrees from Visible nodes where nodes remain non-hoverable (NonHoverable)")]
    [Range(0, 10)]
    [SerializeField] int nonHoverableRange = 2;

    int _previousHoverableRange = -1;
    int _previousNonHoverableRange = -1;

    void OnValidate()
    {
        bool rangeChanged = false;
        
        if (_previousHoverableRange == -1)
        {
            _previousHoverableRange = hoverableRange;
        }
        else if (_previousHoverableRange != hoverableRange)
        {
            _previousHoverableRange = hoverableRange;
            rangeChanged = true;
        }

        if (_previousNonHoverableRange == -1)
        {
            _previousNonHoverableRange = nonHoverableRange;
        } 
        else if (_previousNonHoverableRange != nonHoverableRange)
        {
            _previousNonHoverableRange = nonHoverableRange;
            rangeChanged = true;
        }

        if (rangeChanged)
        {
            if (!Application.isPlaying)
            {
                TriggerNodeSettingsAdjuster();
            }
        }
    }

    public void TriggerNodeSettingsAdjuster()
    {
        if (!Application.isPlaying)
        {
            ResetNonUnlockedNodes();
        }
        
        foreach (Transform child in transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();

            if (nsm != null && nsm.state == NodeState.Unlocked)
            {
                nsm.Ripple();
            }
        }
    }

    void ResetNonUnlockedNodes()
    {
        foreach (Transform child in transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();

            if (nsm != null && nsm.state != NodeState.Unlocked && nsm.state != NodeState.Locked)
            {
                nsm.degreesFromUnlocked = int.MaxValue;
                nsm.degreesFromVisible = 0;
                nsm.degreesFromNonHoverable = 0;
            }
        }
    }

    public int GetHoverableRange()
    {
        return hoverableRange;
    }

    public int GetNonHoverableRange()
    {
        return nonHoverableRange;
    }
    
    [ContextMenu("Refresh Node Graph")]
    public void RefreshNodeGraph()
    {
        TriggerNodeSettingsAdjuster();
    }
}

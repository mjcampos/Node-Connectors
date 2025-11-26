using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighborNodes = new List<Node>();

    public void AddNeighbor(Node node)
    {
        if (!neighborNodes.Contains(node))
        {
            neighborNodes.Add(node);
        }
    }
}

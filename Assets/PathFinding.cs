using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    NodeSystem ns;

    struct DistNode
    {
        public NodeSystem.Node node;
        public float f;
        public float g;
        public float h;
    }

    ArrayList openSet;
    ArrayList closedSet;

    void CalulatePath(NodeSystem.Node start, NodeSystem.Node end)
    {
        DistNode dn = new DistNode();
        dn.node = start;
        dn.f = 0;

        openSet.Add(dn);

        DistNode currentNode = new DistNode();
        currentNode.f = 1000000;

        bool b = false;

        while (!b)
        {
            //1
            for (int i = 0; i < openSet.Count; i++)
            {
                DistNode temp = (DistNode)(openSet[i]);
                if (temp.f < currentNode.f)
                    currentNode = temp;
            }

            //2
            openSet.Remove(currentNode);

            //3
            for (int i = 0; i < currentNode.node.Connections.Count; i++)
            {
                NodeSystem.Node other = ((NodeSystem.Line)start.Connections[i]).GetOther(dn.node);
                DistNode cDistNode = new DistNode();
                cDistNode.node = other;
                cDistNode.g = currentNode.g + other.DistanceTo(currentNode.node);
                cDistNode.h = other.DistanceTo(end);
                cDistNode.f = cDistNode.g + cDistNode.h;

                if (other == end)
                {
                    print("Goal Found!");
                    b = true;
                    return;
                }
                else
                {
                    openSet.Add(cDistNode);
                }
            }

            closedSet.Add(currentNode);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

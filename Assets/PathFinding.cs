using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField]
    GameController game;

    NodeSystem ns;

    NodeSystem.Node[] selectedNodes = new NodeSystem.Node[2];
    int nSelectedNodes = 0;

    [SerializeField]
    LayerMask Waypoint_layermask;

    List<NodeSystem.Node> CalulatePath(NodeSystem.Node start, NodeSystem.Node end)
    {
        List<NodeSystem.Node> openSet = new List<NodeSystem.Node>();
        List<NodeSystem.Node> closedSet = new List<NodeSystem.Node>();
        List<NodeSystem.Node> actualPath = new List<NodeSystem.Node>();

        bool foundTarget = false;

        start.parrent = null;
        openSet.Add(start);

        int rageQuit = 0;

        while (openSet.Count > 0 && !foundTarget && rageQuit != 1000)
        {
            rageQuit++;

            NodeSystem.Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == end)
            {
                foundTarget = true;
            }
            else
            {
                List<NodeSystem.Node> Neighbours = currentNode.GetNeighbours();
                print("Neighbours: " + Neighbours.Count);

                foreach (NodeSystem.Node n in Neighbours)
                {
                    if (closedSet.Contains(n))
                        continue;

                    if (n == currentNode)
                    {
                        print("Wrong: ");
                        continue;
                    }

                    float newCost = currentNode.gCost + currentNode.DistanceTo(n);
                    if (newCost < n.gCost || !openSet.Contains(n))
                    {
                        n.gCost = newCost;
                        n.hCost = n.DistanceTo(end);
                        n.parrent = currentNode;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }

                    }
                }
            }
        }

        if (foundTarget)
        {
            print("Target Found");
            NodeSystem.Node backTrack = end;
            int ragequit = 0;
            while (backTrack != null && ragequit < 1000)
            {
                ragequit++;

                actualPath.Add(backTrack);
                backTrack = backTrack.parrent;
            }

            if(ragequit>= 1000)
            {
                print("RageQuited after target found");
            }
            actualPath.Reverse();

            foreach (NodeSystem.Node n in actualPath)
            {
                print(n.X + ", " + n.Y + ", " + n.Z);
            }

                return actualPath;
        }
        else
        {
            print("Did not find target!");
        }

        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        ns = game.getNodeSystem();
    }

    // Update is called once per frame
    void Update()
    {
        if (StateManager.Instance.CurrentState() != "Path")
            return;

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, Waypoint_layermask))
            {
                bool found = false;
                NodeSystem.Node nodeHit = hit.collider.GetComponent<waypoint_script>().Node;

                for (int i = 0; i < nSelectedNodes && i < selectedNodes.Length; i++)
                {
                    if (selectedNodes[i] == nodeHit)
                    {
                        print("Found");
                        found = true;
                    }
                }

                if (!found)
                {
                    print("Not Found: " + nSelectedNodes);
                    selectedNodes[nSelectedNodes++] = nodeHit;

                    if (nSelectedNodes == 2)
                    {
                        CalulatePath(selectedNodes[0], selectedNodes[1]);
                        nSelectedNodes = 0;
                    }
                }
            }
            else
            {
                nSelectedNodes = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            CalulatePath(ns.Nodes[0], ns.Nodes[ns.Nodes.Count - 1]);
        }
    }
}

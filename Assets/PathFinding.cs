using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField]
    GameController game;

    NodeSystem ns;



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

            //print("Openset count b4 Remove: " + openSet.Count);
            openSet.Remove(currentNode);
            //print("Openset count after Remove: " + openSet.Count);

            closedSet.Add(currentNode);
            //print("Closedset count: " + closedSet.Count);

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

                        //print(n.fCost);

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                            //print("Added");
                        }
                        else
                        {
                            //print("Changed");
                        }
                    }
                }
            }
        }

        if (foundTarget)
        {
            print("Target Found");
            NodeSystem.Node backTrack = end;
            while (backTrack != null)
            {
                print("Parrent Jump");

                actualPath.Add(backTrack);
                backTrack = backTrack.parrent;
            }

            //actualPath.Reverse();

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
        if (Input.GetKeyUp(KeyCode.A))
        {
            CalulatePath(ns.Nodes[0], ns.Nodes[ns.Nodes.Count - 1]);
        }
    }
}

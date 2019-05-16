using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinding : MonoBehaviour
{
    [SerializeField]
    GameController game;

    [SerializeField]
    GameObject TextPrefab;
    [SerializeField]
    GameObject ghostCubePrefab;

    [SerializeField]
    GameObject errorMessage;

    [System.Serializable]
    class TypeButton
    {
        [SerializeField]
        Toggle toggle;
        [SerializeField]
        Button button;
        public void ShowToggle(bool b)
        {
            toggle.gameObject.SetActive(b);
            button.gameObject.SetActive(!b);
        }

        public bool isToggleOn()
        {
            return toggle.isOn;
        }
    }

    [SerializeField]
    TypeButton[] typeButtons;

    NodeSystem ns;

    //NodeSystem.Node[] selectedNodes = new NodeSystem.Node[2];
    //int nSelectedNodes = 0;

    List<NodeSystem.Node> m_selectedNodes = new List<NodeSystem.Node>();
    List<GameObject> m_textMeshes = new List<GameObject>();
    List<GameObject> m_ghostCubes = new List<GameObject>();

    [SerializeField]
    Material originalMaterial;

    [SerializeField]
    LayerMask Waypoint_layermask;

    List<NodeSystem.Node> CalulatePath(NodeSystem.Node start, NodeSystem.Node end, List<int> allowedTypes = null)
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
                List<NodeSystem.Node> Neighbours = currentNode.GetNeighbours(allowedTypes);
                //print("Neighbours: " + Neighbours.Count);

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

            //foreach (NodeSystem.Node n in actualPath)
            //{
            //    print(n.X + ", " + n.Y + ", " + n.Z);
            //}

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

    void Clear()
    {
        foreach (GameObject obj in m_textMeshes)
        {
            Destroy(obj);
        }

        m_textMeshes.Clear();
        m_selectedNodes.Clear();
    }

    void UnselectPath()
    {
        for (int i = 0; i < m_ghostCubes.Count; i++)
        {
            Destroy(m_ghostCubes[i]);
        }
        m_ghostCubes.Clear();
    }

    void SelectPath(List<NodeSystem.Node> path)
    {
        UnselectPath();

        for (int i = 0; i < 1/*max number of cubes*/ && i < path.Count - 1 /*Upper cube limit(failsafe)*/; i++)
        {
            GameObject ghostCube = Instantiate(ghostCubePrefab);
            ghostCube.GetComponent<PathFindingCube>().path = path;
            ghostCube.GetComponent<PathFindingCube>().setPathPos(i);
            ghostCube.GetComponent<PathFindingCube>().speed = 10;
            m_ghostCubes.Add(ghostCube);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < typeButtons.Length; i++)
        {
            typeButtons[i].ShowToggle(StateManager.Instance.CurrentState() == "Path");
        }

        if (StateManager.Instance.CurrentState() != "Path")
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Clear();
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if(m_selectedNodes.Count < 2)
            {
                errorMessage.GetComponent<Animator>().SetTrigger("Show");
                return;
            }

            List<NodeSystem.Node> path = new List<NodeSystem.Node>();
            List<int> allowedTypes = new List<int>();

            for (int i = 0; i < typeButtons.Length; i++)
            {
                if (typeButtons[i].isToggleOn())
                    allowedTypes.Add(i);
            }

            for (int i = 0; i < m_selectedNodes.Count-1; i++)
            {
                path.AddRange(CalulatePath(m_selectedNodes[i], m_selectedNodes[i+1], allowedTypes));

                //Remove Duplications
                if (i != m_selectedNodes.Count - 2)
                    path.RemoveAt(path.Count - 1);
            }

            SelectPath(path);
            //Clear();
            m_selectedNodes.Clear();
            //Dictionary<NodeSystem.Node, int> nVisitsToNode = new Dictionary<NodeSystem.Node, int>();

            //for (int i = 0; i < path.Count; i++)
            //{
            //    if (!nVisitsToNode.ContainsKey(path[i]))
            //        nVisitsToNode[path[i]] = 0;

            //    GameObject text = Instantiate(TextPrefab);
            //    text.transform.position = new Vector3(path[i].X + (nVisitsToNode[path[i]]++)*1.4f, path[i].Y, path[i].Z);
            //    text.GetComponent<TextMesh>().text = /*((nVisitsToNode[path[i]] > 1) ? "" : "")*/ i + 1 + ",";
            //    m_textMeshes.Add(text);
            //    //nVisitsToNode[path[i]]++;
            //}
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_selectedNodes.Count == 0 && m_textMeshes.Count > 0)
                Clear();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Waypoints")))
            {
                print("Hit");

                bool found = false;
                //hit.collider.GetComponent<MeshRenderer>().material.color = Color.cyan;

                NodeSystem.Node nodeHit = hit.collider.GetComponent<waypoint_script>().Node;

                for (int i = 0; i < m_selectedNodes.Count && !found; i++)
                {
                    if (m_selectedNodes[i] == nodeHit)
                    {
                        print("Found");
                        found = true;
                    }
                }

                if (!found)
                {
                    m_selectedNodes.Add(nodeHit);
                    GameObject text = Instantiate(TextPrefab);
                    text.transform.position = new Vector3(nodeHit.X, nodeHit.Y, nodeHit.Z);
                    text.GetComponent<TextMesh>().text = "" + m_selectedNodes.Count;
                    m_textMeshes.Add(text);

                    //if (nSelectedNodes == 2)
                    //{
                    //    CalulatePath(selectedNodes[0], selectedNodes[1]);
                    //    nSelectedNodes = 0;
                    //}
                }
            }
            else
            {
                //nSelectedNodes = 0;
                print("No Hit");
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            CalulatePath(ns.Nodes[0], ns.Nodes[ns.Nodes.Count - 1]);
        }
    }
}

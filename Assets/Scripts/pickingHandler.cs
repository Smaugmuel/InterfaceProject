using UnityEngine;
using System.Collections;

struct Connection
{
    public GameObject line;
    public GameObject startNode;
    public GameObject endNode;
}

public class pickingHandler : MonoBehaviour
{
    private const int MAX_GHOST_COUNT = 10;
    private Vector3 placeOffset = new Vector3(0f, 1f, 0f);
    public static Vector2 cameraLookAt = new Vector2(0f, 0f);


    // Variables for node system usage
    [SerializeField]
    public GameController gc;
    //ArrayList m_waypoints = new ArrayList();
    //ArrayList m_connections = new ArrayList();
    public UI_nodePanel nodePanel;
    private Color standardColor;
    private Color selectColor;
    GameObject selectedNode;

    // Models for instantiation
    //public GameObject model_waypoint;
    public GameObject model_ghost;
    public Material material_waypoint;
    public Material material_highlight;
    //public GameObject model_connection;

    public ObjectHandler oh;

    // Temp list of ghosts when multiple alternatives occurs
    private GameObject[] ghostList = new GameObject[MAX_GHOST_COUNT];
    private int ghostCount;

    // To keep track of positioning prediction
    private Vector3 lastHitPos;
    private Vector3 lastPlacedPos;
    private GameObject lastPlaced;
    private bool lastPlacedRestart;

    // For adding connections manually
    private GameObject[] con_selectedNodes = new GameObject[2];
    private int con_selectedCount = 0; // Up to two

    // Variables for position calculation
    public RectTransform sideViewRect;
    public Canvas ui_canvas;

    // Cameras to raycast from
    public Camera camera_main;
    public Camera camera_side;

    // DEBUG, 0 = waypoint, 1 = connection
    [Range(0,1)]
    public int DEBUG_STATE;

    // Used by CameraMovement.cs
    [HideInInspector]
    public static Vector3 sideCameraLookAt = new Vector3();

    private void Start()
    {
        // Default start position is not reachable from the main camera.
        // Default has a high y value to "predict" the first app interaction
        // to be placed on the highest position possible.
        lastHitPos = new Vector3(0f, 100f, 0f);
        lastPlacedPos = new Vector3(0f, 100f, 0f);
        lastPlacedRestart = true;
        ClearGhostList();
        standardColor = Color.blue;
        selectColor = Color.green;
        selectedNode = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeState(1 - DEBUG_STATE);
        }

        if (!Input.GetKey("space")) 
        {
//<<<<<<< HEAD
//            if (Input.GetMouseButtonDown(0))
//=======
            // Check if mouse pos is MAIN VIEW or SIDE VIEW (exclude node list)
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            cameraLookAt = mousePos;

            // Calculate SIDE VIEW min-max coordinates
            Vector2 sideViewSize = new Vector2(sideViewRect.rect.width * ui_canvas.scaleFactor, sideViewRect.rect.height * ui_canvas.scaleFactor);
            Vector2 sideMin = new Vector2(sideViewRect.position.x - sideViewSize.x / 2f, sideViewRect.position.y - sideViewSize.y / 2f);
            Vector2 sideMax = new Vector2(sideViewRect.position.x + sideViewSize.x / 2f, sideViewRect.position.y + sideViewSize.y / 2f);

            // Inside MAIN VIEW
            //if (mousePos.x < sideMin.x)
            if (Input.GetMouseButtonDown(0))
//>>>>>>> lhure-frustum-from-camera
            {
                // Check if mouse pos is MAIN VIEW or SIDE VIEW (exclude node list)
                //Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //// Calculate SIDE VIEW min-max coordinates
                //Vector2 sideViewSize = new Vector2(sideViewRect.rect.width * ui_canvas.scaleFactor, sideViewRect.rect.height * ui_canvas.scaleFactor);
                //Vector2 sideMin = new Vector2(sideViewRect.position.x - sideViewSize.x / 2f, sideViewRect.position.y - sideViewSize.y / 2f);
                //Vector2 sideMax = new Vector2(sideViewRect.position.x + sideViewSize.x / 2f, sideViewRect.position.y + sideViewSize.y / 2f);

                // Inside MAIN VIEW
                if (mousePos.x < sideMin.x)
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);

                    // Check if new position, include delta to eliminate -to close points-
                    // mouseWorldPoint is necessary since camera can move and lastPlacedPos is in world space
                    Vector3 mouseWorldPoint = camera_main.ViewportToWorldPoint(mousePos);
                    Vector2 mouseWorldPoint2D = new Vector2(ray.origin.x, ray.origin.z); // Possible since ray direction is (0, -1, 0)
                    Vector2 lastPlaced2D = new Vector2(lastHitPos.x, lastHitPos.z);

                    if (DEBUG_STATE == 0) // Waypoint state
                    {
                        // New position
                        if (Vector2.Distance(mouseWorldPoint2D, lastPlaced2D) > 0.5f)
                        {

                            //// If only one hit -> place waypoint
                            //if (hits.Length == 1)
                            //{
                            //    SpawnWaypoint(hits[0].point);


                            //}
                            //else if (hits.Length > 1)
                            //{
                            //    // Sort list by hight, highest to lowest.
                            //    // Eliminate all fake hits.
                            //    hits = SortHitList(hits);

                            ClearGhostList();

                            // Find all possible placements from click, mask off waypoints
                            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, 9);

                            // If only one hit -> place waypoint
                            if (hits.Length == 1)
                            {
                                //SpawnWaypoint(hits[0].point);
                                SpawnWaypointFromHitPos(hits[0].point);
                            }
                            else if (hits.Length > 1)
                            {
                                // Sort list by hight, highest to lowest.
                                // Eliminate all fake hits.
                                hits = SortHitList(hits);

                                // Move side camera
                                MoveSideCamera(GetAverageHitPos(hits));

                                // Predict user choice from hits
                                int predicted = GetPredictedIndex(hits);

                                for (int i = 0; i < hits.Length; i++)
                                {
                                    if (i == predicted)
                                    {
                                        //SpawnWaypoint(hits[i].point);
                                        SpawnWaypointFromHitPos(hits[i].point);
                                    }
                                    else
                                    {
                                        SpawnGhost(hits[i].point);
                                    }
                                }
                            }
                        }
                    }
                    else if (DEBUG_STATE == 1) // Connection state
                    {
                        // 1. Wait for two nodes to be selected
                        // 2. Spawn a connection between them

                        // Check if a waypoint is hit
                        LayerMask mask = LayerMask.GetMask("Waypoints");
                        RaycastHit hit;// = Physics.Raycast(ray, 1000f, mask);

                        if (Physics.Raycast(ray, out hit, 1000f, mask))
                        {
                            con_selectedNodes[con_selectedCount++] = hit.collider.gameObject;
                            hit.collider.gameObject.GetComponent<MeshRenderer>().material = material_highlight;

                            if (con_selectedCount == 2)
                            {
                                // Create connection and reset
                                oh.AddConnection(con_selectedNodes[0], con_selectedNodes[1]);
                                ResetConnectionListener();
                            }
                        }
                        else // If miss
                        {
                            // Reset highlight
                            ResetConnectionListener();
                        }
                    }
                }
                else if (mousePos.y < sideMax.y) // Inside SIDE VIEW (given !(mousePos.x < sideMin.x))
                {
                    // Calculate local coordinates within side view -window-
                    Vector2 localPos = mousePos - sideMin;

                    // Convert coordinate to [0;1]
                    localPos /= sideViewSize;

                    // Raycast into world from side camera
                    Ray ray = camera_side.ViewportPointToRay(localPos);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 1000f))
                    {
                        if (IsGhost(hit.collider.gameObject))
                        {
                            ConvertWaypointToGhost(lastPlaced);
                            ConvertGhostToWaypoint(hit.collider.gameObject);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) // Rightclick
            {
                if(true) // ifstatement if removing objects
                {
                    // Inside MAIN VIEW
                    if (mousePos.x < sideMin.x)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(mousePos);

                        // Check if new position, include delta to eliminate -to close points-
                        // mouseWorldPoint is necessary since camera can move and lastPlacedPos is in world space
                        Vector3 mouseWorldPoint = camera_main.ViewportToWorldPoint(mousePos);
                        Vector2 mouseWorldPoint2D = new Vector2(ray.origin.x, ray.origin.z); // Possible since ray direction is (0, -1, 0)
                        Vector2 lastPlaced2D = new Vector2(lastPlacedPos.x, lastPlacedPos.z);

                        ClearGhostList();

                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 1000f))
                        {
                            if (IsWaypoint(hit.collider.gameObject))
                            {
                                EraseWaypoint(hit.collider.gameObject);
                            }
                            else
                            {
                                // Move side camera if no objekt was hit
                                MoveSideCamera(hit.point);
                            }

                        }

                    }
                    else if (mousePos.y < sideMax.y) // Inside SIDE VIEW (given !(mousePos.x < sideMin.x))
                    {
                        // Calculate local coordinates within side view -window-
                        Vector2 localPos = mousePos - sideMin;

                        // Convert coordinate to [0;1]
                        localPos /= sideViewSize;

                        // Raycast into world from side camera
                        Ray ray = camera_side.ViewportPointToRay(localPos);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 1000f))
                        {
                            if (IsWaypoint(hit.collider.gameObject))
                            {
                                EraseWaypoint(hit.collider.gameObject);
                            }
                            //hit.collider.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f);
                        }
                    }
                }

                

            }
        }
    }



    ////
    //  Private help functions
    ////

    void ClearGhostList()
    {
        for (int i = 0; i < MAX_GHOST_COUNT; i++)
        {
            Destroy(ghostList[i]);
            ghostList[i] = null;
        }
        ghostCount = 0;
    }

    void SpawnWaypointFromHitPos(Vector3 pos)
    {
        lastHitPos = pos;
        pos += placeOffset;
        GameObject obj = oh.AddWaypoint(pos);

        // Spawn connection between the two latest waypoints
        //if (m_waypoints.Count > 1 && !lastPlacedRestart)
        if (oh.m_waypoints.Count > 1 && lastPlaced != null)
        {
            oh.AddConnection(lastPlaced, obj);
        }

        lastPlaced = obj;
        lastPlacedRestart = false;
    }

    void SpawnGhost(Vector3 pos)
    {
        pos += placeOffset;
        GameObject obj = Instantiate(model_ghost, pos, Quaternion.identity);

        // Push to ghostList
        ghostList[ghostCount++] = obj;
    }

    void ConvertGhostToWaypoint(GameObject ghost)
    {
        Vector3 pos = ghost.transform.position - placeOffset;
        ClearGhostFromList(ghost);
        Destroy(ghost);
        SpawnWaypointFromHitPos(pos);
    }

    void ConvertWaypointToGhost(GameObject waypoint)
    {
        Vector3 pos = waypoint.transform.position - placeOffset;
        //EraseWaypoint(waypoint);
        ArrayList neighbors = oh.GetNeighbors(waypoint);


        // In this implementation, ghosts only appears
        // on NEW waypoints. Here, the new waypoint only
        // has ONE connection.
        lastPlaced = (GameObject)neighbors[0];

        oh.RemoveWaypoint(waypoint);
        SpawnGhost(pos);
    }

    void EraseWaypoint(GameObject waypoint)
    {
        //for (int i = 0; i < m_waypoints.Count; i++)
        //{
        //    if (m_waypoints[i] == waypoint)
        //    {
        //        m_waypoints.RemoveAt(i);
        //        break;
        //    }
        //}

        // Assigns first and last waypoints as placeholders
        //GameObject connectedWaypoint0 = new GameObject();
        //GameObject connectedWaypoint1 = new GameObject();
        //bool foundConnectedWaypoint0 = false;
        //bool foundConnectedWaypoint1 = false;
        //for (int i = m_connections.Count - 1; i >= 0; i--)
        //{
        //    if (((Connection)m_connections[i]).endNode   == waypoint ||
        //        ((Connection)m_connections[i]).startNode == waypoint)
        //    {

        //        if (((Connection)m_connections[i]).endNode == waypoint)
        //        {
        //            connectedWaypoint0 = ((Connection)m_connections[i]).startNode;
        //            foundConnectedWaypoint0 = true;
        //        }
        //        else
        //        {
        //            connectedWaypoint1 = ((Connection)m_connections[i]).endNode;
        //            foundConnectedWaypoint1 = true;
        //        }

        //        RemoveConnection(i);

        //        //break;
        //    }
        //}

        //if (foundConnectedWaypoint0 && foundConnectedWaypoint0)
        //{
        //    if(true) // If the state for creating a new connection
        //        oh.AddConnection(connectedWaypoint0, connectedWaypoint1);
        //        //CreateConnection(connectedWaypoint0, connectedWaypoint1);
        //    lastPlaced = connectedWaypoint1;
        //}
        //else if (foundConnectedWaypoint0)
        //    lastPlaced = connectedWaypoint0;
        //else if (foundConnectedWaypoint1)
        //    lastPlaced = connectedWaypoint1;
        //else if (m_waypoints.Count > 1)
        //{
        //    lastPlacedRestart = true;
        //}


        //gc.getNodeSystem().RemoveNode(waypoint.GetComponent<waypoint_script>().Node);
        //Destroy(waypoint);

        //nodePanel.UpdateUI();

        oh.RemoveWaypoint(waypoint);

    }
    //void RemoveConnection(int index)
    //{
    //    Debug.Log("Connection " + index + " is getting removed");

    //    Connection conToDestroy = (Connection)m_connections[index];
    //    m_connections.RemoveAt(index);
    //    gc.getNodeSystem().RemoveLine(conToDestroy.line.GetComponent<connection_script>().Line);
    //    Destroy(conToDestroy.line);
    //}

    bool IsGhost(GameObject obj)
    {
        for (int i = 0; i < ghostCount; i++)
        {
            if (obj == ghostList[i])
            {
                return true;
            }
        }

        return false;
    }

    bool IsWaypoint(GameObject obj)
    {   
        //if (m_waypoints.Contains(obj))
            
        return oh.m_waypoints.Contains(obj);
    }

    void ClearGhostFromList(GameObject ghost)
    {
        for (int i = 0; i < ghostCount; i++)
        {
            if (ghostList[i] == ghost)
            {
                ghostList[i] = ghostList[ghostCount - 1];
                ghostList[ghostCount - 1] = null;
                ghostCount--;
                return;
            }
        }
    }

    RaycastHit[] SortHitList(RaycastHit[] hits)
    {
        const float minimumDistance = 1f;
        int realHitCount = 0;

        // Sort list, Bubble sort <3
        for (int i = 0; i < hits.Length; i++)
        {
            for (int j = i; j < hits.Length - 1; j++)
            {
                if (hits[j].point.y < hits[j + 1].point.y)
                {
                    RaycastHit temp = hits[j];
                    hits[j] = hits[j + 1];
                    hits[j + 1] = temp;
                }
            }
        }

        // Count real hit count
        // Avoid hits that are to close to the one before it
        // This avoids multiple waypoints at one "plank-intersection"
        bool[] hitIsReal = new bool[hits.Length];
        realHitCount++; hitIsReal[0] = true; // First hit always counts.
        for (int i = 1; i < hits.Length; i++)
        {
            hitIsReal[i] = Vector3.Distance(hits[i].point, hits[i - 1].point) >= minimumDistance;
            if (hitIsReal[i])
            {
                realHitCount++;
            }
        }

        RaycastHit[] hits_sorted = new RaycastHit[realHitCount];

        // Copy list, avoid fake hits
        int nextIndex = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hitIsReal[i])
            {
                hits_sorted[nextIndex++] = hits[i];
            }
        }

        return hits_sorted;
    }

    Vector3 GetAverageHitPos(RaycastHit[] hits)
    {
        Vector3 average = hits[0].point;

        for (int i = 1; i < hits.Length; i++)
        {
            average += hits[i].point;
        }
        average /= hits.Length;

        return average;
    }

    void MoveSideCamera(Vector3 lookAt)
    {
        lookAt += placeOffset;
        
        Vector3 dirFromOrigo = new Vector3(lookAt.x, 0f, lookAt.z).normalized * 6f;

        camera_side.transform.position = lookAt + dirFromOrigo + new Vector3(0, 3f, 0f);
        camera_side.transform.LookAt(lookAt);
        
        sideCameraLookAt = lookAt;
    }

    int GetPredictedIndex(RaycastHit[] hits)
    {
        int closest = 0;
        float closestDistance = GetHightDistance(hits[0].point, lastHitPos);

        float distance;
        for (int i = 1; i < hits.Length; i++)
        {
            distance = GetHightDistance(hits[i].point, lastHitPos);
            if (distance < closestDistance)
            {
                closest = i;
                closestDistance = distance;
            }
        }

        return closest;
    }

    float GetHightDistance(Vector3 v, Vector3 u)
    {
        return Mathf.Abs((v - u).y);
    }

    void ResetConnectionListener()
    {
        if (con_selectedNodes[0] != null)
        {
            con_selectedNodes[0].GetComponent<MeshRenderer>().material = material_waypoint;
            con_selectedNodes[0] = null;
        }
        if (con_selectedNodes[1] != null)
        {
            con_selectedNodes[1].GetComponent<MeshRenderer>().material = material_waypoint;
            con_selectedNodes[1] = null;
        }
        con_selectedCount = 0;
    }


    ////
    //  Public access functions 
    ////

    public void ChangeState(int state)
    {
        DEBUG_STATE = state;
        con_selectedCount = 0;
        lastPlaced = null;
    }

    public void SetSelectedNode(NodeSystem.Node node)
    {
        //for (int i = 0; i < m_waypoints.Count; i++)
        //{
        //    if (((GameObject)m_waypoints[i]).GetComponent<waypoint_script>().Node == node)
        //    {
        //        if (selectedNode != null)
        //            selectedNode.GetComponent<Renderer>().material.color = standardColor;
        //        selectedNode = ((GameObject)m_waypoints[i]);
        //        selectedNode.GetComponent<Renderer>().material.color = selectColor;

        //        return;
        //    }
        //}
        if (selectedNode != null)
        {
            selectedNode.GetComponent<Renderer>().material = material_waypoint;
        }
        selectedNode = oh.GetWaypoint(node);
        selectedNode.GetComponent<Renderer>().material = material_highlight;
    }
}

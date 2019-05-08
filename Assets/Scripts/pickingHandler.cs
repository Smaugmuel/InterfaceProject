﻿using UnityEngine;
using System.Collections;

public class pickingHandler : MonoBehaviour
{
    private const int MAX_GHOST_COUNT = 10;
    private Vector3 placeOffset = new Vector3(0f, 1f, 0f);

    // Variables for node system usage
    [SerializeField]
    public GameController gc;
    ArrayList m_waypoints = new ArrayList();
    public UI_nodePanel nodePanel;
    private Color standardColor;
    private Color selectColor;
    GameObject selectedNode;

    // Models for instantiation
    public GameObject model_waypoint;
    public GameObject model_ghost;

    // Temp list of ghosts when multiple alternatives occurs
    private GameObject[] ghostList = new GameObject[MAX_GHOST_COUNT];
    private int ghostCount;

    // To keep track of positioning prediction
    private Vector3 lastPlacedPos;
    private GameObject lastPlaced;

    // Variables for position calculation
    public RectTransform sideViewRect;
    public Canvas ui_canvas;

    // Cameras to raycast from
    public Camera camera_main;
    public Camera camera_side;

    private void Start()
    {
        // Default start position is not reachable from the main camera.
        // Default has a high y value to "predict" the first app interaction
        // to be placed on the highest position possible.
        lastPlacedPos = new Vector3(0f, 100f, 0f);
        ClearGhostList();
        standardColor = Color.blue;
        selectColor = Color.green;
        selectedNode = null;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // Check if mouse pos is MAIN VIEW or SIDE VIEW (exclude node list)
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            // Calculate SIDE VIEW min-max coordinates
            Vector2 sideViewSize = new Vector2(sideViewRect.rect.width * ui_canvas.scaleFactor, sideViewRect.rect.height * ui_canvas.scaleFactor);
            Vector2 sideMin = new Vector2(sideViewRect.position.x - sideViewSize.x / 2f, sideViewRect.position.y - sideViewSize.y / 2f);
            Vector2 sideMax = new Vector2(sideViewRect.position.x + sideViewSize.x / 2f, sideViewRect.position.y + sideViewSize.y / 2f);

            // Inside MAIN VIEW
            if (mousePos.x < sideMin.x)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                // Check if new position, include delta to eliminate -to close points-
                // mouseWorldPoint is necessary since camera can move and lastPlacedPos is in world space
                Vector3 mouseWorldPoint = camera_main.ViewportToWorldPoint(mousePos);
                Vector2 mouseWorldPoint2D = new Vector2(ray.origin.x, ray.origin.z); // Possible since ray direction is (0, -1, 0)
                Vector2 lastPlaced2D = new Vector2(lastPlacedPos.x, lastPlacedPos.z);

                // New position
                if (Vector2.Distance(mouseWorldPoint2D, lastPlaced2D) > 0.5f)
                {
                    ClearGhostList();

                    // Find all possible placements from click, mask waypoints
                    RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, 9);

                    // If only one hit -> place waypoint
                    if (hits.Length == 1)
                    {
                        SpawnWaypoint(hits[0].point);
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
                                SpawnWaypoint(hits[i].point);
                            }
                            else
                            {
                                SpawnGhost(hits[i].point);
                            }
                        }
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
                    //hit.collider.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f);
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

    void SpawnWaypoint(Vector3 pos)
    {
        lastPlacedPos = pos;
        pos += placeOffset;
        GameObject obj = Instantiate(model_waypoint, pos, Quaternion.identity);
        lastPlaced = obj;

        // Add to node system
        obj.GetComponent<waypoint_script>().Node = gc.getNodeSystem().AddNode(pos.x, pos.y, pos.z);
        m_waypoints.Add(obj);
        nodePanel.UpdateUI();
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
        SpawnWaypoint(pos);
    }

    void ConvertWaypointToGhost(GameObject waypoint)
    {
        Vector3 pos = waypoint.transform.position - placeOffset;
        gc.getNodeSystem().RemoveNode(waypoint.GetComponent<waypoint_script>().Node);
        Destroy(waypoint);
        SpawnGhost(pos);
    }

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
        // All waypoint models offsets upwards -> offset lookAt too
        lookAt += placeOffset;
        Vector3 dirFromOrigo = new Vector3(lookAt.x, 0f, lookAt.y).normalized * 6f;

        camera_side.transform.position = lookAt + dirFromOrigo + new Vector3(0, 3f, 0f);
        camera_side.transform.LookAt(lookAt);
    }

    int GetPredictedIndex(RaycastHit[] hits)
    {
        int closest = 0;
        float closestDistance = GetHightDistance(hits[0].point, lastPlacedPos);

        float distance;
        for (int i = 1; i < hits.Length; i++)
        {
            distance = GetHightDistance(hits[i].point, lastPlacedPos);
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



    ////
    //  Public access functions 
    ////

    public void SetSelectedNode(NodeSystem.Node node)
    {
        for (int i = 0; i < m_waypoints.Count; i++)
        {
            if (((GameObject)m_waypoints[i]).GetComponent<waypoint_script>().Node == node)
            {
                if (selectedNode != null)
                    selectedNode.GetComponent<Renderer>().material.color = standardColor;
                selectedNode = ((GameObject)m_waypoints[i]);
                selectedNode.GetComponent<Renderer>().material.color = selectColor;

                return;
            }
        }
    }
}
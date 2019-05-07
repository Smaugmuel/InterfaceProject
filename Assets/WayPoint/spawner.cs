using UnityEngine;
using System.Collections;
public class spawner : MonoBehaviour
{
    public GameObject waypoint_model;

    Color standarColor;
    Color selectedColor;

    [SerializeField]
    public GameController gc;
    [SerializeField]
    UI_nodePanel nodePanel;

    ArrayList m_waypoints = new ArrayList();
    GameObject selectedNode;
    public void SetSelectedNode(NodeSystem.Node node)
    {
        for(int i = 0; i < m_waypoints.Count; i++)
        {
            if(((GameObject)m_waypoints[i]).GetComponent<waypoint_script>().Node == node)
            {
                if(selectedNode != null)
                    selectedNode.GetComponent<Renderer>().material.color = standarColor;
                selectedNode = ((GameObject)m_waypoints[i]);
                selectedNode.GetComponent<Renderer>().material.color = selectedColor;

                return;
            }
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        standarColor = Color.blue;
        selectedColor = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, 9);

            hits = sortHitList(hits);

            // If hit
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    SpawnWaypoint(hits[i].point);
                }
            }
        }
    }

    void SpawnWaypoint(Vector3 pos)
    {
        pos += new Vector3(0f, 1f, 0f);
        GameObject obj = Instantiate(waypoint_model, pos, Quaternion.identity);
        obj.GetComponent<Renderer>().material.color = standarColor;
        waypoint_script wp = obj.GetComponent<waypoint_script>();

        NodeSystem ns = gc.getNodeSystem();
        NodeSystem.Node node = ns.AddNode(pos.x, pos.y, pos.z);
        wp.Node = node;

        m_waypoints.Add(obj);

        nodePanel.UpdateUI();
    }

    RaycastHit[] sortHitList(RaycastHit[] hits)
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
        realHitCount++; // First hit always counts.
        hitIsReal[0] = true;
        for (int i = 1; i < hits.Length; i++)
        {
            hitIsReal[i] = Vector3.Distance(hits[i].point, hits[i - 1].point) >= minimumDistance;
            if (hitIsReal[i])
            {
                realHitCount++;
            }
        }

        RaycastHit[] hits_sorted = new RaycastHit[realHitCount];

        // Copy list
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
}

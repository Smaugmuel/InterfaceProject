using UnityEngine;
using System.Collections;

public struct Connection
{
    public GameObject line;
    public GameObject startNode;
    public GameObject endNode;

    private int type;

    // Changing type will change the material of the connection object.
    // Therefor, the change will happen in a function where the variable itself
    // is private.
    public void SetType(int _type)
    {
        if (type < 0 || type > 2)
        {
            Debug.Log("Connection.SetType(" + _type + "): Invalid type");
            return;
        }
        type = _type;
        line.GetComponent<MeshRenderer>().material = Connection.materials[type];
    }
    public int GetType()
    {
        return type;
    }


    public static Material[] materials = new Material[3];
}

public class ObjectHandler : MonoBehaviour
{
    public GameObject model_waypoint;
    public GameObject model_connection;

    public ArrayList m_waypoints = new ArrayList();
    public ArrayList m_connections = new ArrayList();

    public GameController gc;
    public UI_nodePanel nodePanel;

    public Material mat0;
    public Material mat1;
    public Material mat2;


    // Start is called before the first frame update
    void Start()
    {
        Connection.materials[0] = mat0;
        Connection.materials[1] = mat1;
        Connection.materials[2] = mat2;
    }

    // Update is called once per frame
    void Update()
    {

    }




    // Adds
    public GameObject AddWaypoint(Vector3 pos)
    {
        GameObject obj = Instantiate(model_waypoint, pos, Quaternion.identity);
        m_waypoints.Add(obj);

        // Add to node system
        obj.GetComponent<waypoint_script>().Node = gc.getNodeSystem().AddNode(pos.x, pos.y, pos.z);
        nodePanel.UpdateUI();

        return obj;
    }

    public GameObject AddWaypoint(NodeSystem.Node node)
    {
        Vector3 pos = new Vector3(node.X, node.Y, node.Z);

        GameObject obj = Instantiate(model_waypoint, pos, Quaternion.identity);
        m_waypoints.Add(obj);

        // Add to node system
        obj.GetComponent<waypoint_script>().Node = node;
        nodePanel.UpdateUI();

        return obj;
    }

    public void AddConnection(GameObject start, GameObject end, int type = 0)
    {
        Vector3 midPoint = (start.transform.position + end.transform.position) / 2f;
        Vector3 direction = end.transform.position - start.transform.position;
        float length = direction.magnitude;
        direction = Vector3.Normalize(direction);

        // Calculate forward
        Vector3 forward;
        if (direction == Vector3.up)
        {
            forward = Vector3.right;
        }
        else
        {
            forward = Vector3.Cross(direction, Vector3.Cross(Vector3.up, direction));
        }

        GameObject obj = Instantiate(model_connection, midPoint, Quaternion.LookRotation(forward, direction));
        Vector3 newScale = obj.transform.localScale;
        newScale.y = length;
        obj.transform.localScale = newScale / 2f;

        Connection newCon = new Connection();
        newCon.startNode = start;
        newCon.endNode = end;
        newCon.line = obj;
        newCon.SetType(type);
        m_connections.Add(newCon);

        // Add to node system
        obj.GetComponent<connection_script>().Line = gc.getNodeSystem().AddLine(
            start.GetComponent<waypoint_script>().Node,
            end.GetComponent<waypoint_script>().Node,
            type);
        nodePanel.UpdateUI();
    }

    public void AddConnection(NodeSystem.Line line)
    {
        Vector3 pos1 = new Vector3(line.Nodes[0].X, line.Nodes[0].Y, line.Nodes[0].Z);
        Vector3 pos2 = new Vector3(line.Nodes[1].X, line.Nodes[1].Y, line.Nodes[1].Z);

        GameObject start = Instantiate(model_waypoint, pos1, Quaternion.identity);
        GameObject end = Instantiate(model_waypoint, pos2, Quaternion.identity);

        Vector3 midPoint = (start.transform.position + end.transform.position) / 2f;
        Vector3 direction = end.transform.position - start.transform.position;
        float length = direction.magnitude;
        direction = Vector3.Normalize(direction);

        // Calculate forward
        Vector3 forward;
        if (direction == Vector3.up)
        {
            forward = Vector3.right;
        }
        else
        {
            forward = Vector3.Cross(direction, Vector3.Cross(Vector3.up, direction));
        }

        GameObject obj = Instantiate(model_connection, midPoint, Quaternion.LookRotation(forward, direction));
        Vector3 newScale = obj.transform.localScale;
        newScale.y = length;
        obj.transform.localScale = newScale / 2f;

        Connection newCon = new Connection();
        newCon.startNode = start;
        newCon.endNode = end;
        newCon.line = obj;
        newCon.SetType(line.Type);
        m_connections.Add(newCon);

        // Add to node system
        obj.GetComponent<connection_script>().Line = line;
        nodePanel.UpdateUI();
    }

    // Removes
    public void RemoveWaypoint(GameObject waypoint)
    {
        // Find any connection the waypoint is connected to and erase it
        for (int i = m_connections.Count - 1; i >= 0; i--)
        {
            if (((Connection)m_connections[i]).endNode == waypoint ||
                ((Connection)m_connections[i]).startNode == waypoint)
            {
                Connection conToDestroy = (Connection)m_connections[i];
                m_connections.RemoveAt(i);
                gc.getNodeSystem().RemoveLine(conToDestroy.line.GetComponent<connection_script>().Line);
                Destroy(conToDestroy.line);
            }
        }

        // Remove waypoint node from node system
        gc.getNodeSystem().RemoveNode(waypoint.GetComponent<waypoint_script>().Node);
       
        // Remove from list
        m_waypoints.Remove(waypoint);
        Destroy(waypoint);
    }

    public void RemoveWaypoint(NodeSystem.Node node)
    {
        GameObject waypoint = null;
        // Find waypoint of node
        for (int i = 0; i < m_waypoints.Count; i++)
        {
            if (((GameObject)m_waypoints[i]).GetComponent<waypoint_script>().Node == node)
            {
                waypoint = (GameObject)m_waypoints[i];
                break;
            }
        }

        // If waypoint exists
        if (waypoint != null)
        {
            // Find any connection the waypoint is connected to and erase it
            for (int i = m_connections.Count - 1; i >= 0; i--)
            {
                if (((Connection)m_connections[i]).endNode == waypoint ||
                    ((Connection)m_connections[i]).startNode == waypoint)
                {
                    Connection conToDestroy = (Connection)m_connections[i];
                    m_connections.RemoveAt(i);
                    gc.getNodeSystem().RemoveLine(conToDestroy.line.GetComponent<connection_script>().Line);
                    Destroy(conToDestroy.line);
                }
            }

            // Remove waypoint node from node system
            gc.getNodeSystem().RemoveNode(waypoint.GetComponent<waypoint_script>().Node);

            // Remove from list
            m_waypoints.Remove(waypoint);
            Destroy(waypoint);
        }
    }

    public void RemoveConnection(GameObject connection)
    {
        bool found = false;

        // Cant set to null, will never be used of 'found' is false
        // anyway
        Connection toRemove = (Connection)m_connections[0];

        // Find connection in list
        for (int i = 0; i < m_connections.Count; i++)
        {
            if (((Connection)m_connections[i]).line == connection)
            {
                toRemove = (Connection)m_connections[i];
                found = true;
                break;
            }
        }

        if (found)
        {
            // Remove connection in node system
            gc.getNodeSystem().RemoveLine(toRemove.line.GetComponent<connection_script>().Line);

            m_connections.Remove(toRemove);
            Destroy(toRemove.line);
        }
    }

    public void RemoveConnection(NodeSystem.Line line)
    {
        // Cant set to null, will never be used of 'found' is false
        // anyway
        bool found = false;
        Connection toRemove = (Connection)m_connections[0];

        // Find corresponding connection
        for (int i = 0; i < m_connections.Count; i++)
        {
            if (((Connection)m_connections[i]).line.GetComponent<connection_script>().Line == line)
            {
                toRemove = (Connection)m_connections[i];
                found = true;
                break;
            }
        }

        if (found)
        {
            // Remove connection in node system
            gc.getNodeSystem().RemoveLine(toRemove.line.GetComponent<connection_script>().Line);

            m_connections.Remove(toRemove);
            Destroy(toRemove.line);
        }
    }

    public ArrayList GetNeighbors(GameObject waypoint)
    {
        ArrayList neighbors = new ArrayList();
        for (int i = 0; i < m_connections.Count; i++)
        {
            if (((Connection)m_connections[i]).startNode == waypoint)
            {
                neighbors.Add(((Connection)m_connections[i]).endNode);
            }
            else if (((Connection)m_connections[i]).endNode == waypoint)
            {
                neighbors.Add(((Connection)m_connections[i]).startNode);
            }
        }

        return neighbors;
    }

    public GameObject GetWaypoint(NodeSystem.Node node)
    {
        for (int i = 0; i < m_waypoints.Count; i++)
        {
            if (((GameObject)m_waypoints[i]).GetComponent<waypoint_script>().Node == node)
            {
                return(GameObject)m_waypoints[i];
            }
        }
        return null;
    }

    public GameObject GetConnection(NodeSystem.Line line)
    {
        for (int i = 0; i < m_connections.Count; i++)
        {
            if (((GameObject)m_connections[i]).GetComponent<connection_script>().Line == line)
            {
                return ((Connection)m_connections[i]).line;
            }
        }
        return null;
    }
}

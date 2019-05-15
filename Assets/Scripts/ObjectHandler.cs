using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour
{
    public ArrayList m_waypoints = new ArrayList();
    public ArrayList m_connections = new ArrayList();

    //GameController gc

    // Adds
    public void AddWaypoint(GameObject waypoint)
    {
        m_waypoints.Add(waypoint);
    }

    public void AddConnection(GameObject connection)
    {
        m_connections.Add(connection);
    }

    // Removes
    public void RemoveWaypoint(GameObject waypoint)
    {
        m_waypoints.Remove(waypoint);
    }

    public void RemoveConnection(GameObject connection)
    {
        m_waypoints.Remove(connection);
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

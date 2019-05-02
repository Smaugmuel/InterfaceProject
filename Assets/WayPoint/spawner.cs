using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject waypoint_model;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 1000f, 9))
            {
                SpawnWaypoint(hit.point);
            }
        }
    }

    void SpawnWaypoint(Vector3 pos)
    {
        Instantiate(waypoint_model, pos + new Vector3(0f, 1f, 0f), Quaternion.identity);
    }
}

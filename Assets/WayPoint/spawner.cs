using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject waypoint_model;

    [SerializeField]
    public GameController gc;

    public Camera sideCamera;

    private Vector3 waypointPlaceOffset = new Vector3(0f, 1f, 0f);
    

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, 9);

            // If hit
            if (hits.Length > 0)
            {
                hits = sortHitList(hits);

                // If multiple
                if (hits.Length > 1)
                {
                    moveSideCamera(getAveragePos(hits));
                }

                for (int i = 0; i < hits.Length; i++)
                {
                    SpawnWaypoint(hits[i].point);
                }
            }
        }
    }

    void SpawnWaypoint(Vector3 pos)
    {
        pos += waypointPlaceOffset;
        Instantiate(waypoint_model, pos, Quaternion.identity);
        NodeSystem ns = gc.getNodeSystem();
        ns.AddNode(pos.x, pos.y, pos.z);
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

    Vector3 getAveragePos(RaycastHit[] hits)
    {
        Vector3 average = hits[0].point;

        for (int i = 1; i < hits.Length; i++)
        {
            average += hits[i].point;
        }
        average /= hits.Length;

        return average;
    }

    void moveSideCamera(Vector3 lookAt)
    {
        lookAt += waypointPlaceOffset;
        Vector3 dirFromOrigo = new Vector3(lookAt.x, 0f, lookAt.y).normalized * 6f;

        sideCamera.transform.position = lookAt + dirFromOrigo + new Vector3(0, 3f, 0f);
        sideCamera.transform.LookAt(lookAt);
    }
}

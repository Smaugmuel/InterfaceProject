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
        Instantiate(waypoint_model, pos + new Vector3(0f, 1f, 0f), Quaternion.identity);
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

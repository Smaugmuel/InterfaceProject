using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingCube : MonoBehaviour
{
    public List<NodeSystem.Node> path;
    int nextTarget = 1;
    public float speed = 1;

    public void setPathPos(int nodeIndex)
    {
        //print("SetPathPos");

        nextTarget = nodeIndex + 1;
        transform.position = new Vector3(path[nodeIndex].X, path[nodeIndex].Y, path[nodeIndex].Z);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null)
        {
            //print("Path is Null");
            return;
        }

        if (nextTarget >= path.Count)
        {
            //print("TP Cube");
            
            nextTarget = 1;
            transform.position = new Vector3(path[0].X, path[0].Y, path[0].Z);
        }

        Vector3 targetPos = new Vector3(path[nextTarget].X, path[nextTarget].Y, path[nextTarget].Z);
        //print(targetPos);
        transform.position += (targetPos-transform.position).normalized * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, targetPos) < 0.1)
            nextTarget++;
    }
}

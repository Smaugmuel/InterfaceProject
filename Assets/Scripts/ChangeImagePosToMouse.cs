using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeImagePosToMouse : MonoBehaviour
{
    public GameObject gameObjectB;
    Vector3 pos;
    Vector3 centerPos;
    Vector3 sideCamerPos;
    // Start is called before the first frame update
    void Start()
    {
        sideCamerPos = gameObjectB.transform.position;
        centerPos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {

        //transform.position = pos;
        //if (Input.GetMouseButtonDown(0))
        if (sideCamerPos != gameObjectB.transform.position)
        {
            transform.position = Input.mousePosition;
            transform.LookAt(centerPos);
            transform.Rotate(0, 90, 90 - 31);
            sideCamerPos = gameObjectB.transform.position;
        }


        //Input.mousePosition
    }

}

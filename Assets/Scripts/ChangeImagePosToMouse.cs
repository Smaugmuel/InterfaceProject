using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeImagePosToMouse : MonoBehaviour
{


    public Camera gameObjectB;
    public Camera mainCamera;
    Vector3 pos;
    Vector3 centerPos;
    Vector3 sideCamerPos;
    // Start is called before the first frame update
    void Start()
    {
        sideCamerPos = gameObjectB.transform.position;
        centerPos = new Vector3(Screen.width * 0.5f, Screen.height * 0.7f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = pos;
        //if (sideCamerPos != gameObjectB.transform.position)
        //{
            Vector3 camPos = mainCamera.WorldToScreenPoint(gameObjectB.transform.position);

            Vector3 target = mainCamera.WorldToScreenPoint(pickingHandler.sideCameraLookAt);
            Vector3 rotateDir = camPos - target;
            
            //Vector3 rotateDir = camPos - Input.mousePosition;

            float rotAngle = Mathf.Atan2(rotateDir.y, rotateDir.x) * Mathf.Rad2Deg;
            Quaternion newRot = Quaternion.AngleAxis(rotAngle + 90, Vector3.forward);

            transform.rotation = newRot;
            transform.position = camPos;//Input.mousePosition;

            sideCamerPos = gameObjectB.transform.position;
        //}


        //Input.mousePosition
    }

}

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
        //if (Input.GetMouseButtonDown(0))
        if (sideCamerPos != gameObjectB.transform.position)
        {
            //transform.position = Input.mousePosition;

            // cam pos = cam UI pos
            // mouse pos

            // 
            //transform.position;
            Vector3 camPos = mainCamera.WorldToScreenPoint(gameObjectB.transform.position);


            //Vector3 rotateDir = Input.mousePosition - transform.position;
            Vector3 rotateDir = camPos - Input.mousePosition;
            //Vector3 rotateDir = Input.mousePosition - camPos;
            //Vector3 rotateDir = mainCamera.WorldToScreenPoint(Input.mousePosition) - camPos;
            //Vector3 rotateDir = mainCamera.WorldToScreenPoint(Input.mousePosition) - transform.position;
            //Vector3 rotateDir = (Input.mousePosition) - transform.position;
            //rotateDir = rotateDir.normalized;

            float rotAngle = Mathf.Atan2(rotateDir.y, rotateDir.x) * Mathf.Rad2Deg;
            Quaternion newRot = Quaternion.AngleAxis(rotAngle + 90, Vector3.forward);

            //float newRot = Quaternion.Angle(gameObjectB.transform.rotation, transform.r);

            //transform.rotation = Quaternion.LookRotation(Input.mousePosition, Vector3.up);
            //transform.rotation = Quaternion.LookRotation(rotateDir, Vector3.up);
            //transform.Rotate(0, 90, 45);

            //transform.position = camPos;
            transform.rotation = newRot;
            transform.position = Input.mousePosition;





            //Quaternion temp = new Vector3(0, 0, gameObjectB.transform.rotation.eulerAngles.y);
            //transform.rotation.eulerAngles = temp;
            //pickingHandler.cameraLookAt;
            //transform.LookAt(pickingHandler.cameraLookAt);
            //transform.LookAt(centerPos);
            //transform.Rotate(0, 90,  90 + Mathf.Asin(rotateDir.x));
            sideCamerPos = gameObjectB.transform.position;
        }


        //Input.mousePosition
    }

}

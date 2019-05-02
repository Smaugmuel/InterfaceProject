using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 mousePos;
    private Vector3 cameraPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ManageMovement();
        ManageZoom();
    }

    void ManageMovement()
    {
        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
            {
                mousePos = Input.mousePosition;
                cameraPos = Camera.main.GetComponent<Transform>().position;
            }

            Vector3 movement = Input.mousePosition - mousePos;

            // Turn y-movement into z-movement
            movement.z = movement.y;
            movement.y = 0.0f;

            // Flip movement controls and decrease movement speed
            movement *= -0.05f;

            Camera.main.transform.position = cameraPos + movement;
        }
    }
    void ManageZoom()
    {
        float scrolled = Input.mouseScrollDelta.y;
        if (scrolled != 0.0f)
        {
            float orthoSize = Camera.main.orthographicSize;
            orthoSize -= scrolled;

            if (orthoSize < 1.0f)   orthoSize = 1.0f;
            if (orthoSize > 16.0f)  orthoSize = 16.0f;
            //Mathf.Clamp(orthoSize, 1.0f, 16.0f);      // Did not work

            Camera.main.orthographicSize = orthoSize;


            // Zoom based on mouse pointer location

            //Camera.main.transform.position;




            float screenWidth = Screen.width;
            float screenHeight = Screen.height;





            Debug.Log(screenWidth.ToString() + "x" + screenHeight.ToString());
        }
    }
}

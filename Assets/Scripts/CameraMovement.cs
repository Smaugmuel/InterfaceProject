using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 mousePosPrev;
    private Vector3 cameraPosPrev;

    public float orthoMin = 1.0f;
    public float orthoMax = 16.0f;
    public float movementSpeed = 0.05f;
    public float zoomMovementSpeed = 3.0f;

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
                mousePosPrev = Input.mousePosition;
                cameraPosPrev = Camera.main.GetComponent<Transform>().position;
            }

            Vector3 movement = Input.mousePosition - mousePosPrev;

            // Turn y-movement into z-movement
            movement.z = movement.y;
            movement.y = 0.0f;

            // Flip movement controls and apply speed
            movement *= -movementSpeed;

            movement *= (Camera.main.orthographicSize / (orthoMax - orthoMin));

            Camera.main.transform.position = cameraPosPrev + movement;
        }
    }
    void ManageZoom()
    {
        float scrolled = Input.mouseScrollDelta.y;

        if (scrolled != 0.0f)
        {
            Vector3 mousePos = Input.mousePosition;
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            int halfScreenWidth = screenWidth / 2;
            int halfScreenHeight = screenHeight / 2;

            // Only zoom when within the editor window
            if (mousePos.x >= 0 && mousePos.x <= screenWidth &&
                mousePos.y >= 0 && mousePos.y <= screenHeight)
            {
                // Zoom
                float orthoSize = Camera.main.orthographicSize;
                orthoSize -= scrolled;

                // Zoom limits
                if (orthoSize < orthoMin) orthoSize = orthoMin;
                if (orthoSize > orthoMax) orthoSize = orthoMax;
                //Mathf.Clamp(orthoSize, 1.0f, 16.0f);      // <-- Did not work

                // Set zoom value
                Camera.main.orthographicSize = orthoSize;


                // Move camera based on mouse pointer location

                // Poor results + does not work when not aiming on an object
                {
                    /*Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 9))
                    {
                        Vector3 direction = hit.point - Camera.main.transform.position;
                        direction.y = 0.0f;

                        // Flip movement
                        direction *= -1;

                        Camera.main.transform.position += direction;
                        Debug.Log(direction.ToString());
                    }*/
                }
                
                // Set mouse position origin on center of the screen
                mousePos.x -= halfScreenWidth;
                mousePos.y -= halfScreenHeight;

                // Calculate percentaged distances from cursor to screen edge
                float posPercentageX = (float)mousePos.x / halfScreenWidth;
                float posPercentageY = (float)mousePos.y / halfScreenHeight;
                
                Vector3 movement = new Vector3(posPercentageX, 0.0f, posPercentageY);


                movement *= zoomMovementSpeed;

                movement *= (orthoSize / (orthoMax - orthoMin));

                // Flip movement when zooming out
                if (scrolled < 0) movement *= -1.0f;
                
                Camera.main.transform.position += movement;
                
                Debug.Log(
                    screenWidth.ToString() + "x" + screenHeight.ToString() +
                    ", (" + mousePos.x + ", " + mousePos.y + ")" +
                    "(" + posPercentageX.ToString() + ", " + posPercentageY.ToString() + ")"
                    );
            }
        }
    }
}

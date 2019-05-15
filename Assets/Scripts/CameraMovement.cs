using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera topDownCam;
    [SerializeField] private Camera sideCam;
    [SerializeField] private RectTransform sideCamViewRect;
    [SerializeField] private Canvas ui_canvas;
    [SerializeField] private float orthoMin = 1.0f;
    [SerializeField] private float orthoMax = 16.0f;
    [SerializeField] private float topDownMovementSpeed = 0.05f;
    [SerializeField] private float topDownZoomMovementSpeed = 3.0f;


    private enum CameraLocation { TOP_DOWN, SIDE, NONE };

    private CameraLocation currentCamType;

    private Vector2 mousePosBegin;
    private Vector3 camPosBegin;

    

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
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                
                // Determine where the cursor is when MMB is pressed
                if (IsScreenCoordWithinSceneView(mousePos))
                {
                    currentCamType = CameraLocation.TOP_DOWN;
                    camPosBegin = topDownCam.transform.position;
                }
                else if (IsScreenCoordWithinSideCamView(mousePos))
                {
                    currentCamType = CameraLocation.SIDE;
                    camPosBegin = sideCam.transform.position;
                }
                else
                {
                    currentCamType = CameraLocation.NONE;
                }

                mousePosBegin = mousePos;
            }

            // Calculate mouse movement while holding down MMB
            Vector2 mouseMovement = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mouseMovement -= mousePosBegin;

            // Take action based on where the cursor was and how much it has moved 
            if (currentCamType == CameraLocation.TOP_DOWN)
            {
                // Turn y-movement on the screen into z-movement in the scene
                Vector3 camMovement = new Vector3(mouseMovement.x, 0.0f, mouseMovement.y);
                
                // Scale movement speed based on how zoomed the camera is
                float scaledMovementSpeed = topDownMovementSpeed * (topDownCam.orthographicSize / (orthoMax - orthoMin));

                // Flip movement and apply the speed
                camMovement *= -scaledMovementSpeed;

                topDownCam.transform.position = camPosBegin + camMovement;
            }
            else if (currentCamType == CameraLocation.SIDE)
            {
                Vector3 toTarget = pickingHandler.sideCameraLookAt - camPosBegin;

                float angle = mouseMovement.x * 0.05f;
                
                Vector3 toTargetNew = new Vector3();
                toTargetNew.x = toTarget.x *  Mathf.Cos(angle) + toTarget.z * Mathf.Sin(angle);
                toTargetNew.y = toTarget.y;
                toTargetNew.z = toTarget.x * -Mathf.Sin(angle) + toTarget.z * Mathf.Cos(angle);

                sideCam.transform.position = camPosBegin + toTarget - toTargetNew;
                sideCam.transform.LookAt(pickingHandler.sideCameraLookAt);
            }
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

            // Determine where the cursor is when scrolling
            if (IsScreenCoordWithinSceneView(mousePos))
            {
                // Zoom
                float orthoSize = topDownCam.orthographicSize;
                orthoSize -= scrolled;

                // Zoom limits
                if (orthoSize < orthoMin) orthoSize = orthoMin;
                if (orthoSize > orthoMax) orthoSize = orthoMax;
                //Mathf.Clamp(orthoSize, 1.0f, 16.0f);      // <-- Did not work

                // Set zoom value
                topDownCam.orthographicSize = orthoSize;


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


                movement *= topDownZoomMovementSpeed;

                movement *= (orthoSize / (orthoMax - orthoMin));

                // Flip movement when zooming out
                if (scrolled < 0) movement *= -1.0f;

                // Stop movement when max or min zoomed
                if (orthoSize == orthoMin || orthoSize == orthoMax) movement *= 0.0f;
                topDownCam.transform.position += movement;
            }
        }
    }

    float SidePanelLeftEdge()
    {
        float sideViewWidth = sideCamViewRect.rect.width * ui_canvas.scaleFactor;
        
        return (sideCamViewRect.position.x - sideViewWidth / 2f);
    }
    bool IsScreenCoordWithinWindow(Vector2 coord)
    {
        return (coord.x >= 0 && coord.x <= Screen.width && coord.y >= 0 && coord.y <= Screen.height);
    }
    bool IsScreenCoordWithinSidePanel(Vector2 coord)
    {
        return (
            IsScreenCoordWithinWindow(coord) &&
            coord.x >= SidePanelLeftEdge()
            );
    }
    bool IsScreenCoordWithinSceneView(Vector2 coord)
    {
        return (
            IsScreenCoordWithinWindow(coord) &&
            coord.x < SidePanelLeftEdge()
            );
    }
    bool IsScreenCoordWithinSideCamView(Vector2 coord)
    {
        Vector2 sideViewSize = new Vector2(sideCamViewRect.rect.width, sideCamViewRect.rect.height);
        sideViewSize *= ui_canvas.scaleFactor;

        Vector2 sideViewPosition = new Vector2(sideCamViewRect.position.x, sideCamViewRect.position.y);

        Vector2 bottomLeft = sideViewPosition - sideViewSize / 2f;
        Vector2 topRight = sideViewPosition + sideViewSize / 2f;

        return (coord.x >= bottomLeft.x && coord.x <= topRight.x && coord.y >= bottomLeft.y && coord.y <= topRight.y);
    }
    bool IsScreenCoordWithinListView(Vector2 coord)
    {
        return (
            IsScreenCoordWithinSidePanel(coord) &&
            !IsScreenCoordWithinSideCamView(coord)
            );
    }
}

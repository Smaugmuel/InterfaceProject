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
        // Manage camera movement
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

        // Manage zoom
        float scrolled = Input.mouseScrollDelta.y;
        if (scrolled != 0.0f)
        {
            float orthoSize = Camera.main.orthographicSize;
            orthoSize -= scrolled;

            if (orthoSize < 1.0f)
            {
                orthoSize = 1.0f;
            }
            if (orthoSize > 16.0f)
            {
                orthoSize = 16.0f;
            }
            //Mathf.Clamp(orthoSize, 1.0f, 16.0f);
            Camera.main.orthographicSize = orthoSize;
        }
    }
}

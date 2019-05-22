using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showHotkey : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector2(-200, -200);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.mousePosition.x < 100 && Input.mousePosition.y > Screen.height - 100)
        {
            gameObject.transform.position = new Vector2(120, Screen.height - 100);
        }
        else
        {
            gameObject.transform.position = new Vector2(-200, -200);
        }
    }
}

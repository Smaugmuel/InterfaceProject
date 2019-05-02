using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{



    public void func()
    {
        print("hej");
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            NodeSystem.Test();
            print("Node System Test: OK");
        }
        catch
        {
            print("Node System Test: Failed");

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

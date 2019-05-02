using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    NodeSystem ns = new NodeSystem();

    float nextUpdate = 1;

    [SerializeField]
    UI_nodePanel ui_nodePanel;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            NodeSystem.Test();
            //print("Node System Test: OK");
        }
        catch
        {
            print("Node System Test: Failed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //nextUpdate -= Time.deltaTime;
        //if(nextUpdate < 0)
        //{
        //    nextUpdate = 1;
        //    ns.AddNode(new NodeSystem.Node(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100)));
        //    ui_nodePanel.UpdateUI();
        //}
    }

    public NodeSystem getNodeSystem()
    {
        return ns;
    }
}



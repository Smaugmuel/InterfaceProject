using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    NodeSystem ns = new NodeSystem();

    float nextUpdate = 1;

    [SerializeField]
    UI_nodePanel ui_nodePanel;

    [SerializeField]
    GameObject[] worldObjects;

    [SerializeField]
    GameObject loadButtoin;

    public void ShowWorld()
    {
        foreach(GameObject obj in worldObjects)
        {
            obj.SetActive(true);
        }

        GUIHandler.Instance.SetActive(0, true);
        GUIHandler.Instance.SetActive(1, true);
        GUIHandler.Instance.SetActive(2, true);

        loadButtoin.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in worldObjects)
        {
            obj.SetActive(false);
        }

        GUIHandler.Instance.SetActive(0,false);
        GUIHandler.Instance.SetActive(1,false);
        GUIHandler.Instance.SetActive(2,false);

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



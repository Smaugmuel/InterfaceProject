using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    NodeSystem ns = new NodeSystem();

    float nextUpdate = 1;

    //[SerializeField]
    //UI_nodePanel ui_nodePanel;
    //[SerializeField]
    //UI_pathPanel ui_pathPanel;

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

        GUIHandler.Instance.SetActive(0, true);    // Node panel
        GUIHandler.Instance.SetActive(1, false);   // Path panel
        GUIHandler.Instance.SetActive(2, false);   // Line panel
        GUIHandler.Instance.SetActive(3, true);    // Side camera
        GUIHandler.Instance.SetActive(4, true);    // Frustum triangle

        loadButtoin.SetActive(false);
    }

    public void changeCurrentConnectionType(int type)
    {
        pickingHandler.currentConnectionType = type;
    }

    // Start is called before the first frame update
    void Start()
    {
        GUIHandler.Instance.SetActive(0, false);    // Node panel
        GUIHandler.Instance.SetActive(1, false);    // Path panel
        GUIHandler.Instance.SetActive(2, false);    // Line panel
        GUIHandler.Instance.SetActive(3, false);    // Side camera
        GUIHandler.Instance.SetActive(4, false);    // Frustum triangle

        foreach (GameObject obj in worldObjects)
        {
            obj.SetActive(false);
        }

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



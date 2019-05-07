using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_nodeItem : MonoBehaviour
{
    NodeSystem.Node node;

    [SerializeField]
    Text IDText;
    [SerializeField]
    Text CoordText;

    [SerializeField]
    spawner spawnhandler;

    int id = 0;

    public void Clicked()
    {
        spawnhandler.SetSelectedNode(node);
    }

    public NodeSystem.Node Node
    {
        get { return node; }
        set { node = value; }
    }

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public void UpdateUI()
    {
        IDText.text = "ID: " + id;
        if (node != null)
        {
            CoordText.text = "x: " + node.X + ", y: " + node.Y + ", z: " + node.Z;
        }
        else
        {
            CoordText.text = "no assigned node";
        }
    }

    float nextUpdate = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnhandler = GameObject.FindGameObjectWithTag("spawner").GetComponent<spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        nextUpdate -= Time.deltaTime;
        if(nextUpdate <= 0)
        {
            UpdateUI();
            nextUpdate = 2;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_nodePanel : MonoBehaviour
{
    NodeSystem ns;

    [SerializeField]
    GameObject nodeItem_Prefab;
    [SerializeField]
    GameController gameController;
    [SerializeField]
    GameObject content;
    [SerializeField]
    ScrollRect sc = null;

    float nextUpdate = 0;

    public void UpdateUI()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ui_node_item");
        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }

        for (int i = 0; i < ns.Nodes.Count; i++)
        {
            GameObject item = Instantiate(nodeItem_Prefab, content.transform);
            Vector3 itemPos = new Vector3(0.0f, i * -25.0f -15, 0.0f);

            item.transform.localPosition = itemPos;

            item.GetComponent<UI_nodeItem>().ID = i;
            item.GetComponent<UI_nodeItem>().Node = (NodeSystem.Node)ns.Nodes[i];
            item.GetComponent<UI_nodeItem>().UpdateUI();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        ns = gameController.getNodeSystem();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        nextUpdate -= Time.deltaTime;
        if (nextUpdate <= 0)
        {
            UpdateUI();
            nextUpdate = 5;
        }
    }
}

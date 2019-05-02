using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_nodePanel : MonoBehaviour
{
    NodeSystem ns;

    [SerializeField]
    GameObject nodeItem_Prefab;

    public void UpdateUI()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ui_node_item");
        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }

        for (int i = 0; i < ns.Nodes.Count; i++)
        {
            Instantiate(nodeItem_Prefab, transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

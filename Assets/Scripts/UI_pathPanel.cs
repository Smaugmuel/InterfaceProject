using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_pathPanel : MonoBehaviour
{
    PathFinding pathFinding;

    [SerializeField]
    GameObject pathItem_prefab;
    [SerializeField]
    GameController gameController;
    [SerializeField]
    GameObject content;
    [SerializeField]
    ScrollRect sc = null;

    float nextUpdate = 0;

    public void UpdateUI()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ui_path_item");
        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }

        List<PathFinding.CheckpointedPath> paths = pathFinding.GetPaths();

        for (int i = 0; i < paths.Count; i++)
        {
            GameObject item = Instantiate(pathItem_prefab, content.transform);
            Vector3 itemPos = new Vector3(0.0f, i * -25.0f - 15, 0.0f);

            item.transform.localPosition = itemPos;

            item.GetComponent<UI_pathItem>().ID = i;
            item.GetComponent<UI_pathItem>().Path = paths[i];
            item.GetComponent<UI_pathItem>().UpdateUI();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pathFinding = gameController.GetComponent<PathFinding>();
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

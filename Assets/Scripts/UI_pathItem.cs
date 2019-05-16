using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_pathItem : MonoBehaviour
{
    PathFinding.CheckpointedPath path;
    int id = 0;

    [SerializeField]
    Text IDText;

    PathFinding pathFinding;
    float nextUpdate = 0;

    public PathFinding.CheckpointedPath Path
    {
        get { return path; }
        set { path = value; }
    }
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public void Clicked()
    {
        pathFinding.SelectPath(path);
    }

    public void UpdateUI()
    {
        IDText.text = "ID: " + id;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathFinding = FindObjectOfType<PathFinding>();
    }

    // Update is called once per frame
    void Update()
    {
        nextUpdate -= Time.deltaTime;
        if (nextUpdate <= 0)
        {
            UpdateUI();
            nextUpdate = 2;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateManager : MonoBehaviour
{
    [SerializeField]
    private List<string> states;
    private int initialStateIndex = 0;

    private int currentStateIndex;

    // Singleton
    public static StateManager Instance { get; set; }
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentStateIndex = initialStateIndex;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public string CurrentState()
    {
        return states[currentStateIndex];
    }
    public int NameToIndex(string stateName)
    {
        int index = -1;
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i] == stateName)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    public string IndexToName(int index)
    {
        string name = "";
        if (index >= 0 && index < states.Count)
        {
            name = states[index];
        }
        return name;
    }
    public bool SetState(int index)
    {
        bool switchedState = false;
        if (index >= 0 && index < states.Count)
        {
            currentStateIndex = index;
            switchedState = true;

            Debug.Log("Switched to " + states[currentStateIndex] + " state");
        }
        return switchedState;
    }
    public bool SetState(string stateName)
    {
        return SetState(NameToIndex(stateName));
    }
}

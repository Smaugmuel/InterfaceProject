using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public RectTransform radialMenuElements;

    /*
     * NOTE
     * Every section on the Radial menu will become a state
     * Whether or not a programmer switches to them is their responsibility
     * */

    private int nStates;
    private string[] stateNames;
    private int currentStateIndex;

    // Start is called before the first frame update
    void Start()
    {
        nStates = radialMenuElements.transform.childCount;
        stateNames = new string[nStates];

        for (int i = 0; i < nStates; i++)
        {
            // Retrieve the text object
            UnityEngine.UI.Text text = radialMenuElements.GetChild(i).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();

            stateNames[i] = text.text;
        }

        // Initial state index is 0 unless that is the "Adv" section
        currentStateIndex = (stateNames[0] == "Adv" ? 1 : 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetStateIndex(string stateName)
    {
        int index = -1;
        for (int i = 0; i < nStates; i++)
        {
            if (stateNames[i] == stateName)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public string GetStateName(int index)
    {
        string name = "";
        if (index >= 0 && index < nStates)
        {
            name = stateNames[index];
        }
        return name;
    }

    public bool SetState(int index)
    {
        bool switchedState = false;
        if (index >= 0 && index < nStates)
        {
            currentStateIndex = index;
            switchedState = true;
        }
        return switchedState;
    }

    public bool SetState(string stateName)
    {
        return SetState(GetStateIndex(stateName));
    }
}

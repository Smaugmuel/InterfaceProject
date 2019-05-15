using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOptions : MonoBehaviour
{
    [SerializeField] private Button m_button1,m_button2,m_button3,m_button4;

    // Start is called before the first frame update
    void Start()
    {
        m_button1.onClick.AddListener(ClickButtonOne);
        m_button2.onClick.AddListener(ClickButtonTwo);
        m_button3.onClick.AddListener(ClickButtonThree);
        m_button4.onClick.AddListener(ClickButtonFour);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ClickButtonOne()
    {
        StateManager.Instance.SetState("Node");
        //Debug.Log("Clicked button 1");
    }
    void ClickButtonTwo()
    {
        StateManager.Instance.SetState("Path");
        //Debug.Log("Clicked button 2");
    }
    void ClickButtonThree()
    {
        //Debug.Log("Clicked button 3");
    }
    void ClickButtonFour()
    {
        StateManager.Instance.SetState("Line");
        //Debug.Log("Clicked button 4");
    }
}

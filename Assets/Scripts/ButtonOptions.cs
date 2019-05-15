using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOptions : MonoBehaviour
{
    public Button m_button1,m_button2,m_button3,m_button4;
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
        Debug.Log("Clicked button 1");
    }
    void ClickButtonTwo()
    {
        Debug.Log("Clicked button 2");
    }
    void ClickButtonThree()
    {
        Debug.Log("Clicked button 3");
    }
    void ClickButtonFour()
    {
        Debug.Log("Clicked button 4");
    }
}

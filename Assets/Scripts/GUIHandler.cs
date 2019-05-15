using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHandler : MonoBehaviour
{
    [System.Serializable]
    public class RenderItem
    {
        public GameObject item;
        public bool isActive;
    }

    [SerializeField] private List<RenderItem> guiItems;

    // Singleton
    public static GUIHandler Instance { get; set; }
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
        
    }

    void OnValidate()
    {
        for (int i = 0; i < guiItems.Count; i++)
        {
            guiItems[i].item.SetActive(guiItems[i].isActive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public bool SetActive(int index, bool isActive)
    {
        bool wasSuccessful = false;
        if (index >= 0 && index < guiItems.Count)
        {
            guiItems[index].isActive = isActive;
            guiItems[index].item.SetActive(isActive);
            wasSuccessful = true;
        }
        return wasSuccessful;
    }

    public bool Toggle(int index)
    {
        bool wasSuccessful = false;
        if (index >= 0 && index < guiItems.Count)
        {
            guiItems[index].isActive = !guiItems[index].isActive;
            guiItems[index].item.SetActive(guiItems[index].isActive);
            wasSuccessful = true;
        }
        return wasSuccessful;
    }

    // Beware, this does not return information about whether the index was valid
    public bool IsActive(int index)
    {
        bool isActive = false;
        if (index >= 0 && index < guiItems.Count)
        {
            isActive = guiItems[index].isActive;
        }
        return isActive;
    }
}

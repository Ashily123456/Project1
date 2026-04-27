using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // making this a singleton
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // create a list holding all the hidden objects
    public Transform hiddenObjectsParent;
    private List<GameObject> hiddenObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        // loading all the hidden objects into the list
        foreach (Transform child in hiddenObjectsParent)
        {
            hiddenObjects.Add(child.gameObject);
            
            // turn off the sprite renderer and collider 
            child.GetComponent<SpriteRenderer>().enabled = false;
            child.GetComponent<Collider>().enabled = false;
            
            // debugging
            //Debug.Log("Added " + child.gameObject.name + " to hidden objects list");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropNextItem()
    {
        if (hiddenObjects.Count == 0)
        {
            // if there's no items left
            Debug.Log("BAD ENDING");
            return; // exit the function
        }
        
        // get the next item from the list
        int randomIndex = UnityEngine.Random.Range(0, hiddenObjects.Count);
        GameObject nextItem = hiddenObjects[randomIndex];
        
        // remove this item from the list so it won't be dropped again
        hiddenObjects.RemoveAt(randomIndex);
        
        // turn on the sprite renderer and collider to make it visible and interactable
        nextItem.GetComponent<SpriteRenderer>().enabled = true;
        nextItem.GetComponent<Collider>().enabled = true;
        
        // debugging
        Debug.Log("Dropped " + nextItem.name);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool hasBeenInteractedWith = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && 
            !hasBeenInteractedWith)
        {
            LevelManager.instance.DropNextItem();
            
            // prevent repeated interactions
            hasBeenInteractedWith = true;
        }
    }
    
    
}

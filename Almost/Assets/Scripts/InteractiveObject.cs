using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool hasBeenInteractedWith = false;
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        indicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

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
            // turn off the indicator of self
            HideIndicator();
            
            LevelManager.instance.DropNextItem();
            
            // prevent repeated interactions
            hasBeenInteractedWith = true;
        }
    }
    
    public void HideIndicator()
    {
        if (indicator != null)
        {
            indicator.enabled = false;
            
            if (animator != null)
            {
                animator.enabled = false; 
            }
        }
        else
        {
            Debug.LogWarning("Indicator not found for " + gameObject.name);
        }
    }

    public void ShowIndicator()
    {
        if (indicator != null)
        {
            indicator.enabled = true;
            
            if (animator != null)
            {
                animator.enabled = true; 
                animator.Play("Spark");
            }
        }
        else
        {
            Debug.LogWarning("Indicator not found for " + gameObject.name);
        }
        
        // activate the animation
        animator.Play("Spark");
    }
}

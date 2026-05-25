using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool hasBeenInteractedWith = false;
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Animator animator;
    
    // the list holding all the audio clips played when the objects dropped
    public AudioSource audioSource;
    public List<AudioClip> droppingAudioClips;
    public AudioClip droppingAudioClip;

    private void Awake()
    {
        indicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // loading all dropping audio clips from the Resources folder
        droppingAudioClips = new List<AudioClip>(Resources.LoadAll<AudioClip>("Audio"));
        
        // find the audio clip with the name of current gameobject
        foreach (AudioClip clip in droppingAudioClips)
        {
            if (clip.name == gameObject.name)
            {
                droppingAudioClip = clip;
                break;
            }
        }
        
        // add the audio source component to the game object
        audioSource = new AudioSource();
        audioSource.clip = droppingAudioClip;
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

    public void PlayDroppingAudio()
    {
        if (audioSource != null && droppingAudioClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Audio source or dropping audio clip not found for " + gameObject.name);
        }
    }
}

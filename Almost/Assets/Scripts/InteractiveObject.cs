using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Drinks,
    Coffee,
    Temperature,
    Planning,
    Distraction,
    Technology,
    Curiosity,
    Desk
}

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool hasBeenInteractedWith = false;
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Animator animator;
    
    // the list holding all the audio clips played when the objects dropped
    public AudioSource audioSource;
    public List<AudioClip> droppingAudioClips;
    public AudioClip droppingAudioClip;
    
    // categorize the item objects into different types
    public ItemType itemType;
    private string[] foodKeys = {"oven", "chips", "cookie"};
    private string[] drinkKeys = {"fridge", "vending machine"};
    private string[] coffeeKeys = { "coffee", "coffee desk" };
    private string[] planningKeys = {"calendar", "list", "document", "pencil", "pencil sharpener", "sand"};
    private string[] tempKeys = {"thermometer", "light"};
    private string[] distractionKeys = {"phone", "message", "call", "news"};
    private string[] techKeys = {"main unit", "computer", "battery", "glasses", "airpods", "typing", "zip", "loading"};
    private string[] curiosityKeys = {"box", "?box", "key", "enter"};
    
    private void Awake()
    {
        indicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // categorize the item objects into different types
        if (IsItemInCategory(gameObject.name.ToLower(), foodKeys))
        {
            itemType = ItemType.Food;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), drinkKeys))
        {
            itemType = ItemType.Drinks;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), coffeeKeys))
        {
            itemType = ItemType.Coffee;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), planningKeys))
        {
            itemType = ItemType.Planning;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), tempKeys))
        {
            itemType = ItemType.Temperature;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), distractionKeys))
        {
            itemType = ItemType.Distraction;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), techKeys))
        {
            itemType = ItemType.Technology;
        }
        else if (IsItemInCategory(gameObject.name.ToLower(), curiosityKeys))
        {
            itemType = ItemType.Curiosity;
        }
        else if (gameObject.name.ToLower() == "desk") // special: first item -- desk
        {
            itemType = ItemType.Desk;
        }
        
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
        
        // if no matching audio clip is found, assign a default sound effect
        if (droppingAudioClip == null)
        {
            // assign the default sound effect
            droppingAudioClip = Resources.Load<AudioClip>("Audio/default");
        }
        
        // add the audio source component to the game object
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // mute
        
        // implement the sound effects
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
            
            LevelManager.instance.DropNextItem(itemType);
            
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

    private bool IsItemInCategory(string category, string[] keywordArray)
    {
        foreach (var keyword in keywordArray)
        {
            if (category.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }
}

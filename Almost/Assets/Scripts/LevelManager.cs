using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
            
            Debug.LogWarning("Multiple instances of LevelManager detected!");
        }
    }

    // create a list holding all the hidden objects
    public Transform hiddenObjectsParent;
    private List<GameObject> hiddenObjects = new List<GameObject>();
    
    // dropping animation parameters
    public AudioSource audioSource;
    public AudioClip holySoundEffectClip;
    public AudioClip dropSoundEffectClip;
    
    public float hitStopDuration = 0.2f;
    public float delayBeforeDrop = 1.25f;
    
    public float dropDuration = 0.5f; // duration of the drop animation
    
    // intro animation
    [SerializeField] 
    private bool playIntroAnimation = true;
    public GameObject introCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        // loading all the hidden objects into the list
        foreach (Transform child in hiddenObjectsParent)
        {
            // add interactive script to the child objects
            if (child.GetComponent<InteractiveObject>() == null)
            {
                InteractiveObject interaction = child.gameObject.AddComponent<InteractiveObject>();
                interaction.HideIndicator(); // hide the indicator at the start of the game
            }
            
            hiddenObjects.Add(child.gameObject);
            
            // turn off the sprite renderer and collider 
            child.GetComponent<SpriteRenderer>().enabled = false;
            child.GetComponent<Collider>().enabled = false;
            
            // debugging
            //Debug.Log("Added " + child.gameObject.name + " to hidden objects list");
            
            // intro animation
            introCanvas = GameObject.Find("IntroCanvas");
        }
        
        // initialize
        GameManager.instance.gameStarted = false;
        
        // play the intro animation if the flag is set to true
        if (playIntroAnimation && introCanvas != null)
        {
            // find avatar on the fist child
            introCanvas.transform.GetChild(0).GetComponent<Animator>().Play("IntroStoryBoard");
        }
        else
        {
            Debug.LogWarning("Intro canvas not found or playIntroAnimation is set to false. Skipping intro animation.");
            introCanvas.SetActive(false);
            GameManager.instance.gameStarted = true; // start the game immediately if no intro animation
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropNextItem()
    {
        if (hiddenObjects.Count == 0) //TODO : SHOULD BE 1, TO TRIGGER THE BAD ENDING
        {
            // if there's no items left
            Debug.Log("BAD ENDING!! DIE! DIE! DIE!!!");
            return; // exit the function
        }
        
        // two containers 
        List<GameObject> outsideCameraObjects = new List<GameObject>();
        List<GameObject> insideCameraObjects = new List<GameObject>();
        
        // setup camera
        Camera mainCamera = Camera.main;

        foreach (var obj in hiddenObjects)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(obj.transform.position);
            
            // check if the object is inside or outside the camera view
            bool isInsideCamera = viewportPos.x > 0 && 
                                  viewportPos.x < 1 && 
                                  viewportPos.y > 0 && 
                                  viewportPos.y < 1;

            if (isInsideCamera)
            {
                insideCameraObjects.Add(obj);
            }
            else
            {
                outsideCameraObjects.Add(obj);
            }
        }

        GameObject itemToDrop = null;
        
        // if there are items outside the camera, drop one of those
        if (outsideCameraObjects.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, outsideCameraObjects.Count);
            itemToDrop = outsideCameraObjects[randomIndex];
        }
        else // otherwise, drop one from inside the camera
        {
            int randomIndex = UnityEngine.Random.Range(0, insideCameraObjects.Count);
            itemToDrop = insideCameraObjects[randomIndex];
            Debug.Log("NOWHERE TO HIDE!!");
        }
        
        // remove this item from the list so it won't be dropped again
        hiddenObjects.Remove(itemToDrop);
        
        // start the drop animation
        StartCoroutine(DropAnimation(itemToDrop, dropDuration));
    }

    private IEnumerator DropAnimation(GameObject item, float duration)
    {
        // record the origin position
        Vector3 targetPosition = item.transform.position;
        
        // higher position
        Vector3 startPosition = targetPosition + new Vector3(0, 5f, 0);
        item.transform.position = startPosition;
        
        // step 0: hit-stop
        yield return new WaitForSeconds(hitStopDuration);
        
        // step 1: play holy sound effect
        Debug.Log("Playing holy sound effect and showing the holy light...");
        if (audioSource != null)
        {
            audioSource.PlayOneShot(holySoundEffectClip);
        }
        
        // step 2: hit-stop but 2
        yield return new WaitForSeconds(hitStopDuration);
        
        // actual drop
        // show the item sprite
        item.GetComponent<SpriteRenderer>().enabled = true;
        
        // droooooooppping
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            item.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            
            elapsedTime += Time.deltaTime;
            
            yield return null; // wait for the next frame
        }
        
        // ensure the item is exactly at the target position at the end of the animation
        item.transform.position = targetPosition;
        
        // screen shake effect
        float finalForce = 1f;

        if (item.CompareTag("Light"))
        {
            finalForce = 0.2f;
        }
        else if (item.CompareTag("Heavy"))
        {
            finalForce = 2.0f;
        }
        
        GetComponent<CinemachineImpulseSource>().GenerateImpulseWithForce(finalForce);
        
        // play drop sound effect
        item.GetComponent<InteractiveObject>().PlayDroppingAudio();
        
        // turn on the collider to make it interactable
        item.GetComponent<Collider>().enabled = true;
        
        // turn on the indicator from the item
        item.GetComponent<InteractiveObject>().ShowIndicator();
        
        // debugging
        Debug.Log("Dropped " + item.name);
    }

}

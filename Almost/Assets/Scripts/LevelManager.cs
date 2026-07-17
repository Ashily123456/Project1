using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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
    
    public float hitStopDuration = 0.2f;
    public float delayBeforeDrop = 1.25f;
    
    public float dropDuration = 0.5f; // duration of the drop animation
    
    // intro animation
    [SerializeField] 
    private bool playIntroAnimation = true;
    public GameObject introCanvas;
    
    // holy light effect
    public Image holyLightImage;
    public float fadeSpeed = 2f;
    private Coroutine fadeLightCoroutine;
    
    // particles playing along w. the holy light
    public VisualEffect statusVFX;
    
    // interaction dialogue box
    public int guaranteedDialogueCount = 3; // the total amount of dialogue boxes in the intro part
    public float dialogueProbability = 0.3f; // the probability of showing a dialogue box after dropping an item
    private int currentInteractionCount = 0;
    
    public GameObject dialogueCanvas;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    
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
            
            // holy-sound-effect
            audioSource = GetComponent<AudioSource>();
            holySoundEffectClip = Resources.Load<AudioClip>("Audio/holy-spell-cast");
            
            // debugging
            //Debug.Log("Added " + child.gameObject.name + " to hidden objects list");
        }
        
        // intro animation
        introCanvas = GameObject.Find("IntroCanvas");
        
        // holy light effect
        holyLightImage = GameObject.Find("HolyLight").GetComponent<Image>();
        
        // status VFX
        statusVFX = GameObject.Find("StatusVFX").GetComponent<VisualEffect>();
            
        // dialogue boxes
        dialogueCanvas = GameObject.Find("DialogueCanvas");
        dialogueBox = dialogueCanvas.transform.GetChild(0).gameObject;
        dialogueText = dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogueBox.SetActive(false);
        
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

    public void DropNextItem(ItemType currentItemType)
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
        StartCoroutine(DropAnimation(itemToDrop, dropDuration, currentItemType));
    }

    private IEnumerator DropAnimation(GameObject item, float duration, ItemType currentItemType)
    {
        // record the origin position
        Vector3 targetPosition = item.transform.position;
        
        // higher position
        Vector3 startPosition = targetPosition + new Vector3(0, 5f, 0);
        item.transform.position = startPosition;
        
        // step 0: hit-stop
        yield return new WaitForSeconds(hitStopDuration);
        
        // step 0.5: prepare the bool for playing effects
        // step 0.6: show dialogue box
        bool isDialogueShowing = ShowDialogue(currentItemType);

        if (isDialogueShowing)
        {
            // if the dialogue box is showing
            // step 1: play holy sound effect
            if (audioSource != null)
            {
                Debug.Log("Playing holy sound effect and showing the holy light...");
                audioSource.PlayOneShot(holySoundEffectClip);
            }
            
            // step 2: show holy effect
            PlayHolyLight();
            
            // step 3: show status VFX
            PlayStatusVFX(currentItemType);
        
            // step 3.5: time buffer for reading dialogues
            if (dialogueBox.activeSelf) // only wait if the dialogue box is active
            {
                yield return new WaitForSeconds(2.5f);
                dialogueBox.SetActive(false); // hide the dialogue box after the buffer time
            }
        }
        
        // step 4: hit-stop but 2
        yield return new WaitForSeconds(delayBeforeDrop);
        
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

    // dialogue boxes
    public bool ShowDialogue(ItemType type)
    {
        currentInteractionCount++;
        
        bool shouldShowDialogue = false;

        if (currentInteractionCount <= guaranteedDialogueCount)
        {
            shouldShowDialogue = true; // show guaranteed dialogues
        }
        else
        {
            // show dialogues based on probability
            float roll = UnityEngine.Random.value; // random value between 0 and 1
            
            if (roll < dialogueProbability)
            {
                shouldShowDialogue = true;
            }
        }

        // if the dialogue should not be shown, hide the dialogue box and return
        if (!shouldShowDialogue)
        {
            dialogueBox.SetActive(false);
            return false;
        }
        
        // otherwise, show the dialogue box with the appropriate text
        dialogueBox.SetActive(true);

        switch (type)
        {
            // first item -- desk
            case ItemType.Desk:
                dialogueText.text = "Alright, I'm at my desk. \n Today is the day I absolutely, positively finish \n all my work... probably.";
                break;
            
            // food, get energy
            case ItemType.Food:
                dialogueText.text = "Oops. Well, I couldn't possibly work on \n an empty stomach. \n Consider this... brain fuel!";
                break;
            
            // drinks, stay hydrated
            case ItemType.Drinks:
                dialogueText.text = "Ah, much better. \n A well-hydrated brain is a productive brain, right?";
                break;
            
            // coffee, wake-up
            case ItemType.Coffee:
                dialogueText.text = "I had to! \n Caffeine levels were critically low. \n Now I can finally focus.";
                break;
            
            // planning, get ready for the task
            case ItemType.Planning:
                dialogueText.text = "There. \n A perfectly sharpened pencil and a sorted list. \n I'm just being efficient!";
                break;
            
            // temperature, get warm... or cold
            case ItemType.Temperature:
                dialogueText.text = "Ah, perfect. \n I was just adjusting the environment for peak performance, that's all.";
                break;
            
            // distraction, what's happening
            case ItemType.Distraction:
                dialogueText.text = "What if it was an absolute emergency? \n Staying informed is just me being a responsible adult!";
                break;
            
            // technology, get techy
            case ItemType.Technology:
                dialogueText.text = "Finally connected! \n I was just... optimizing my digital workspace \n before starting.";
                break;
            
            // curiosity, what's inside the box?
            case ItemType.Curiosity:
                dialogueText.text = "Okay, I just HAD to know what that was. \n Curiosity is a sign of intelligence, anyway!";
                break;
            
            default:
                Debug.LogWarning("Unhandled item type for dialogue: " + type);
                break;
        }

        return true;
    }

    public void PlayHolyLight()
    {
        if(fadeLightCoroutine != null)
        {
            // if a fade coroutine is already running,
            // stop it before starting a new one
            StopCoroutine(fadeLightCoroutine);
        }
        
        fadeLightCoroutine = StartCoroutine(FadeHolyLight());
    }
    
    private IEnumerator FadeHolyLight()
    {
        // fade in
        while (holyLightImage.color.a < 0.2f) 
        {
            Color c = holyLightImage.color;
            c.a += Time.deltaTime * fadeSpeed;
            holyLightImage.color = c;
            yield return null; 
        }

        // fade out
        while (holyLightImage.color.a > 0f)
        {
            Color c = holyLightImage.color;
            c.a -= Time.deltaTime * fadeSpeed;
            holyLightImage.color = c;
            yield return null; 
        }
        
        // empty the coroutine reference after finishing the fade effect
        fadeLightCoroutine = null;
    }

    private void PlayStatusVFX(ItemType type)
    {
        if (statusVFX == null)
        {
            Debug.LogWarning("Status VFX is not assigned in the LevelManager.");
            return;
        }
        
        // position of the particles
        statusVFX.transform.position = GameObject.Find("Player").transform.position 
                                       + new Vector3(0, 0.75f, 0);

        // 0 = Fullness
        // 1 = Energy
        // 2 = Mood
        
        int vfxIndex = 2; // default VFX index : mood +10

        switch (type)
        {
            // food and drinks increase fullness
            case ItemType.Food:
            case ItemType.Drinks:
                vfxIndex = 0; 
                break;
            
            // energy
            case ItemType.Coffee:
            case ItemType.Desk:
            case ItemType.Technology:
            case ItemType.Planning:
                vfxIndex = 1;
                break;
            
            // mood
            case ItemType.Temperature:
            case ItemType.Curiosity:
            case ItemType.Distraction:
                vfxIndex = 2; 
                break;
        }
        
        statusVFX.SetInt("StatusTypeIndex", vfxIndex); 
        statusVFX.SendEvent("OnInteract");
    }
}

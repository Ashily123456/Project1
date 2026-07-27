using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 0.1f;

    private float moveX;
    private float moveY;
    
    public Animator animator; 
    public SpriteRenderer spriteRenderer;
    
    // melting
    public float meltTimeGap = 5f; 
    private float idleTimer = 0f;
    private bool isMelting = false;
    
    // WASD cue
    public GameObject wasdCueUI;
    private bool hasMovedOnce = false;
    private bool wasdCueShown = false;
    
    // squashing
    public VisualEffect squashEffect;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        squashEffect = GameObject.Find("SquashVFX").GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        // melting -- if already melting, skip the rest of the code
        if (isMelting || 
            GameManager.instance.gameStarted == false)
        {
            return;
        }
        
        moveX = 0f;
        moveY = 0f;
        
        // WASD ctrls
        if (Input.GetKey(KeyCode.W))
        {
            moveY = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }
        
        // turn around
        if (moveX > 0)
        {
            spriteRenderer.flipX = false; // face right
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true; // face left
        }
        
        // set the animation parameters
        bool isMoving = (moveX != 0f || moveY != 0f);
        animator.SetBool("isWalking", isMoving);
        
        // show WASD cue when game just started
        if (!wasdCueShown)
        {
            wasdCueShown = true;
            if (wasdCueUI != null)
            {
                wasdCueUI.SetActive(true);
            }
        }
        
        // melting
        if (isMoving)
        {
            if (!hasMovedOnce)
            {
                hasMovedOnce = true;
                if (wasdCueUI != null)
                {
                    wasdCueUI.SetActive(false); // hide the cue on first move
                }
            }
            
            // as long as the player moves, reset the idle timer
            idleTimer = 0;
        }
        else
        {
            // do not start melting countdown if the player hasn't moved once yet
            if (!hasMovedOnce)
            {
                return;
            }
            
            if (LevelManager.instance != null && 
                LevelManager.instance.dialogueBox.activeSelf)
            {
                idleTimer = 0; // reset the idle timer if dialogue box is active
            }
            else
            {
                idleTimer += Time.deltaTime;
            
                if (idleTimer >= meltTimeGap)
                {
                    // start melting
                    isMelting = true;
                
                    animator.Play("Melting");
                
                    // freeze the player ctrl
                    FreezeControls();
                    Debug.Log("Player started melting! Game Over!!");
                
                    // coroutine to wait for the melting animation to finish
                    // before showing the game over screen
                    StartCoroutine(WaitAndGameOver(1.5f));
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // moving
        rb.velocity = new Vector3(moveX * speed, moveY * speed, rb.velocity.z);
    }
    
    public void FreezeControls()
    {
        rb.velocity = Vector3.zero;
        enabled = false; // disable this script to prevent further movement
    }
    
    public void ResumeControls()
    {
        enabled = true; // enable this script to allow movement again
    }
    
    // game over
    private IEnumerator WaitAndGameOver(float bufferTime)
    {
        // wait
        yield return new WaitForSeconds(bufferTime);
        
        // load the bad ending scene
        GameManager.instance.LoadEndings(false);
    }
    
    public void SquashPlayer()
    {
        // if already melting, do nothing
        if (isMelting) return; 
        
        isMelting = true;
        
        // camera shake
        LevelManager.instance.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulseWithForce(1f);
        
        // make the player sprite vanish and play the squash particles
        spriteRenderer.enabled = false;
        
        squashEffect.transform.position = transform.position;
        squashEffect.SendEvent("OnSquash");
        
        FreezeControls();
        Debug.Log("Player got squashed! BAD ENDING!!");
        
        StartCoroutine(WaitAndGameOver(1.5f));
    }
}



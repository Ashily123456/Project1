using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // melting -- if already melting, skip the rest of the code
        if (isMelting || 
            LevelManager.instance.gameStarted == false)
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
        
        // melting
        if (isMoving)
        {
            // as long as the player moves, reset the idle timer
            idleTimer = 0;
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
}



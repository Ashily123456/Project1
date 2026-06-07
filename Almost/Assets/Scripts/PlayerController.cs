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
        moveX = 0f;
        moveY = 0f;
        
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
    }

    private void FixedUpdate()
    {
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
    
    // melting
    
}



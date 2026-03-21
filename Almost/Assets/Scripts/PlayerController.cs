using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 0.1f;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 1 * speed);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            rb.velocity = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, -1 * speed);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            rb.velocity = Vector3.zero; 
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector3(-1 * speed, rb.velocity.y, 0);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            rb.velocity = Vector3.zero;
        }
        
        if(Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector3(1 * speed, rb.velocity.y, 0);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            rb.velocity = Vector3.zero;
        }
    }
}



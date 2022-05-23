using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.AddForce(Vector3.right * speed * horizontalInput);
        rb.AddForce(Vector3.forward * speed * verticalInput);

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public static SphereController instance;
    public float distToGround;
    private Rigidbody rb;
    public Vector3 horVelocity;
    public float ArbitraryForce = 10f;
    bool isGrounded;
    // Start is called before the first frame update

    private void OnEnable()
    {
        instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    /*
    //WIP Pickup scriptbit for BuggyGame
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "MoneyUP")
        {
            collision.collider.
        }
        if (collision.gameObject.name == "Ground")
        {
            isGrounded = true;

            Color newColor = new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            GetComponent<Renderer>().material.color = newColor;
        }
    }

    */
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGrounded = false;
        }
    }
    void FixedUpdate()
    {
        if (Input.GetKey ("a")) //Move left Relative to Camera
        {
            rb.AddForce(-Vector3.right * ArbitraryForce);
        }
        if (Input.GetKey("d")) //Move right Relative to Camera
        {
            rb.AddForce(Vector3.right * ArbitraryForce);
        }
        if (Input.GetKey("w")) //Move forward Relative to Camera
        {
            rb.AddForce(Vector3.forward * ArbitraryForce);
        }
        if (Input.GetKey("s")) //Move left Relative to Camera
        {
            rb.AddForce(-Vector3.forward * ArbitraryForce);
        }
        if (Input.GetKey("space"))
        {
            if (isGrounded)
            {
                Debug.Log("Bounce!");
                rb.velocity = new Vector3(rb.velocity.x, 10, rb.velocity.z);

            }
            
        }
    }

}

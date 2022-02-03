using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ApplesPlayer : MonoBehaviour
{
    // Public Variables
    public bool turnable = false;
    public float MovementSpeed = 15;
    public Vector3 MovementAxis = new Vector3(1,0,0);  //Player starts on the x axis.  if this changes, change this vector3

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        //leftward movement
        if (Input.GetKey(KeyCode.A)) rb.velocity = (MovementAxis * MovementSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);

        //rightward movement
        else if (Input.GetKey(KeyCode.D)) rb.velocity = (MovementAxis * MovementSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);

        //if the player is not moving, stops x and z velocity
        else rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }


    //Checks if the player is at an intersection
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Intersection") turnable = true;
    }

    // exiting an intersection blocks the player from turning
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Intersection") turnable = false;
    }
}

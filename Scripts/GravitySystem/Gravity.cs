using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    private Rigidbody rb;
    private GameObject[] gravitationalGameObjects;
    private GravityFocus gf;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

	void FixedUpdate ()
    {
        gravitationalGameObjects = GameObject.FindGameObjectsWithTag("Gravity");
        foreach (GameObject gameObjectFound in gravitationalGameObjects)
        {
            gf = gameObjectFound.GetComponent<GravityFocus>();
            float gravitationalAcceleration = gf.gravitationalAcceleration;

            Vector3 distance = gameObjectFound.transform.position - transform.position;
            
            //Decreases the gravitational acceleration depending on distance.
            gravitationalAcceleration -= distance.magnitude * gf.distanceAccelerationDecrease; 
            if (gravitationalAcceleration < 0)
                gravitationalAcceleration = 0;

            //Applies the calculated acceleration to the game object.
            Vector3 acceleration = distance.normalized * gravitationalAcceleration;
            rb.AddForce(acceleration, ForceMode.Acceleration);
        }
	}
}

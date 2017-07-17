//Author: Ivan Ortiz Escarré

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DistanceData
{  
    //Distance the camera will be placed at the game start. (Positives distances means the camera being behind the focus).
    public float initialDistance;
    //The maximum and minimum distance the camera will be placed. (Without this the player will be able to move the camera distance from -inf to inf.)
    public float maxDistance;
    public float minDistance;
}

[System.Serializable]
public class HeightData
{
    //Height the camera will be placed at the game start. (Positives height means the camera being upper the focus).
    public float initialHeight;
    //Height the camera when the first person camera is active.
    public float firstPersonHeight;
    //The maximum and minimum height the camera will be placed. (Without this the player will be able to move the camera height from -inf to inf.)
    public float maxDegreeRotation;
}

public class CameraController : MonoBehaviour {
    //PUBLIC VARS
        //Float vars
            //The speed the camera distance is changed using mouse scroll wheel.
            public float scrollWheelSpeed;
    
        //Unity types vars
            //The gameobject the camera will follow.
            public GameObject focus;
            //The meshRenderer that will be invisible when the first person camera is activated.
            public MeshRenderer meshRenderer;
            //All distance data class (initial distance, max distance, min distance)
            public DistanceData distanceData;
            //All height data class (initial height, max height, min height)
            public HeightData heightData;


    //PRIVATE VARS
        //Bool vars
            private bool firstPersonCamera;

        //Float vars
            private float currentDistance;
            private float currentHeight;
            private float currentX;
            private float currentY;
            private float maxHeightDegree;
            private float minHeightDegree;

        //Unity types vars
            private Quaternion rotation;
            private Rigidbody rb;
            
    
    //Initialization
    void Start()
    {
        //Initializes the private class vars.
        currentDistance = distanceData.initialDistance;
        currentHeight = heightData.initialHeight;
        currentX = 0;
        currentY = 0;
        firstPersonCamera = false;
        rotation = focus.transform.rotation;

        //Initializes the camera position behind the focus.
        transform.position = focus.transform.position + -focus.transform.forward * currentDistance + new Vector3(0.0f, currentHeight, 0.0f);

        //Initializes the max and min degree depending on where the camera starts.
        float currentDegree = Mathf.Atan(currentHeight / currentDistance) * Mathf.Rad2Deg;
        maxHeightDegree = heightData.maxDegreeRotation - currentDegree;
        minHeightDegree = -heightData.maxDegreeRotation - currentDegree;
    }

    //Update is called once per frame
    void Update ()
    {
        //Changes the camera type when the "FirstPersonCamera" button is pressed,
        if (Input.GetButtonDown("First Person Camera"))
            firstPersonCamera = !firstPersonCamera;

        if (firstPersonCamera)
            FirstPersonCamera();
        else
            ThirdPersonCamera();
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate ()
    {
        if (firstPersonCamera)
        {
            transform.position = focus.transform.position + new Vector3(0.0f, heightData.firstPersonHeight, 0.0f);
            transform.rotation = Quaternion.Euler(-currentX, currentY, 0.0f);
        }
        else
        {
            //Gets the Quaternion of the focus rotation + mouse rotation.
            rotation = focus.transform.rotation * Quaternion.Euler(currentX, currentY, 0.0f);
            //Moves the camera to the right position.
            transform.position = focus.transform.position + rotation * new Vector3(0.0f, currentHeight, -currentDistance);
            //Unity function to make the camera look at the focus.
            transform.LookAt(focus.transform.position + new Vector3(0.0f, heightData.firstPersonHeight, 0.0f));
        }
        
    }

    /// <summary>
    /// First person camera system function.
    /// </summary>
    void FirstPersonCamera ()
    {
        //Makes the player invisible (deactivating the player renderer
        UpdateMeshRenderer(false);
        UpdateRotationAxis();
    }

    /// <summary>
    /// Third person camera system function.
    /// </summary>
    void ThirdPersonCamera ()
    {
        //Makes the player visible (activating the player renderer).
        UpdateMeshRenderer(true);

        //Changes the camera distance iwth the Mouse ScrollWheel axis and clamps it between two values (max & min).
        currentDistance += -Input.GetAxis("Mouse ScrollWheel") * scrollWheelSpeed * Time.deltaTime;
        currentDistance = Mathf.Clamp(currentDistance, distanceData.minDistance, distanceData.maxDistance);

        //Gets the camera rotation axis while the left mouse button is pressed.
        if (Input.GetMouseButton(0))
        {
            UpdateRotationAxis();
        }
        
        //Resets the X rotation when the left mouse button is released.
        if (Input.GetMouseButtonUp(0))
        {
            currentY = 0;
        }
    }

    /// <summary>
    /// Updates the currentX and currentY values using the vertical and horizontal mouse movement axis.
    /// </summary>
    void UpdateRotationAxis()
    {
        //When you rotate through the x axis what you are really doing is facing the camera up/down.
        currentY += Input.GetAxis("Mouse X");
        //When you rotate through the y axis what you are really doing is facing the camera left/right.
        currentX += Input.GetAxis("Mouse Y");
        //The currentY value will be always a value between minHeightDegree and maxHeightDegree using clamp.
        currentX = Mathf.Clamp(currentX, minHeightDegree, maxHeightDegree);
    }

    /// <summary>
    /// Makes the gameObject visible or invisible depending on the visible bool var. 
    /// </summary>
    /// <param name="visible"></param>
    void UpdateMeshRenderer(bool visible)
    {
        meshRenderer.enabled = visible;
    }
}
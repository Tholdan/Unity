using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectsSystem : MonoBehaviour {
    //PUBLIC
    public string grabbableObjectTag;

    public float rayHeight;
    public float grabDistance;

    //PRIVATE
    private Vector3 heightVector;
    private Vector3 grabVector;
    private RaycastHit hitInfo;
    private Transform grabbedObject;

    private bool isGrabbing;

    // Use this for initialization
    void Start () {
        hitInfo = new RaycastHit();
        heightVector =  new Vector3(0.0f, rayHeight, 0.0f);
        isGrabbing = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawLine(transform.position + heightVector, transform.position + heightVector + transform.forward * grabDistance);

        if (!isGrabbing)
        {
            if (Physics.Raycast(transform.position + heightVector, transform.forward, out hitInfo, grabDistance))
            {
                Debug.Log(hitInfo.collider.name);
                if (Input.GetMouseButtonDown(0))
                {
                    grabbedObject = hitInfo.transform;
                    grabVector = grabbedObject.transform.position - transform.position;
                    isGrabbing = true;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log("GrabVector: " + grabVector + " | Forward: " + transform.forward);
                grabbedObject.transform.position = transform.position + transform.forward * grabVector.magnitude + new Vector3(0.0f, grabVector.y, 0.0f);
                grabbedObject.transform.rotation = transform.rotation;
            }
            else
            {
                isGrabbing = false;
            }
        }
	}

    public bool IsGrabbing()
    {
        return isGrabbing;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    GameObject hmdCamera;
    CapsuleCollider playerCollider;

	// Use this for initialization
	void Start ()
    {
        hmdCamera = gameObject.GetComponent<PlayerMovement>().hmdCamera;
        playerCollider = gameObject.GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 hmdPos = transform.InverseTransformPoint(hmdCamera.transform.position);
        playerCollider.center = new Vector3(hmdPos.x, 1.0f, hmdPos.z);
	}
}

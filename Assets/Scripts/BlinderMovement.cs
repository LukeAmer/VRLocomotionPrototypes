using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinderMovement : MonoBehaviour
{
    [SerializeField]
    GameObject cameraOb;

    [SerializeField]
    GameObject[] blinders;
    [SerializeField]
    Vector2[] defaultOffScreenPos;

    [SerializeField]
    GameObject testCube;

    // Left
    // Right
    // Top
    // Bottom

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 offset = cameraOb.transform.position + cameraOb.transform.forward * 0.2f;

        gameObject.transform.position = offset;
        gameObject.transform.rotation = cameraOb.transform.rotation;

        UpdateBlinderSections();
	}


    void UpdateBlinderSections()
    {
        for(int i = 0; i < blinders.Length; i++)
        {

        }
    }
}

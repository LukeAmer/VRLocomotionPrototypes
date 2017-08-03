using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointControl : MonoBehaviour
{
    public bool playerReached = false;

    [SerializeField]
    Color defaultColour;
    [SerializeField]
    Color reachedColour;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(playerReached)
        {
            gameObject.GetComponent<SpriteRenderer>().color = reachedColour;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = defaultColour;
        }
	}

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            if(!col.gameObject.GetComponent<PlayerDashMovement>().dashing)
            {
                playerReached = true;
            }
        }
    }
}

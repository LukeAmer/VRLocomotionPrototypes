using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveControl : MonoBehaviour 
{
    [SerializeField]
    GameObject[] waypoints;
    [SerializeField]
    ControllerInput leftController;
    [SerializeField]
    ControllerInput rightController;

    [SerializeField]
    GameObject door;

    int objectiveNum = 0;
    float timeMenuHeld = 0.0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(objectiveNum == 0)
        {
            // Check Waypoints
            bool complete = true;
            for(int i = 0; i < waypoints.Length - 1; i++)
            {
                if (!waypoints[i].GetComponent<WaypointControl>().playerReached)
                    complete = false;
            }

            if(complete)
            {
                door.GetComponent<Animator>().SetBool("Open", true);

                objectiveNum++;
            }
        }	
        else
        {
            if(waypoints[3].GetComponent<WaypointControl>().playerReached)
            {
                Debug.Log("Complete");
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
        }

        if((rightController.codesSetup && rightController.controller.GetPress(rightController.controllerButtonCodes.applicationMenuButton)) || (leftController.codesSetup && leftController.controller.GetPress(leftController.controllerButtonCodes.applicationMenuButton)))
        {
            timeMenuHeld += 1.0f * Time.deltaTime;
        }
        else
        {
            timeMenuHeld = 0.0f;
        }

        if(timeMenuHeld > 2.0f)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        
	}
}

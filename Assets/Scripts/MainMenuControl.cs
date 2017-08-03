using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField]
    MainMenuOption[] options;
    [SerializeField]
    GameObject crossHair;
    [SerializeField]
    GameObject hmdCamera;

    [SerializeField]
    ControllerInput leftController;
    [SerializeField]
    ControllerInput rightController;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        crossHair.transform.position = hmdCamera.transform.position + hmdCamera.transform.forward * 1.0f;
        crossHair.transform.rotation = hmdCamera.transform.rotation;

        LookAtOptions();
	}

    void LookAtOptions()
    {
        MainMenuOption selectedOption = null;

        RaycastHit hit;
        if(Physics.Linecast(hmdCamera.transform.position, hmdCamera.transform.position + hmdCamera.transform.forward * 10.0f, out hit))
        {
            hit.collider.gameObject.transform.parent.gameObject.GetComponent<MainMenuOption>().selected = true;
            selectedOption = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MainMenuOption>();
        }

        for(int i = 0; i < options.Length; i++)
        {
            if (selectedOption == null || selectedOption != options[i])
                options[i].selected = false;
        }

        if ((rightController.codesSetup && rightController.controller.GetPressDown(rightController.controllerButtonCodes.touchPadButton)) || (leftController.codesSetup && leftController.controller.GetPressDown(leftController.controllerButtonCodes.touchPadButton)))
        {
            if(selectedOption != null)
            {
                // Select Option
                SceneManager.LoadScene(selectedOption.levelName, LoadSceneMode.Single);
            }
        }
    }
}

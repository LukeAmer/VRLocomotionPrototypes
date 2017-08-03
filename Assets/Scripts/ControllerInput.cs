using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    public enum controllerName { Left, Right};
    public controllerName controllerSide;

    [HideInInspector]
    public bool codesSetup = false;

    [HideInInspector]
    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)gameObject.GetComponent<SteamVR_TrackedObject>().index); } }

    [HideInInspector]
    public ControllerButtonCodesStruct controllerButtonCodes;

  

    // Use this for initialization
    void Start ()
    {
        // Setup Codes
        //controllerButtonCodes = new ControllerButtonCodesStruct();
        controllerButtonCodes.triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
        controllerButtonCodes.gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
        controllerButtonCodes.applicationMenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
        controllerButtonCodes.touchPadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

        codesSetup = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (controller.GetPressDown(controllerButtonCodes.triggerButton))
        {
            Debug.Log("Trigger Pressed!");
        }
        
        if(controller.GetPressDown(controllerButtonCodes.touchPadButton))
        {
            Debug.Log("Touch Pad Pressed!");
        }

        if (controller.GetPressDown(controllerButtonCodes.gripButton))
        {
            Debug.Log("Grip Pressed! " + controllerSide);
        }

        //Debug.Log(controller.GetAxis());

    }

    public struct ControllerButtonCodesStruct
    {
        public Valve.VR.EVRButtonId triggerButton;
        public Valve.VR.EVRButtonId gripButton;
        public Valve.VR.EVRButtonId applicationMenuButton;
        public Valve.VR.EVRButtonId touchPadButton;
    }

}

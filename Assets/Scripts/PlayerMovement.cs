using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // LINEAR FORWARD MOVEMENT WITH HEAD ROTATION STEERING
    public bool yawRotaion = false;
    public GameObject hmdCamera; // Link other objects to script via Unity Inspector
    public GameObject forward;
    public GameObject cameraHolder;

    [SerializeField]
    ControllerInput leftController; // Allow for Left & Right controllers to be linked via the Unity Inspector
    [SerializeField]
    ControllerInput rightController;

    public enum MovementStates { Standing, Moving };
    public MovementStates movementState = MovementStates.Standing; // Public movement states, allows for potentially more states to be added in the future rather than just using a boolean

    Quaternion targetRotation; 
    float targetSpeed = 0.0f;

    BlinderControl blinderControl; // Reference other scripts
    QuickBlur quickBlur;

    bool rotating = false;

    float centerAngle = 0.0f;

	// Use this for initialization
	void Start ()
    {
        blinderControl = hmdCamera.GetComponent<BlinderControl>(); // Link scripts from objects
        quickBlur = hmdCamera.GetComponent<QuickBlur>();
        targetRotation = transform.rotation; // Set default rotation
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Modular style functions allow for functionality to easily be disabled if required for testing
        MovementControl(); 
        SnapRotationControl();

    }

    void SnapRotationControl()
    {
        // Right Controller Grip Press & Rotation
        if(rightController.codesSetup && rightController.controller.GetPressDown(rightController.controllerButtonCodes.gripButton) && !rotating) // !rotating prevents additional calls while already rotating
        {
            Vector3 rot = transform.eulerAngles; // Get Current Euler rotation
            rot.y += 45.0f; // Set target rotation 45 degrees to the Right 

            StartCoroutine(SnapRotation(Quaternion.Euler(0.0f, rot.y, 0.0f))); // Start Coroutine that adjusts rotation over time rather than instantly rotating -- Prevents user from feeling disorientated as they can predict movement direction & previous look direction 
        }

        // Left Controller Grip Press & Rotation
        if(leftController.codesSetup && leftController.controller.GetPressDown(leftController.controllerButtonCodes.gripButton) && !rotating) // !rotating prevents additional calls while already rotating
        {
            Vector3 rot = transform.eulerAngles; // Get Current Euler rotation
            rot.y -= 45.0f; // Set target rotation 45 degrees to the Left 

            StartCoroutine(SnapRotation(Quaternion.Euler(0.0f, rot.y, 0.0f))); // Start Coroutine that adjusts rotation over time rather than instantly rotating -- Prevents user from feeling disorientated as they can predict movement direction & previous look direction 
        }
    }

    IEnumerator SnapRotation(Quaternion targetRotation)
    {
        rotating = true; // Prevent additional calls while rotating

        float delay = 0.5f; // Delay Error check if angle value never reaches target value

        quickBlur.StartCoroutine(quickBlur.FastBlur(0.1f)); // Call Coroutine from QuickBlur script that blurs camera for set time and fades in/out the effect

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f && delay > 0.0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200.0f * Time.deltaTime); // Rotate towards target rotation at set speed, independent of frame rate using DeltaTime

            delay -= 1.0f * Time.deltaTime; // Error check delay

            yield return null; // Return function for this frame to prevent freezing the entire program until While loop is complete -- restart from While loop next frame
        }

        rotating = false; 
    }

    void MovementControl()
    {
        // 
        float angle = 0.0f;
        float maxAngle = 0.0f;
        float minAngle = 0.0f;
        float speed = 0.0f;

        if (!yawRotaion) // Head Roll (Z) axis steering
        {
            angle = hmdCamera.transform.localEulerAngles.z;
            angle = (angle > 180) ? angle - 360 : angle; // Covert angle to value that can be both positive and negative, rather than between 360 and 0

            if (angle > 50.0f) // Set limits
                angle = 50.0f; 
            else if (angle < -50.0f)
                angle = -50.0f;

            maxAngle = 5.0f; // Set Angle Steering sensitivity
            minAngle = -5.0f; // Set Angle Steering sensitivity
            speed = 10.0f; // Set rotation speed multiplier based off angle value
        }
        else // Head Yaw (Y) axis steering
        {
            if(movementState == MovementStates.Standing)
            {
                centerAngle = hmdCamera.transform.localEulerAngles.y; // Set relative center Y axis angle
            }

            angle = centerAngle - hmdCamera.transform.localEulerAngles.y; // Get angle based off relative center angle -- 0 while standing
            //Debug.Log(angle); // Debug Log used for testing values throughout development

            if (angle > 50.0f)
                angle = 50.0f;
            else if (angle < -50.0f)
                angle = -50.0f;

            maxAngle = 10.0f; // Adjust sensitivity and speed values
            minAngle = -10.0f;
            speed = 15.0f;
        }

        // Right controller Touch Pad press -- Movement activation
        if (rightController.codesSetup && rightController.controller.GetPress(rightController.controllerButtonCodes.touchPadButton) && movementState == MovementStates.Standing)
        {
            movementState = MovementStates.Moving;

            // Set Initial Direction
            Vector3 fwd = hmdCamera.transform.position + hmdCamera.transform.forward * 2.0f;
            fwd.y = 0.0f;
            Vector3 dir = fwd - forward.transform.position;
            dir.y = 0.0f;

            cameraHolder.transform.parent = transform; // Detach HMD Camera to prevent visual snaping when rotating base

            forward.transform.rotation = Quaternion.LookRotation(dir); // Set forward rotation based of initial HMD camera look position

            cameraHolder.transform.parent = forward.transform; // Re-parent HMD camera

        }
        else if (rightController.codesSetup && !rightController.controller.GetPress(rightController.controllerButtonCodes.touchPadButton) && movementState == MovementStates.Moving)
        {
            movementState = MovementStates.Standing; // Touch pad released -- reset movement state
        }

        if (movementState == MovementStates.Moving) // Steering control while moving
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + forward.transform.forward, 2.0f * Time.deltaTime); // Move forward at a constant acceleration

            Vector3 targetPos = Vector3.zero;
            bool rotate = false;

            if (angle > maxAngle) // Check angle with sensitivity limits
            {
                targetPos = forward.transform.position + (forward.transform.forward * 2.0f) + (-forward.transform.right * 2.0f); // Offset target position
                targetPos.y = forward.transform.position.y; // Ignore Y position when altering direction

                rotate = true; // Rotate
            }
            else if (angle < minAngle)
            {
                targetPos = forward.transform.position + (forward.transform.forward * 2.0f) + (forward.transform.right * 2.0f);
                targetPos.y = forward.transform.position.y;

                rotate = true; // Rotate
            }

            if (rotate)
            {
                Vector3 lookPos = targetPos - forward.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);

                targetRotation = rotation;
                targetSpeed = 0.5f * Mathf.Abs(angle) / speed; // Get Absolute value of Angle (Positive) and divide by speed multiplier to get rotation speed -- This results rotation speed being greater the larger the angle
            }
            else
            {
                if (targetSpeed > 0.0f)
                    targetSpeed -= 1.0f * Time.deltaTime; // Decrease speed over time to make rotation smoother
            }

            forward.transform.rotation = Quaternion.Slerp(forward.transform.rotation, targetRotation, Time.deltaTime * targetSpeed); // Rotate over time
        }
        else
        {
            targetSpeed = 0.0f; // Reset values
            targetRotation = forward.transform.rotation;
        }
    }
}

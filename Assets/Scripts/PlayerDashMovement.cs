using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashMovement : MonoBehaviour
{
    // DASH MOVEMENT SYSTEM
    public bool multiDirectional = false;
    public bool dashing = false;
    public float dashRange = 5.0f;
    public GameObject hmdCamera; // Link other objects to script via Unity Inspector
    public GameObject forward;
    public GameObject cameraHolder;

    [SerializeField]
    ControllerInput leftController; // Allow for Left & Right controllers to be linked via the Unity Inspector
    [SerializeField]
    ControllerInput rightController;

    [SerializeField]
    GameObject dashTarget;
    [SerializeField]
    GameObject dashRangeTarget;
    [SerializeField]
    GameObject fixedCameraForward;

    QuickBlur quickBlur;

    bool rotating = false;
    float dashCooldown = 0.0f;
    Vector3 lastDashPos = Vector3.zero;

    // Use this for initialization
    void Start ()
    {
        quickBlur = hmdCamera.GetComponent<QuickBlur>();
    }
	
	// Update is called once per frame
	void Update ()
    {

        if(rightController.codesSetup)
            MovementControl();

        SnapRotationControl();	
	}

    void MovementControl()
    {
        bool canDash = false;
        fixedCameraForward.transform.position = hmdCamera.transform.position; // Position empty game object that ignores HMD rotation
        fixedCameraForward.transform.eulerAngles = new Vector3(0.0f, hmdCamera.transform.eulerAngles.y, 0.0f); // Only set Y rotation to get forward direction relitive to HMD camera

        if(rightController.controller.GetAxis().y > 0.0f || (multiDirectional && rightController.controller.GetAxis().magnitude > 0.0f)) // If touch pad is tracking input position
        {

            Vector3 hmdPos = hmdCamera.transform.position;
            hmdPos.y = transform.position.y; // Ignore Y position of HMD camera to get relitive direction

            Vector3 fwd = hmdPos + fixedCameraForward.transform.forward * 2.0f; // Get position Infront of HMD Camera
            Vector3 right = hmdPos + fixedCameraForward.transform.right * 2.0f; // Get position to the Right of HMD camera
            Vector3 fwdDir = fwd - hmdPos; // Set directions
            Vector3 righDir = right - hmdPos;
            fwdDir.y = 0.0f;
            righDir.y = 0.0f;
            
            Vector3 targetPos = hmdPos + fwdDir * rightController.controller.GetAxis().y * 10.0f; // Set target position foward multiplied by Touch pad input Y value (Up/Down)

            if(multiDirectional)
            {
                targetPos = hmdPos + (fwdDir * rightController.controller.GetAxis().y * dashRange) + (righDir * rightController.controller.GetAxis().x * dashRange); // If multidirectional get Touch pad X input value as well (Left/Right)
            }

            targetPos.y = transform.position.y; // Reset target position Y


            dashRangeTarget.transform.position = targetPos; // Set out of range target to same direction for now

            RaycastHit hit;
            if(Physics.Linecast(hmdPos, targetPos + (targetPos - hmdPos) * 0.25f, out hit, 1 << 8)) // Raycast from HMD positon to target postion checking for walls
            {
                targetPos = hit.point + (hmdPos - hit.point) * 0.25f; // Raycast Hit wall, reposition target position towards HMD camera from wall hit point
                Debug.Log("RAY HIT");
            }

            // Overlap Sphere to check for close walls
            targetPos = CheckClosestWall(targetPos, hmdPos);
            // Check re-positioned target position for walls once more -- Important for corners
            targetPos = CheckClosestWall(targetPos, hmdPos);

            if (Vector3.Distance(hmdPos, targetPos) > 2.0f) // Only allow dash if distance is above 2.0f, to prevent potentialy dashing through walls
                canDash = true;

            dashTarget.transform.position = targetPos; // Set green dash target position to final Target Position after Raycast and overlap sphere checks

            dashTarget.transform.rotation = fixedCameraForward.transform.rotation; // Set both rotations to face the camera on Y axis only
            dashRangeTarget.transform.rotation = fixedCameraForward.transform.rotation;

            if (dashTarget.transform.position == dashRangeTarget.transform.position) // If the final target positon is the same as the origional position before Raycast and Sphere cast, remove the red out of bounds circle from view
                dashRangeTarget.SetActive(false);
            else
                dashRangeTarget.SetActive(true);

            dashTarget.GetComponent<LineRenderer>().SetPosition(0, hmdPos); // Set green line render positions -- HMD Pos to Target pos
            dashTarget.GetComponent<LineRenderer>().SetPosition(1, dashTarget.transform.position);

            if (dashCooldown > 0.0f) // Cooldown timer to prevent contant dash, which may also increase chance of collision errors
            {
                canDash = false;
                dashCooldown -= 1.0f * Time.deltaTime; // Timer delay
            }

            if (rightController.controller.GetPressDown(rightController.controllerButtonCodes.touchPadButton) && canDash && !dashing) // If Touchpad button is pressed and delay is 0 and not already dashing
            {
                // Dash
                StartCoroutine(DashToTarget(targetPos)); // Start dash co-routine
                dashCooldown = 0.5f; // Set cooldown to half a second
            }
        }
        else
        {
            // If no touchpad touch is detected reset all positions of visual objects and line renders
            dashTarget.transform.position = Vector3.zero;
            dashRangeTarget.transform.position = Vector3.zero;
            dashRangeTarget.SetActive(false);

            dashTarget.GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
            dashTarget.GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
        }
    }

    IEnumerator DashToTarget(Vector3 targetPos)
    {
        dashing = true;

        // Set blur time -- Distance / Speed = Time
        quickBlur.StartCoroutine(quickBlur.FastBlur(Vector3.Distance(transform.position, targetPos) / 50.0f * Time.deltaTime));
        //Debug.Log((Vector3.Distance(transform.position, targetPos) / 50.0f * Time.deltaTime));

        // Move to point 0.8 distance error check to prevent over shooting the target
        while (Vector3.Distance(transform.position, targetPos) > 0.8f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 50.0f * Time.deltaTime);

            yield return null;
        }

        // Re-position just in case -- Frame rate differences can result in player ending up not reaching or moving further
        transform.position = targetPos;

        dashing = false; // Reset boolean to allow for next dash
    }

    void SnapRotationControl()
    {
        // Right Controller Grip Press & Rotation
        if (rightController.codesSetup && rightController.controller.GetPressDown(rightController.controllerButtonCodes.gripButton) && !rotating) // !rotating prevents additional calls while already rotating
        {
            Vector3 rot = transform.eulerAngles; // Get Current Euler rotation
            rot.y += 45.0f; // Set target rotation 45 degrees to the Right 

            StartCoroutine(SnapRotation(Quaternion.Euler(0.0f, rot.y, 0.0f))); // Start Coroutine that adjusts rotation over time rather than instantly rotating -- Prevents user from feeling disorientated as they can predict movement direction & previous look direction 
        }

        // Left Controller Grip Press & Rotation
        if (leftController.codesSetup && leftController.controller.GetPressDown(leftController.controllerButtonCodes.gripButton) && !rotating) // !rotating prevents additional calls while already rotating
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

    Vector3 CheckClosestWall(Vector3 targetPos, Vector3 hmdPos)
    {
        Collider[] cols = Physics.OverlapSphere(targetPos, 1.0f, 1 << 8); // Check for walls on Wall layer (8) with radius of 1 from target position

        if (cols.Length > 0) // If a wall has been detected
        {
            Collider closestWall = cols[0];
            float close = 1000.0f;

            for (int i = 0; i < cols.Length; i++) // Find closest wall if more than one is within overlap sphere
            {
                float dist = Vector3.Distance(targetPos, cols[i].ClosestPointOnBounds(targetPos)); // Set distance value

                if (dist < close)
                {
                    closestWall = cols[i];
                    close = dist;
                }
            }

            //Debug.DrawLine(hmdPos, closestWall.ClosestPointOnBounds(targetPos), Color.red);

            RaycastHit hit; // Linecast wall collision point and retrive Normal direction of face, and use this direction to offset target position to prevent player moving into the wall
            if (Physics.Linecast(hmdPos, closestWall.ClosestPointOnBounds(targetPos) + (closestWall.ClosestPointOnBounds(targetPos) - hmdPos) * 2.0f, out hit, 1 << 8))
            {
                targetPos = targetPos + hit.normal * 1.0f;
            }

            // Due to the dash speed we cannot rely on the Unity collision system to stop the player from moving through colliders while dashing, so these checks must be preformed before seting a dash target position
        }

        return targetPos; // Return the offset position -- If no collisions have been detected return the same targetPos
    }

}

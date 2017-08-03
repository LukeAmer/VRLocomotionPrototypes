using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinderControl : MonoBehaviour
{
    public GameObject baseOb;

    public float horizontalVelocityMulti = 1.0f;
    public float verticalVelocityMulti = 1.0f;
    public float smoothTime = 0.15f;

    public float amount = 0.0f;
    public float movementAmount = 1.1f;
    public float standingAmount = 0.9f;

    public float featherAmount = 0.4f;

    float avSlew;
    float av;

    int propAV;
    int propFeather;
    int propPosX;
    int propPosY;

    Vector3 lastFwd;
    Material m;

    float yCooldown = 0.0f;
    float xCooldown = 0.0f;

    float targetX = 0.0f;
    float targetY = 0.0f;

    float xMultiplier = 0.0f;
    float yMultiplier = 0.0f;

    float dir = 0.0f;
    float lastXAngle = 0.0f;
    float lastYAngle = 0.0f;

    bool trigger = false;
    bool disable = false;

    PlayerMovement playerMovement;
    PlayerDashMovement playerDashMovement;

    void Start()
    {
        playerMovement = baseOb.GetComponent<PlayerMovement>();
        playerDashMovement = baseOb.GetComponent<PlayerDashMovement>();
    }

    void Awake()
    {
        m = new Material(Shader.Find("Hidden/Tunnelling")); // Get shader

        propAV = Shader.PropertyToID("_AV"); // Get shader property values
        propFeather = Shader.PropertyToID("_Feather");
        propPosX = Shader.PropertyToID("_xPos");
        propPosY = Shader.PropertyToID("_yPos");
    }

    void Update()
    {

        float horAngle = Vector3.Angle(new Vector3(transform.forward.x, 0.0f, transform.forward.z), new Vector3(lastFwd.x, 0.0f, lastFwd.z)); // Get horizontal angle, based of current position and last position
        Vector3 cross = Vector3.Cross(new Vector3(transform.forward.x, 0.0f, transform.forward.z), new Vector3(lastFwd.x, 0.0f, lastFwd.z));
        if (cross.y < 0) horAngle = -horAngle; // Allow for negative angles

        if (horAngle > 5.0f) // Set min and max limits to prevent extreem mulipliers
            horAngle = 5.0f;
        else if (horAngle < -5.0f)
            horAngle = -5.0f;

        lastFwd = transform.forward; // Set last position

        float angle = transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle; // Allow for Positive and Negative angles 

        angle = angle - lastXAngle;

        if (angle > 5.0f)
            angle = 5.0f;
        else if (angle < -5.0f)
            angle = -5.0f;

        float verAngle = angle;  

        lastXAngle = (transform.localEulerAngles.x > 180) ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x;

        lastYAngle = (transform.localEulerAngles.y > 180) ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y;

        // Horizontal
        if (horAngle >= 1.0f || horAngle <= -1.0f)
        {
            if(horAngle <= -1.0f)
            {
                // Right
                xMultiplier = Mathf.Abs(horAngle / (10.0f * horizontalVelocityMulti)); // Set multipler based off angle
                //Debug.Log(horAngle / 10.0f);

                xCooldown = 0.2f; // Reset cooldown
            }
            else if(horAngle >= 1.0f)
            {
                // Left
                xMultiplier = -(horAngle / (10.0f * horizontalVelocityMulti));

                xCooldown = 0.2f;
            }
        }

        // Vertical
        if(verAngle >= 1.0f || verAngle <= -1.0f)
        {
            if(verAngle <= -1.0f)
            {
                // Up
                yMultiplier = Mathf.Abs(verAngle / (10.0f * verticalVelocityMulti));

                yCooldown = 0.2f;
            }
            else if(verAngle >= 1.0f)
            {
                // Down
                yMultiplier = -(verAngle / (10.0f * verticalVelocityMulti));

                yCooldown = 0.2f;
            }
        }

        // Reset
        if(xCooldown > 0.0f)
        {
            xCooldown -= 1.0f * Time.deltaTime;
            targetX = Mathf.Lerp(targetX, 0.0f + xMultiplier, 3.0f * Time.deltaTime); // Smooth lerp target x position value
        }
        else
        {
            targetX = Mathf.Lerp(targetX, 0.0f, 2.0f * Time.deltaTime); // Smooth lerp target x position value
            xMultiplier = 0.0f;
        }

        if(yCooldown > 0.0f)
        {
            yCooldown -= 1.0f * Time.deltaTime;
            targetY = Mathf.Lerp(targetY, 0.0f + yMultiplier, 3.0f * Time.deltaTime);
        }
        else
        {
            targetY = Mathf.Lerp(targetY, 0.0f, 3.0f * Time.deltaTime);
            yMultiplier = 0.0f;
        }

        if (!disable) // Set shader values
        {
            m.SetFloat(propAV, amount);
            m.SetFloat(propPosX, targetX);
            m.SetFloat(propPosY, targetY);
            m.SetFloat(propFeather, featherAmount);
        }
        else // Reset
        {
            m.SetFloat(propAV, 0.0f);
        }

        if(Input.GetKeyDown(KeyCode.Return)) // Debug test -- Press Return on Keyboard to disable FOV Binders
        {
            if (!disable)
                disable = true;
            else
                disable = false;
        }

        if (playerMovement.movementState == PlayerMovement.MovementStates.Moving) // If moving or dashing increase FOV value
        {
            amount = Mathf.Lerp(amount, movementAmount, 3.0f * Time.deltaTime);
        }
        else if(playerDashMovement.dashing)
        {
            amount = Mathf.Lerp(amount, movementAmount, 10.0f * Time.deltaTime);
        }
        else
        {
            amount = Mathf.Lerp(amount, standingAmount, 3.0f * Time.deltaTime);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m); // Render to screen
    }

    void OnDestroy()
    {
        Destroy(m);
    }
}

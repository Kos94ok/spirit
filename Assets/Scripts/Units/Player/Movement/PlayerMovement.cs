using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    const float maximumSpeed = 2.15f;
    const float acceleration = 50.00f;
    const float deceleration = 70.00f;
    const float sprintMaximumSpeed = 3.50f;
    const float sprintAcceleration = 50.00f;
    const float sprintDeceleration = 70.00f;
    const bool rocketEnabled = false;
    const float rocketFuelMax = 0.50f;
    const float rocketManaDrain = 20.00f;
    const float sprintManaCost = 5.00f;
    const float sprintManaBuffer = 5.00f;

    bool isSprinting = false;
    bool movementSprintDecelerating = false;
    Vector2 movementVector;
    //float jumpingVector = 0.00f;

    GameObject mainCamera;
    CharacterController movementController;
    UnitStats stats;
    PlayerEquipment equipment;

    void Start()
    {
        movementVector.x = 0.00f;
        movementVector.y = 0.00f;
        keyPressed.Add(KeyCode.W, false);
        keyPressed.Add(KeyCode.A, false);
        keyPressed.Add(KeyCode.S, false);
        keyPressed.Add(KeyCode.D, false);
        keyPressed.Add(KeyCode.Space, false);
        keyPressed.Add(KeyCode.LeftShift, false);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        movementController = GetComponent<CharacterController>();
        stats = GetComponent<UnitStats>();
        equipment = GetComponent<PlayerEquipment>();
    }

    // Update is called once per frame
    void Update()
    {
        Update_KeyStatus();
        Update_Rotation();
        Update_Jump();
        Update_Hover();
        Update_Movement();
        Update_Jump();
        Update_Blink();
    }
    //===================================================================================================
    // Key Status
    //===================================================================================================
    Dictionary<KeyCode, bool> keyPressed = new Dictionary<KeyCode, bool>();
    void Update_KeyStatus()
    {
        var KeyList = new List<KeyCode>(keyPressed.Keys);
        foreach (KeyCode Entry in KeyList)
        {
            if (Input.GetKeyDown(Entry))
            {
                keyPressed[Entry] = true;
            }
            if (Input.GetKeyUp(Entry))
            {
                keyPressed[Entry] = false;
            }
        }
    }
    //===================================================================================================
    // 3rd Person Movement
    //===================================================================================================
    void Update_Movement()
    {
        if (stats.IsDead()) { return; }

        float MovementAccel = acceleration * Time.smoothDeltaTime;
        float MovementDecel = deceleration * Time.smoothDeltaTime;
        float MovementMaxSpeed = maximumSpeed;
        float MovementAccelSprint = sprintAcceleration * Time.smoothDeltaTime;
        float MovementDecelSprint = sprintDeceleration * Time.smoothDeltaTime;
        float MovementMaxSpeedSprint = sprintMaximumSpeed;
        // Enable or disable sprint
        /*if (keyPressed[KeyCode.LeftShift])
        {
            movementSprintDecelerating = true;
        }
        else if (movementSprintDecelerating && Mathf.Abs(movementVector.x) <= MovementMaxSpeed && Mathf.Abs(movementVector.y) <= MovementMaxSpeed)
        {
            movementSprintDecelerating = false;
        }*/

        if (keyPressed[KeyCode.LeftShift] && stats.HasMana(sprintManaBuffer)
            && (keyPressed[KeyCode.W] || keyPressed[KeyCode.A] || keyPressed[KeyCode.S] || keyPressed[KeyCode.D]))
        {
            isSprinting = true;
        }
        else if (!keyPressed[KeyCode.LeftShift] || !stats.HasMana(sprintManaCost * Time.deltaTime)
            || (!keyPressed[KeyCode.W] && !keyPressed[KeyCode.A] && !keyPressed[KeyCode.S] && !keyPressed[KeyCode.D]))
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            stats.DrainMana(sprintManaCost * Time.deltaTime);
            movementSprintDecelerating = true;
        }
        else
        {
            movementSprintDecelerating = false;
        }

        // Acceleration
        if (keyPressed[KeyCode.W])
        {
            movementVector.y += MovementAccel;
            if (keyPressed[KeyCode.LeftShift])
            {
                movementVector.y += MovementAccelSprint - MovementAccel;
                if (movementVector.y > MovementMaxSpeedSprint)
                    movementVector.y = MovementMaxSpeedSprint;
            }
            else if (movementVector.y > MovementMaxSpeed && movementSprintDecelerating)
            {
                movementVector.y -= MovementAccel + MovementDecelSprint;
            }
            if (movementVector.y > MovementMaxSpeed && !movementSprintDecelerating)
                movementVector.y = MovementMaxSpeed;
        }
        else if (movementVector.y > 0.00f)
        {
            movementVector.y -= MovementDecel;
            if (movementVector.y < 0.00f)
                movementVector.y = 0.00f;
        }


        if (keyPressed[KeyCode.A])
        {
            movementVector.x -= MovementAccel;
            if (keyPressed[KeyCode.LeftShift])
            {
                movementVector.x -= MovementAccelSprint - MovementAccel;
                if (movementVector.x < -MovementMaxSpeedSprint)
                    movementVector.x = -MovementMaxSpeedSprint;
            }
            else if (movementVector.x < -MovementMaxSpeed && movementSprintDecelerating)
            {
                movementVector.x += MovementAccel + MovementDecelSprint;
            }
            if (movementVector.x < -MovementMaxSpeed && !movementSprintDecelerating)
                movementVector.x = -MovementMaxSpeed;
        }
        else if (movementVector.x < 0.00f)
        {
            movementVector.x += MovementDecel;
            if (movementVector.x > 0.00f)
                movementVector.x = 0.00f;
        }


        if (keyPressed[KeyCode.S])
        {
            movementVector.y -= MovementAccel;
            if (keyPressed[KeyCode.LeftShift])
            {
                movementVector.y -= MovementAccelSprint - MovementAccel;
                if (movementVector.y < -MovementMaxSpeedSprint)
                    movementVector.y = -MovementMaxSpeedSprint;
            }
            else if (movementVector.y < -MovementMaxSpeed && movementSprintDecelerating)
            {
                movementVector.y += MovementAccel + MovementDecelSprint;
            }
            if (movementVector.y < -MovementMaxSpeed && !movementSprintDecelerating)
                movementVector.y = -MovementMaxSpeed;
        }
        else if (movementVector.y < 0.00f)
        {
            movementVector.y += MovementDecel;
            if (movementVector.y > 0.00f)
                movementVector.y = 0.00f;
        }


        if (keyPressed[KeyCode.D])
        {
            movementVector.x += MovementAccel;
            if (keyPressed[KeyCode.LeftShift])
            {
                movementVector.x += MovementAccelSprint - MovementAccel;
                if (movementVector.x > MovementMaxSpeedSprint)
                    movementVector.x = MovementMaxSpeedSprint;
            }
            else if (movementVector.x > MovementMaxSpeed && movementSprintDecelerating)
            {
                movementVector.x -= MovementAccel + MovementDecelSprint;
            }
            if (movementVector.x > MovementMaxSpeed && !movementSprintDecelerating)
                movementVector.x = MovementMaxSpeed;
        }
        else if (movementVector.x > 0.00f)
        {
            movementVector.x -= MovementDecel;
            if (movementVector.x < 0.00f)
                movementVector.x = 0.00f;
        }
        float theta = -mainCamera.transform.eulerAngles.y * Mathf.Deg2Rad;

        float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);

        //movementVector = movementVector.normalized * MovementMaxSpeed;

        Vector2 fixedMovementVector = movementVector;
        // Fixing the diagonal movement
        if ((keyPressed[KeyCode.W] && keyPressed[KeyCode.A]) || (keyPressed[KeyCode.W] && keyPressed[KeyCode.D])
            || (keyPressed[KeyCode.S] && keyPressed[KeyCode.A]) || (keyPressed[KeyCode.S] && keyPressed[KeyCode.D]))
        {
            fixedMovementVector *= 0.7071069f;
        }
        // Slow when drawing the bow
        if (stats.buffs.Has(Buff.DrawingBow))
        {
            fixedMovementVector *= 0.70f;
        }

        float px = fixedMovementVector.x * cs - fixedMovementVector.y * sn;
        float py = fixedMovementVector.x * sn + fixedMovementVector.y * cs;

        Vector3 m = new Vector3(px * Time.smoothDeltaTime, (expectedHeight - transform.position.y) * 0.04f, py * Time.smoothDeltaTime);
        movementController.Move(m);
    }
    //===================================================================================================
    // Blink
    //===================================================================================================
    void Update_Blink()
    {
        bool hasCheapBlink = equipment.HasGlyph(Glyph.BlinkCost);
        bool hasPreciseBlink = equipment.HasGlyph(Glyph.BlinkPrecision);

        float blinkRange = 3.00f;
        float blinkManaCost = 30.00f;
        if (hasCheapBlink) { blinkManaCost = 15.00f; }

        if (Input.GetKeyDown(KeyCode.Space) && stats.HasMana(blinkManaCost))
        {
            stats.DrainMana(blinkManaCost);
            Vector3 movementVector;

            if (hasPreciseBlink) { movementVector = Vector3.MoveTowards(transform.position, Utility.GetMouseWorldPosition(transform.position), blinkRange) - transform.position; }
            else { movementVector = (Vector3.MoveTowards(transform.position, Utility.GetMouseWorldPosition(transform.position), 1.00f) - transform.position).normalized * blinkRange; }
            
            movementController.Move(movementVector);
        }
    }

    //===================================================================================================
    // Jumping
    //===================================================================================================
    void Update_Jump()
    {
        desiredHoveredHeight = defaultHoveredHeight;
        /*if (rocketEnabled == true && keyPressed[KeyCode.Space] && rocketDepleted == false)
        {
            desiredHoveredHeight += 10.00f * Time.smoothDeltaTime / Mathf.Max(1.00f, Mathf.Pow(GetControllerToGroundDist(), 2.00f));
            rocketFuel -= Time.smoothDeltaTime;
            if (rocketFuel <= 0.00f)
            {
                if (stats.mana >= rocketManaDrain)
                {
                    stats.DrainMana(rocketManaDrain);
                    rocketFuel = rocketFuelMax;
                }
                else
                {
                    rocketFuel = 0.00f;
                    rocketDepleted = true;
                }
            }
        }
        else
        {
            // Replenish supplies
            if (rocketFuel < rocketFuelMax && IsControllerGrounded())
            {
                rocketFuel = rocketFuelMax;
                rocketDepleted = false;
            }
            // Adjust height
            if (desiredHoveredHeight > defaultHoveredHeight && Mathf.Abs(desiredHoveredHeight - defaultHoveredHeight) > 0.10f)
                desiredHoveredHeight -= 5.00f * Time.smoothDeltaTime;
            else
                desiredHoveredHeight = defaultHoveredHeight;
        }*/
    }
    //===================================================================================================
    // Hovering
    //===================================================================================================
    float expectedHeight = 0.25f;
    float desiredHoveredHeight;
    float clearHoveredHeight;
    float defaultHoveredHeight;
    float hoveringTimer = 0.00f;
    bool rocketDepleted = false;
    float rocketFuel = rocketFuelMax;
    float lastGroundHeight = -1.00f;
    void Update_Hover()
    {
        hoveringTimer += Time.deltaTime;
        clearHoveredHeight = 0.25f + GetControllerGroundHeight() + 0.20f;
        defaultHoveredHeight = (Mathf.Sin(hoveringTimer * 2.00f) + 1.00f) / 5.00f + 0.25f + GetControllerGroundHeight();
        // Stairs fix
        if (lastGroundHeight != -1.00f && IsControllerGrounded())
        {
            float delta = GetControllerGroundHeight() - lastGroundHeight;
            defaultHoveredHeight += Mathf.Min(Mathf.Max(0.00f, delta), 0.25f) * 5.00f;
        }
        lastGroundHeight = GetControllerGroundHeight();

        //lastGroundHeight = expectedHeight;

        expectedHeight = desiredHoveredHeight;
    }
    //===================================================================================================
    // Rotation
    //===================================================================================================
    float RotationLastAngle;
    float RotationLookAtTarget;
    void Update_Rotation()
    {
        if (movementVector.x != 0.00f || movementVector.y != 0.00f)
        {
            float theta = -mainCamera.transform.eulerAngles.y * Mathf.Deg2Rad;

            float cs = Mathf.Cos(theta);
            float sn = Mathf.Sin(theta);

            float px = movementVector.x * cs - movementVector.y * sn;
            float py = movementVector.x * sn + movementVector.y * cs;

            Vector3 Direction = new Vector3(px, 0, py);
            if (Direction != Vector3.zero)
            {
                Quaternion RotationBlackBox = Quaternion.LookRotation(Direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, RotationBlackBox, 10.00f * Time.deltaTime);
            }
        }
    }
    //===================================================================================================
    // Collision
    //===================================================================================================
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var Body = hit.gameObject.GetComponent<Rigidbody>();
        if (Body != null)
            Body.AddForce(hit.moveDirection * hit.moveLength * 200);
    }
    //===================================================================================================
    // Extras
    //===================================================================================================
    public void ApplyForce(Vector3 direction, float force)
    {
        Vector3 forceVector = Vector3.MoveTowards(Vector3.zero, direction * 500.00f, force);
        movementVector.x = forceVector.x;
        movementVector.y = forceVector.z;
    }
    public bool IsControllerGrounded()
    {
        if (keyPressed[KeyCode.Space] && rocketDepleted == false)
            return false;
        else
            return transform.position.y - defaultHoveredHeight <= 0.40f;
        //return GetControllerToGroundDist() <= 0.40f;
    }
    float lastGroundedHeight;
    public float CalculateCameraHeight()
    {
        if (IsControllerGrounded() == false)
        {
            return transform.position.y;
        }
        else
        {
            return clearHoveredHeight;
        }
    }
    float GetControllerToGroundDist()
    {
        //RaycastHit hit;
        //Physics.Raycast(transform.position, Vector3.down, out hit);
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        float closestDist = 1000.00f;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag != "Hitbox" && (hit.distance < closestDist))
            {
                closestDist = hit.distance;
            }
        }
        if (closestDist < 1000.00f)
        {
            return Mathf.Round((closestDist - movementController.radius * transform.localScale.y) * 100.00f) / 100.00f;
        }
        else
            return 0.00f;
    }
    float GetControllerGroundHeight()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        float closestDist = 1000.00f;
        RaycastHit closestHit = new RaycastHit();
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag != "Enemy" && hit.transform.tag != "Hitbox" && hit.distance < closestDist)
            {
                closestHit = hit;
                closestDist = hit.distance;
            }
        }
        if (closestDist < 1000.00f)
        {
            return closestHit.point.y;
        }
        else
            return 0.00f;
    }
}
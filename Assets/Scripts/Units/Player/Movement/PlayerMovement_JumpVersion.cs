using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement_JumpVersion : MonoBehaviour
{
	// Initialization
	void Start ()
    {
        MovementVector.x = 0.00f;
        MovementVector.y = 0.00f;
        KeyPressed.Add(KeyCode.W, false);
        KeyPressed.Add(KeyCode.A, false);
        KeyPressed.Add(KeyCode.S, false);
        KeyPressed.Add(KeyCode.D, false);
        KeyPressed.Add(KeyCode.Space, false);
        KeyPressed.Add(KeyCode.LeftShift, false);
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        MovementController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Update_KeyStatus();
        Update_Rotation();
        Update_Jump();
        Update_Hover();
        Update_Movement();
	}
    //===================================================================================================
    // Key Status
    //===================================================================================================
    Dictionary<KeyCode, bool> KeyPressed = new Dictionary<KeyCode, bool>();
    void Update_KeyStatus()
    {
        var KeyList = new List<KeyCode>(KeyPressed.Keys);
        foreach (KeyCode Entry in KeyList)
        {
            if (Input.GetKeyDown(Entry))
            {
                KeyPressed[Entry] = true;
            }
            if (Input.GetKeyUp(Entry))
            {
                KeyPressed[Entry] = false;
            }
        }
    }
    //===================================================================================================
    // 3rd Person Movement
    //===================================================================================================
    bool MovementSprintDecelerating = false;
    Vector2 MovementVector;
    float JumpingVector = 0.00f;
    GameObject Camera;
    CharacterController MovementController;
    void Update_Movement()
    {
        float MovementAccel = 10.00f * Time.smoothDeltaTime;
        float MovementDecel = 40.00f * Time.smoothDeltaTime;
        float MovementMaxSpeed = 2.00f;
        float MovementAccelSprint = 20.00f * Time.smoothDeltaTime;
        float MovementDecelSprint = 20.00f * Time.smoothDeltaTime;
        float MovementMaxSpeedSprint = 3.00f;
        // Enable or disable sprint
        if (KeyPressed[KeyCode.LeftShift])
        {
            MovementSprintDecelerating = true;
        }
        else if (MovementSprintDecelerating && Mathf.Abs(MovementVector.x) <= MovementMaxSpeed && Mathf.Abs(MovementVector.y) <= MovementMaxSpeed)
        {
            MovementSprintDecelerating = false;
        }

        // Acceleration
        if (KeyPressed[KeyCode.W])
        {
            MovementVector.y += MovementAccel;
            if (KeyPressed[KeyCode.LeftShift])
            {
                MovementVector.y += MovementAccelSprint - MovementAccel;
                if (MovementVector.y > MovementMaxSpeedSprint)
                    MovementVector.y = MovementMaxSpeedSprint;
            }
            else if (MovementVector.y > MovementMaxSpeed && MovementSprintDecelerating)
            {
                MovementVector.y -= MovementAccel + MovementDecelSprint;
            }
            if (MovementVector.y > MovementMaxSpeed && !MovementSprintDecelerating)
                MovementVector.y = MovementMaxSpeed;
        }
        else if (MovementVector.y > 0.00f)
        {
            MovementVector.y -= MovementDecel;
            if (MovementVector.y < 0.00f)
                MovementVector.y = 0.00f;
        }
        

        if (KeyPressed[KeyCode.A])
        {
            MovementVector.x -= MovementAccel;
            if (KeyPressed[KeyCode.LeftShift])
             {
                 MovementVector.x -= MovementAccelSprint - MovementAccel;
                 if (MovementVector.x < -MovementMaxSpeedSprint)
                     MovementVector.x = -MovementMaxSpeedSprint;
             }
             else if (MovementVector.x < -MovementMaxSpeed && MovementSprintDecelerating)
             {
                 MovementVector.x += MovementAccel + MovementDecelSprint;
             }
            if (MovementVector.x < -MovementMaxSpeed && !MovementSprintDecelerating)
                MovementVector.x = -MovementMaxSpeed;
        }
        else if (MovementVector.x < 0.00f)
        {
            MovementVector.x += MovementDecel;
            if (MovementVector.x > 0.00f)
                MovementVector.x = 0.00f;
        }


        if (KeyPressed[KeyCode.S])
        {
            MovementVector.y -= MovementAccel;
            if (KeyPressed[KeyCode.LeftShift])
            {
                MovementVector.y -= MovementAccelSprint - MovementAccel;
                if (MovementVector.y < -MovementMaxSpeedSprint)
                    MovementVector.y = -MovementMaxSpeedSprint;
            }
            else if (MovementVector.y < -MovementMaxSpeed && MovementSprintDecelerating)
            {
                MovementVector.y += MovementAccel + MovementDecelSprint;
            }
            if (MovementVector.y < -MovementMaxSpeed && !MovementSprintDecelerating)
                MovementVector.y = -MovementMaxSpeed;
        }
        else if (MovementVector.y < 0.00f)
        {
            MovementVector.y += MovementDecel;
            if (MovementVector.y > 0.00f)
                MovementVector.y = 0.00f;
        }
        

        if (KeyPressed[KeyCode.D])
        {
            MovementVector.x += MovementAccel;
            if (KeyPressed[KeyCode.LeftShift])
            {
                MovementVector.x += MovementAccelSprint - MovementAccel;
                if (MovementVector.x > MovementMaxSpeedSprint)
                    MovementVector.x = MovementMaxSpeedSprint;
            }
            else if (MovementVector.x > MovementMaxSpeed && MovementSprintDecelerating)
            {
                MovementVector.x -= MovementAccel + MovementDecelSprint;
            }
            if (MovementVector.x > MovementMaxSpeed && !MovementSprintDecelerating)
                MovementVector.x = MovementMaxSpeed;
        }
        else if (MovementVector.x > 0.00f)
        {
            MovementVector.x -= MovementDecel;
            if (MovementVector.x < 0.00f)
                MovementVector.x = 0.00f;
        }
        float theta = -Camera.transform.eulerAngles.y * Mathf.Deg2Rad;

        float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);

        float px = MovementVector.x * cs - MovementVector.y * sn;
        float py = MovementVector.x * sn + MovementVector.y * cs;

        Vector3 m = new Vector3(px * Time.smoothDeltaTime, JumpingVector * Time.smoothDeltaTime, py * Time.smoothDeltaTime);
        MovementController.Move(m);
    }
    //===================================================================================================
    // Jumping
    //===================================================================================================
    void Update_Jump()
    {
        if (KeyPressed[KeyCode.Space] && IsControllerGrounded())
        {
            JumpingVector = 5.00f;
        }
        else if (IsControllerGrounded())
            JumpingVector = 0.00f;

        if (IsControllerGrounded() == false)
            if (KeyPressed[KeyCode.Space] && JumpingVector > 0.00f)
                JumpingVector -= 10.00f * Time.deltaTime;
            else
                JumpingVector -= 20.00f * Time.deltaTime;
    }
    //===================================================================================================
    // Hovering
    //===================================================================================================
    float DesiredHoveredHeight;
    float HoveringTimer = 1.00f;
    void Update_Hover()
    {
        HoveringTimer -= Time.deltaTime;
        DesiredHoveredHeight = Mathf.Sin(HoveringTimer) + 0.25f;
    }
    //===================================================================================================
    // Rotation
    //===================================================================================================
    float RotationLastAngle;
    float RotationLookAtTarget;
    void Update_Rotation()
    {
        if (MovementVector.x != 0.00f || MovementVector.y != 0.00f)
        {
            float theta = -Camera.transform.eulerAngles.y * Mathf.Deg2Rad;

            float cs = Mathf.Cos(theta);
            float sn = Mathf.Sin(theta);

            float px = MovementVector.x * cs - MovementVector.y * sn;
            float py = MovementVector.x * sn + MovementVector.y * cs;

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
    bool IsControllerGrounded()
    {
        return MovementController.isGrounded;
    }
}
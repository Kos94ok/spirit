using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public float cameraSpeed = 2.00f;

    GameObject player;
    PlayerMovement movementScript;
    CharacterController playerController;
    CameraControls cameraControls;
    float PlayerLastGroundedHeight = 0.00f;

    Vector3 movementVector;
    Vector3 cameraOffset;
    float movementVectorModifier = 1.00f;

    void Start ()
    {
        FindPlayer();
        cameraControls = GetComponent<CameraControls>();
        cameraOffset = new Vector3(-3.00f, 6.50f, -3.00f);
        movementVector = new Vector3(0.00f, 0.00f, 0.00f);
	}
    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementScript = player.GetComponent<PlayerMovement>();
        playerController = player.GetComponent<CharacterController>();
    }
	void Update ()
    {
        if (Vector3.Distance(player.transform.position + cameraOffset, transform.position) >= 1000.00f)
        {
            transform.Translate(player.transform.position + cameraOffset - transform.position, Space.World);
        }
        else
        {
            // Calculate camera offset from hero
            float dist = 4.25f;
            cameraOffset = (new Vector3(Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad), 0.00f, Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad))).normalized * -dist;
            cameraOffset.y = 6.50f;

            // Camera moves faster while rotating
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.V))
            {
                movementVectorModifier = 4.00f;
            }
            else if (movementVectorModifier > 1.00f)
            {
                movementVectorModifier -= 1.00f * Time.deltaTime;
                if (movementVectorModifier < 1.00f) { movementVectorModifier = 1.00f; }
            }

            movementVector.x = player.transform.position.x - transform.position.x + cameraOffset.x;
            movementVector.y = movementScript.CalculateCameraHeight() - transform.position.y + cameraOffset.y;
            movementVector.z = player.transform.position.z - transform.position.z + cameraOffset.z;
            movementVector *= cameraSpeed * movementVectorModifier;
            transform.Translate(movementVector.x * Time.smoothDeltaTime, movementVector.y * Time.smoothDeltaTime, movementVector.z * Time.smoothDeltaTime, Space.World);
        }
	}
}

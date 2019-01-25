using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    public int zoomTicks;
    public int maxZoom;

    float currentZoom = 0;

    bool isRotating = false;
    int currentRotation = 0;
    //float rotationTargetAngle;

    float defaultFieldOfView;

    GameObject player;
    Camera cameraHandle;

	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraHandle = GetComponent<Camera>();
        defaultFieldOfView = cameraHandle.fieldOfView;
	}
	
	void Update()
    {
        // Camera zoom
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        // If wheel not used, check the keys
        if (wheel == 0)
        {
            // Shift + Numerical Plus
            if (Input.GetKeyDown(KeyCode.KeypadPlus) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) { wheel = 1; }
            // Shift + Numerical Minus
            else if (Input.GetKeyDown(KeyCode.KeypadMinus) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) { wheel = -1; }
        }
        // If any related input is found
        if (wheel != 0)
        {
            // Zoom in
            if (currentZoom < zoomTicks && wheel > 0)
            {
                currentZoom += 1;
            }
            // Zoom out
            else if (currentZoom > 0 && wheel < 0)
            {
                currentZoom -= 1;
            }
            cameraHandle.fieldOfView = defaultFieldOfView - currentZoom * ((float)maxZoom / zoomTicks);
        }

        // C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentRotation += 1;
            if (currentRotation > 3) { currentRotation -= 4; }

            isRotating = true;
        }
        // V key
        else if (Input.GetKeyDown(KeyCode.V))
        {
            currentRotation -= 1;
            if (currentRotation < 0) { currentRotation += 4; }

            isRotating = true;
        }

        // Rotating in progress
        if (isRotating)
        {
            float rotationTargetAngle = currentRotation * 90.00f + 45.00f;
            // The angles are still bitchy on the left
            if (currentRotation == 0 && cameraHandle.transform.rotation.eulerAngles.y > 180.00f) { rotationTargetAngle += 360.00f; }
            if (currentRotation == 3 && cameraHandle.transform.rotation.eulerAngles.y < 180.00f) { rotationTargetAngle -= 360.00f; }

            // Rotating the camera
            if (cameraHandle.transform.rotation.eulerAngles.y != rotationTargetAngle)
            {
                // If close enough to the target, snap in place
                if (Mathf.Abs(rotationTargetAngle - cameraHandle.transform.rotation.eulerAngles.y) < 0.05f)
                {
                    isRotating = false;
                    cameraHandle.transform.Rotate(Vector3.up, (rotationTargetAngle - cameraHandle.transform.rotation.eulerAngles.y), Space.World);
                }
                // Otherwise, rotate as usual
                else
                {
                    cameraHandle.transform.Rotate(Vector3.up, (rotationTargetAngle - cameraHandle.transform.rotation.eulerAngles.y) * Time.deltaTime * 2.50f, Space.World);
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class PlayerInteractor : MonoBehaviour
{
    public int promptSize;
    public Texture2D promptTexture;

    GameObject targetedObject = null;
    Vector3 targetedObjectTransformOffset = Vector3.zero;
    List<GameObject> objectsInRange = new List<GameObject>();
    PlayerInteractorPrompt activePrompt = null;

    //Font textLabelFont;
    //GUIStyle promptTextStyle;

    Camera cameraHandle;
    void Start()
    {
        cameraHandle = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        // Determine fonts
        //foreach (string str in Font.GetOSInstalledFontNames())
        //{
            //Debug.Log(str);
        //}
        //textLabelFont = Font.CreateDynamicFontFromOSFont("Times New Roman", 16);
        //textLabelFont.material.mainTexture.filterMode = FilterMode.Point;
        //promptTextStyle = new GUIStyle();
        //promptTextStyle.fontStyle = FontStyle.Normal;
        //promptTextStyle.font = textLabelFont;
        //promptTextStyle.normal.textColor = Color.white;
    }

    void Update()
    {
        // Find new object
        float minimalDistance = 1000.00f;
        GameObject closestObject = null;
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            float distanceToObject = Math.GetDistance2D(transform.position, objectsInRange[i].transform.position);
            // Check if the object is within range
            if (distanceToObject > objectsInRange[i].GetComponent<Interact>().interactRange)
            {
                objectsInRange.RemoveAt(i);
                i -= 1;
            }
            // If yes, then compare it against last found object
            else if (distanceToObject < minimalDistance)
            {
                minimalDistance = distanceToObject;
                closestObject = objectsInRange[i];
            }
        }
        // Change the object if needed
        if (closestObject != targetedObject)
        {
            targetedObject = closestObject;
            if (targetedObject != null)
                targetedObjectTransformOffset = targetedObject.GetComponent<Interact>().transformOffset;

            // Create a new prompt
            if (activePrompt != null) { activePrompt.Destroy(); }
            if (targetedObject != null)
            {
                GameObject newPrompt = new GameObject();
                newPrompt.name = "Player Interact Prompt";
                newPrompt.transform.position = targetedObject.transform.position + targetedObjectTransformOffset;
                activePrompt = newPrompt.AddComponent<PlayerInteractorPrompt>();
                activePrompt.Initialize(promptTexture, promptSize);
            }
            else
            {
                activePrompt = null;
            }
        }

        // Interact with the object
        if (targetedObject != null && Input.GetKeyDown(KeyCode.E))
        {
            targetedObject.GetComponent<Interact>().OnInteract();
        }
    }

    public void Notify(GameObject sender)
    {
        if (!objectsInRange.Contains(sender))
        {
            objectsInRange.Add(sender);
        }
    }
    public void NotifyOnDestroy(GameObject sender)
    {
        if (objectsInRange.Contains(sender))
        {
            objectsInRange.Remove(sender);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Units.Common;

public class PlayerInteractorPrompt : MonoBehaviour
{
    int size;
    Texture2D texture;
    float fadeInSpeed = 10.00f;
    float fadeOutSpeed = 3.00f;

    bool isBeingDestroyed = false;
    float currentAlpha = 0.00f;

    Camera cameraHandle;
    void Start()
    {
        cameraHandle = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        if (!isBeingDestroyed && currentAlpha < 1.00f)
        {
            currentAlpha += fadeInSpeed * Time.deltaTime;
            if (currentAlpha > 1.00f) { currentAlpha = 1.00f; }
        }
        else if (isBeingDestroyed && currentAlpha > 0.00f)
        {
            currentAlpha -= fadeOutSpeed * Time.deltaTime;
            if (currentAlpha < 0.00f)
            {
                currentAlpha = 0.00f;
                Destroy(gameObject);
            }
        }
    }

    void OnGUI()
    {
//        if (currentAlpha > 0.00f)
//        {
//            Vector3 position = cameraHandle.WorldToScreenPoint(transform.position);
//
//            GUI.BeginGroup(new Rect(position.x - size / 2, cameraHandle.pixelHeight - position.y - size / 2, size, size));
//                GUI.color = new Color(1.00f, 1.00f, 1.00f, currentAlpha);
//                GUI.DrawTexture(new Rect(0, 0, size, size), texture);
//                //GUI.Label(new Rect(promptSize / 4, promptSize / 4, promptSize, promptSize), "E", promptTextStyle);
//            GUI.EndGroup();
//        }
    }
    public void Initialize(Texture2D texture, int size)
    {
        this.texture = texture;
        this.size = size;
    }
    public void Destroy()
    {
        isBeingDestroyed = true;
        gameObject.AddComponent<TimedLife>().Timer = 1.00f;
    }
}

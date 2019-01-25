using UnityEngine;
using System.Collections;

public class SimpleTeleport : MonoBehaviour
{
    public GameObject teleportExit;

    public void OnTriggerEnter(Collider other)
    {
        other.transform.position = teleportExit.transform.position;
    }
}

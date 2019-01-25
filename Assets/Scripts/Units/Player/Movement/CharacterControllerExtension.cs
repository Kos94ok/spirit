using UnityEngine;
using System.Collections.Generic;

class CharacterControllerExtension : MonoBehaviour
{
    [HideInInspector]
    public Vector3 averageVelocity;

    List<Vector3> velocityHistory = new List<Vector3>();

    CharacterController original;

    void Start()
    {
        original = GetComponent<CharacterController>();
    }

    void Update()
    {
        while (velocityHistory.Count > 60)
        {
            velocityHistory.RemoveAt(0);
        }
        velocityHistory.Add(original.velocity);

        averageVelocity = new Vector3(0.00f, 0.00f, 0.00f);
        for (int i = 0; i < velocityHistory.Count; i++)
        {
            averageVelocity += velocityHistory[i];
        }
        averageVelocity /= velocityHistory.Count;
    }
}

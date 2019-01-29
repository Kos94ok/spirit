using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Units.Common;

public class CorruptedFireDeath : UnitDeath
{
    public override void OnDeath()
    {
        // Add burst decay particle emitter
        GameObject decayPE = Instantiate(Resources.Load("CorruptedFirePEDeath")) as GameObject;
        decayPE.transform.position = transform.position;
        decayPE.transform.rotation = transform.rotation;
        decayPE.transform.Rotate(Vector3.up, -90, Space.World);

        // Set free the main particle emitter
        GameObject Child = transform.GetChild(0).gameObject;
        Child.transform.parent = null;
        Child.AddComponent<TimedLife>().Timer = 2.00f;
        Child.GetComponent<ParticleSystem>().enableEmission = false;

        Destroy(gameObject);
    }
    public override void ApplyForce(Vector3 force, ForceMode mode)
    {
        //GetComponent<Rigidbody>().AddForce(force, mode);
    }
}

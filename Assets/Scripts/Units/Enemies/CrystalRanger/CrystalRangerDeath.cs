using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Units.Common;

public class CrystalRangerDeath : UnitDeath
{
    public override void OnDeath()
    {
        // Set free the main particle emitter
        GameObject body = GetComponent<CrystalRangerAI>().AskForBody();
        GameObject bodyBase = GetComponent<CrystalRangerAI>().AskForBase();
        List<GameObject> spheres = GetComponent<CrystalRangerAI>().AskForSpheres();
        body.transform.parent = null;
        body.AddComponent<TimedLife>().Timer = 2.00f;
        body.GetComponent<ParticleSystem>().enableEmission = false;

        // Add burst decay particle emitter
        GameObject decayPE = Instantiate(Resources.Load("CrystalRangerPEDeath")) as GameObject;
        decayPE.transform.position = body.transform.position;
        decayPE.transform.rotation = body.transform.rotation;

        decayPE = Instantiate(Resources.Load("CrystalRangerBasePEDeath")) as GameObject;
        decayPE.transform.position = bodyBase.transform.position;
        decayPE.transform.rotation = bodyBase.transform.rotation;

        // Timeout the spheres
        for (int i = 0; i < spheres.Count; i++)
        {
            // Disable the old object
            spheres[i].transform.parent = null;
            spheres[i].AddComponent<TimedLife>().Timer = 2.00f;
            spheres[i].GetComponentInChildren<ParticleSystem>().enableEmission = false;

            // Create an effect
            decayPE = Instantiate(Resources.Load("CrystalRangerProjectilePEDeathStationary")) as GameObject;
            decayPE.transform.position = spheres[i].transform.position;
            decayPE.transform.rotation = spheres[i].transform.rotation;
            //decayPE.transform.Rotate(Vector3.up, -90, Space.World);
        }

        Destroy(gameObject);
    }

    public override void ApplyForce(Vector3 force, ForceMode mode)
    {
        return;
    }
}

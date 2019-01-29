using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Units.Common;

public class EternalGuardianDeath : UnitDeath
{
    List<GameObject> blades;
    public void Start()
    {
        blades = GetComponent<EternalGuardianAI>().AskForBlades();
    }
    public override void OnDeath()
    {
        // Add burst decay particle emitter
        GameObject decayPE = Instantiate(Resources.Load("EternalGuardianPEDeath")) as GameObject;
        decayPE.transform.position = transform.position;
        decayPE.transform.rotation = transform.rotation;
        //decayPE.transform.Rotate(Vector3.up, -90, Space.World);

        // Set free the main particle emitter
        GameObject Child = transform.GetChild(0).gameObject;
        Child.transform.parent = null;
        Child.AddComponent<TimedLife>().Timer = 2.00f;
        Child.GetComponent<ParticleSystem>().enableEmission = false;

        // Timeout the blades
        for (int i = 0; i < blades.Count; i++)
        {
            // Disable the old object
            blades[i].GetComponentInChildren<EternalGuardianAttackHitbox>().SetDamage(0.00f, 0.00f);
            blades[i].transform.parent = null;
            blades[i].AddComponent<TimedLife>().Timer = 2.00f;
            blades[i].GetComponentInChildren<ParticleSystem>().enableEmission = false;

            // Create an effect
            decayPE = Instantiate(Resources.Load("EternalGuardianPEDeathBlade")) as GameObject;
            decayPE.transform.position = blades[i].transform.position;
            decayPE.transform.rotation = blades[i].transform.rotation;
            //decayPE.transform.Rotate(Vector3.up, -90, Space.World);
        }

        Destroy(gameObject);
    }
    public override void ApplyForce(Vector3 force, ForceMode mode)
    {
        //GetComponent<Rigidbody>().AddForce(force, mode);
    }
}

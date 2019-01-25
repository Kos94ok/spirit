using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostileGolemDeath : UnitDeath
{
    bool RagdollEnabled = false;
    List<GameObject> RagdollParts;
    public override void OnDeath()
    {
        if (RagdollEnabled == true)
            return;

        RagdollParts = new List<GameObject>();
        float RandomOffset = 0.50f;
        Vector3 ExplosionCenter = transform.position + new Vector3(Random.Range(-RandomOffset, RandomOffset), Random.Range(-RandomOffset, RandomOffset), Random.Range(-RandomOffset, RandomOffset));
        //Destroy(GetComponent<NavMeshAgent>());
        //gameObject.AddComponent<TimedLife>().Timer = 5.00f;
        for (int i = 0; i < 3; i++)
        {
            // Find a child object, detach and clean them up
            GameObject Child = transform.GetChild(0).gameObject;
            RagdollParts.Add(Child);
            Child.transform.parent = null;
            Child.AddComponent<CorpseCleanUp>().SetTimer(2.50f, 0.0f);

            // Add decay particle emitter
            GameObject decayPE = Instantiate(Resources.Load("HostileGolemPEDeath")) as GameObject;
            decayPE.transform.rotation = Child.transform.rotation;
            decayPE.transform.Rotate(Vector3.up, -90, Space.World);
            decayPE.AddComponent<BasicFollow>().Follow(Child);

            // Setup mesh colliders
            Destroy(Child.GetComponent<BoxCollider>());
            Child.AddComponent<MeshCollider>().convex = true;

            // Setup physics
            if (Child.GetComponent<Rigidbody>() == null)
                Child.AddComponent<Rigidbody>();
            else
            {
                Child.GetComponent<Rigidbody>().useGravity = true;
                Child.GetComponent<Rigidbody>().isKinematic = false;
            }
            Child.GetComponent<Rigidbody>().AddExplosionForce(100.00f, ExplosionCenter, 5.00f, 0.00f);
        }
        RagdollEnabled = true;
        Destroy(gameObject);
    }
    public override void ApplyForce(Vector3 force, ForceMode mode)
    {
        if (RagdollEnabled == false)
            return;

        foreach (GameObject Part in RagdollParts)
        {
            Part.GetComponent<Rigidbody>().AddForce(force, mode);
        }
    }
}

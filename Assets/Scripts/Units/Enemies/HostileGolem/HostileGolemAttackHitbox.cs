using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostileGolemAttackHitbox : MonoBehaviour
{
    const float Damage = 25.00f;

    List<GameObject> hitObjects;
	void Start()
    {
        hitObjects = new List<GameObject>();
	}
	void Update()
    {
	
	}
    public void ResetHitObjects()
    {
        hitObjects = new List<GameObject>();
    }
    void OnTriggerEnter(Collider hit)
    {
        var EnemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        if (EnemyHit == null || hitObjects.Contains(EnemyHit.gameObject))
            return;

        if (EnemyHit.Stats.Alliance == UnitAlliance.Player && EnemyHit.Stats.IsAlive())
        {
            EnemyHit.DealDamage(Damage, gameObject);
            hitObjects.Add(EnemyHit.gameObject);
            UnitBleeding Bleeding = hit.GetComponent<UnitBleeding>();
            if (Bleeding != null)
            {
                GameObject bloodEmitter = Instantiate(Resources.Load("EnemyPEBlood")) as GameObject;
                bloodEmitter.GetComponent<ParticleSystem>().startColor = Bleeding.bloodColor;
                bloodEmitter.transform.rotation = transform.rotation;
                bloodEmitter.transform.position = transform.position;
                bloodEmitter.transform.Rotate(Vector3.up, -90, Space.World);
            }
        }
    }
}

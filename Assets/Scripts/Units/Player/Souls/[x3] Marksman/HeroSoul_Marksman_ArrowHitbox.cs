using UnityEngine;
using System.Collections;
using Units;

public class HeroSoul_Marksman_ArrowHitbox : MonoBehaviour
{
    float collisionDamage = 0.00f;
    float movementAngle;
    public void SetDamage(float damage)
    {
        collisionDamage = damage;
    }
    public void SetVisual(float rotation)
    {
        movementAngle = rotation;
    }

    void OnTriggerEnter(Collider hit)
    {
        if (collisionDamage == 0.00f) { return; }

        var enemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        if (enemyHit == null)
            return;

        if (enemyHit.Stats.Alliance == UnitAlliance.Corruption && enemyHit.Stats.IsAlive())
        {
            enemyHit.DealDamage(collisionDamage, gameObject);
            UnitBleeding Bleeding = hit.GetComponent<UnitBleeding>();
            if (Bleeding != null)
            {
                GameObject bloodEmitter = Instantiate(Resources.Load("EnemyPEBlood")) as GameObject;
                bloodEmitter.GetComponent<ParticleSystem>().startColor = Bleeding.bloodColor;
                bloodEmitter.transform.position = hit.transform.position;
                bloodEmitter.transform.Rotate(Vector3.up, -movementAngle, Space.World);
            }
            GetComponentInParent<HeroSoul_Marksman_Arrow>().KillProjectile();
        }
    }
}

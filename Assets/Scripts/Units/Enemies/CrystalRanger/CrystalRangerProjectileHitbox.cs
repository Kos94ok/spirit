using UnityEngine;
using System.Collections;

public class CrystalRangerProjectileHitbox : MonoBehaviour
{
    float collisionDamage = 0.00f;
    float parentRotation;
    public void SetDamage(float damage)
    {
        collisionDamage = damage;
    }
    public void SetVisual(float rotation)
    {
        parentRotation = rotation;
    }

    void OnTriggerEnter(Collider hit)
    {
        if (collisionDamage == 0.00f) { return; }

        var enemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        if (enemyHit == null)
            return;

        if (enemyHit.Stats.Alliance == UnitAlliance.Player && enemyHit.Stats.IsAlive())
        {
            enemyHit.DealDamage(collisionDamage, gameObject);
            UnitBleeding Bleeding = hit.GetComponent<UnitBleeding>();
            if (Bleeding != null)
            {
                GameObject playerHitPE = Instantiate(Resources.Load("CorruptedFirePEOnPlayerHit")) as GameObject;
                playerHitPE.transform.position = hit.transform.position;
                playerHitPE.transform.Rotate(Vector3.up, -parentRotation, Space.World);
            }
            GetComponentInParent<CrystalRangerProjectile>().KillProjectile();
        }
    }
}

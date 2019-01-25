using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EternalGuardianAttackHitbox : MonoBehaviour
{
    float damageOnImpact = 0.00f;
    float damagePerSecond = 0.00f;
    List<UnitHitbox> hitObjects;

    public void SetDamage(float onImpact, float perSecond)
    {
        damageOnImpact = onImpact;
        damagePerSecond = perSecond;
    }

    void Start()
    {
        hitObjects = new List<UnitHitbox>();
	}
	void Update()
    {
        // Contact damage per second
        if (damagePerSecond > 0.00f)
        {
            for (int i = 0; i < hitObjects.Count; i++)
            {
                hitObjects[i].DealDamage(damagePerSecond * Time.deltaTime, gameObject);

                GameObject playerBurnPE = Instantiate(Resources.Load("CorruptedFirePEOnPlayerHitSmall")) as GameObject;
                playerBurnPE.transform.position = hitObjects[i].transform.position;
            }
        }
    }
    void OnTriggerEnter(Collider hit)
    {
        var enemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        if (enemyHit == null || hitObjects.Contains(enemyHit))
            return;

        if (enemyHit.Stats.alliance == UnitAlliance.Player && enemyHit.Stats.IsAlive())
        {
            hitObjects.Add(enemyHit);
            if (damageOnImpact > 0.00f)
            {
                enemyHit.DealDamage(damageOnImpact, gameObject);
                UnitBleeding Bleeding = hit.GetComponent<UnitBleeding>();
                if (Bleeding != null)
                {
                    GameObject playerHitPE = Instantiate(Resources.Load("CorruptedFirePEOnPlayerHit")) as GameObject;
                    playerHitPE.transform.rotation = transform.rotation;
                    playerHitPE.transform.position = hit.transform.position;
                    playerHitPE.transform.Rotate(Vector3.up, 90, Space.World);
                }
            }
        }
    }
    void OnTriggerExit(Collider hit)
    {
        var EnemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        if (EnemyHit == null || !hitObjects.Contains(EnemyHit))
            return;

        hitObjects.Remove(EnemyHit);
    }
}

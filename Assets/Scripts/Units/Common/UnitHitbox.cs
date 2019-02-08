using UnityEngine;
using System.Collections;
using Misc;
using Units;

public class UnitHitbox : MonoBehaviour
{
    public UnitStats Stats;
    void Start()
    {
        // Looking for UnitStats
        if (Stats == null)
        {
            Stats = GetComponent<UnitStats>();
            if (Stats == null)
                Stats = GetComponentInParent<UnitStats>();
        }
        // Looking for RigidBody
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody Body = gameObject.AddComponent<Rigidbody>();
            Body.isKinematic = true;
            Body.useGravity = false;
        }
    }
    public void DealDamage(float amount, GameObject source)
    {
        Stats.DealDamage(amount, Maybe<GameObject>.Some(source));
    }
    public void HealDamage(float amount)
    {
        Stats.HealDamage(amount);
    }
    public void DrainMana(float amount)
    {
        Stats.DrainMana(amount);
    }
    public void RestoreMana(float amount)
    {
        Stats.RestoreMana(amount);
    }
    public void ApplyForce(Vector3 force, ForceMode mode)
    {
        Stats.ApplyForce(force, mode);
    }
}

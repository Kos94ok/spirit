using UnityEngine;
using System.Collections;

public abstract class UnitDeath : MonoBehaviour
{
    public abstract void OnDeath();
    public abstract void ApplyForce(Vector3 force, ForceMode mode);
}

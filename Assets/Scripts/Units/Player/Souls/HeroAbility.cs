using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AbilityType {
    SingleShot,
    Hold,
}

public abstract class HeroAbility : MonoBehaviour {
    float cooldownTimer = 0.00f;

    public void Start() {

    }

    // API functions
    public abstract AbilityType GetAbilityType();

    public virtual float GetHealthCost() { return 0.00f; }
    public virtual float GetManaCost() { return 0.00f; }

    // Fires only for Hold type abilities
    public virtual void OnTriggerBegin() { }
    // Fires only for Hold type abilities
    public virtual void OnTriggerEnd() { }
    // Fires for both SingleShot and Hold type abilities
    public abstract void OnTrigger();

    // Service functions
    protected void SetCooldown(float value) {
        cooldownTimer = value;
    }
    protected void ResetCooldown() {
        SetCooldown(0.00f);
    }
    protected bool IsOnCooldown() {
        return cooldownTimer > 0.00f;
    }
}

using UnityEngine;
using System.Collections;
using System;

public enum AbilitySlot {
    LeftClick,
    RightClick,
    FirstCore,
    SecondCore,
    Ultimate,
}

public abstract class HeroSoul : MonoBehaviour {
    protected UnitStats stats;
    protected BuffController buffs;
    protected GameObject player;
    public virtual void Start() {
        stats = GetComponent<UnitStats>();
        buffs = GetComponent<BuffController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public virtual void Update() {
    }
}

public abstract class HeroSoulOffensive : HeroSoul {
    protected HeroAbility[] ability;

    new public virtual void Start() {
        base.Start();
    }

    public HeroAbility GetAbility(AbilitySlot slot) {
        return ability[(int)slot];
    }
    public void RegisterAbility(AbilitySlot slot, HeroAbility abilityController) {
        ability[(int)slot] = abilityController;
    }
}

public abstract class HeroSoulOffensive_Old : HeroSoul {
    // Settings flags
    public bool AllowRepeatOnButtonHold = false;
    public bool AllowAltRepeatOnButtonHold = false;
    // Obligatory data
    public float CooldownTimer = 0.00f;
    // API functions
    public virtual float GetHealthCost() { return 0.00f; }
    public virtual float GetManaCost() { return 0.00f; }
    public virtual float GetAltHealthCost() { return GetHealthCost(); }
    public virtual float GetAltManaCost() { return GetManaCost(); }
    public virtual bool IsOnCooldown() { return CooldownTimer > 0.00f; }
    public virtual bool IsSupport() { return false; }
    // Event functions
    public abstract bool AttackPress(Vector3 Target);
    public abstract bool AttackRelease(Vector3 Target);
    public abstract bool AlternatePress(Vector3 Target);
    public abstract bool AlternateRelease(Vector3 Target);
}
public abstract class HeroSoulDefensive : HeroSoul {
}
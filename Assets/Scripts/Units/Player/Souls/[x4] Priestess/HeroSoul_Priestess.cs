using UnityEngine;
using System.Collections.Generic;
using Units.Common;

public class HeroSoul_Priestess : HeroSoulOffensive_Old
{
    const float ticksPerSecond = 60;

    const float healPerSecond = 50f;
    const float shieldBurst = 30f;
    const float normalHealManaCost = 30f;
    const float combatHealManaCost = 30f;
    const float shieldManaBuffer = 1f;
    const float shieldBrokenCooldown = 3f;

    private GameObject healEffectPrefab;
    private GameObject shieldEffectPrefab;
    private GameObject manaShieldEffectPrefab;
    private GameObject manaShieldBrokenPrefab;

    private GameObject healEffect;
    private GameObject manaShieldEffect;

    private float manaShieldCooldown = 0f;

    // API functions
    public override bool IsSupport() { return true; }
    public override float GetManaCost()
    {
        if (!stats.IsInCombat()) { return normalHealManaCost / ticksPerSecond; }
        else { return combatHealManaCost / ticksPerSecond; }
    }
    public override float GetAltManaCost() { return shieldManaBuffer; }

    // Basic
    public override void Start()
    {
        base.Start();
        healEffectPrefab = Resources.Load("HeroSoulPriestessHealPE") as GameObject;
        manaShieldEffectPrefab = Resources.Load("HeroSoulPriestessManaShieldPE") as GameObject;
        manaShieldBrokenPrefab = Resources.Load("HeroSoulPriestessManaShieldBrokenPE") as GameObject;
        AllowRepeatOnButtonHold = true;
        AllowAltRepeatOnButtonHold = true;
    }

    public override void Update()
    {
        base.Update();
        // Update timers
        if (manaShieldCooldown > 0f)
        {
            manaShieldCooldown -= Time.deltaTime;
            if (manaShieldCooldown < 0f) { manaShieldCooldown = 0f; }
        }

        // Move effects
        if (healEffect != null)
        {
            healEffect.transform.position = player.transform.position;
        }
        if (manaShieldEffect != null && stats.HasMana(shieldManaBuffer))
        {
            manaShieldEffect.transform.position = player.transform.position;
        }
        // Mana shield broken
        else if (buffs.Has(Buff.ManaShield) && !stats.HasMana(shieldManaBuffer))
        {
            if (manaShieldEffect != null)
            {
                manaShieldEffect.GetComponent<ParticleSystem>().enableEmission = false;
                manaShieldEffect.AddComponent<TimedLife>().Timer = 1f;
                manaShieldEffect = null;
            }
            buffs.Remove(Buff.ManaShield);
            GameObject shieldBrokenEffect = Instantiate(manaShieldBrokenPrefab);
            shieldBrokenEffect.transform.position = player.transform.position;
            shieldBrokenEffect.AddComponent<TimedLife>().Timer = 2f;
            manaShieldCooldown = shieldBrokenCooldown;
        }
    }
    public void OnDestroy()
    {
        // Removing all the stuff
        if (healEffect != null)
        {
            healEffect.GetComponent<ParticleSystem>().enableEmission = false;
            healEffect.AddComponent<TimedLife>().Timer = 1f;
        }
        if (manaShieldEffect != null)
        {
            manaShieldEffect.GetComponent<ParticleSystem>().enableEmission = false;
            manaShieldEffect.AddComponent<TimedLife>().Timer = 1f;
        }
        buffs.Remove(Buff.ManaShield);
    }

    // Event handlers
    public override bool AttackPress(Vector3 Target)
    {
        CooldownTimer = 1f / ticksPerSecond;

        if (healEffect == null)
        {
            healEffect = Instantiate(healEffectPrefab);
        }
        if (stats.Health < stats.HealthMax)
        {
            stats.HealDamage(healPerSecond / ticksPerSecond);
            return true;
        }

        return false;
    }
    public override bool AttackRelease(Vector3 Target)
    {
        if (healEffect != null)
        {
            healEffect.GetComponent<ParticleSystem>().enableEmission = false;
            healEffect.AddComponent<TimedLife>().Timer = 1f;
            healEffect = null;
        }

        return false;
    }
    public override bool AlternatePress(Vector3 Target)
    {
        CooldownTimer = 1f / ticksPerSecond;

        if (manaShieldCooldown == 0f)
        {
            if (manaShieldEffect == null)
            {
                manaShieldEffect = Instantiate(manaShieldEffectPrefab);
            }

            stats.DrainMana(0.01f * Time.deltaTime);
            buffs.Add(Buff.ManaShield);
        }

        return false;
    }
    public override bool AlternateRelease(Vector3 Target)
    {
        if (manaShieldEffect != null)
        {
            manaShieldEffect.GetComponent<ParticleSystem>().enableEmission = false;
            manaShieldEffect.AddComponent<TimedLife>().Timer = 1f;
            manaShieldEffect = null;
        }
        buffs.Remove(Buff.ManaShield);

        return false;
    }
}
using UnityEngine;
using System.Collections.Generic;

public enum UnitAlliance
{
    Neutral,
    Ally,
    Enemy,
    Player,
}

public enum ShieldsBehaviour
{
    PassiveGeneration,
    PassiveDecay,
}

public enum ShieldsRegenerationType
{
    Always,
    InCombat,
    OutOfCombatInstant,
    OutOfCombatWithDelay,
    OutOfCombatWhenNotUsed,
    Never,
}

public enum RegenerationType
{
    Always,
    WithDelay,
    WhenNotUsed,
    Never,
}

public enum CombatState
{
    In,
    Out,
}

public class UnitStats : MonoBehaviour
{
    public string debugName;
    public UnitAlliance alliance = UnitAlliance.Neutral;
    public float timeUntilOutOfCombat = 3.00f;

    public float health = 100.00f;
    public float healthMax = 100.00f;
    public float healthRegen = 1.00f;
    public RegenerationType healthRegenMode = RegenerationType.Always;
    public float healthRegenDelay = 1.00f;

    public float mana = 100.00f;
    public float manaMax = 100.00f;
    public float manaRegen = 10.00f;
    public RegenerationType manaRegenMode = RegenerationType.WhenNotUsed;
    public float manaRegenDelay = 1.00f;

    public float shields = 0.00f;
    public float shieldsMax = 1000.00f;
    public float shieldsRegen = 10.00f;
    public ShieldsBehaviour shieldsPassiveMode = ShieldsBehaviour.PassiveDecay;
    public ShieldsRegenerationType shieldsRegenMode = ShieldsRegenerationType.OutOfCombatWithDelay;
    public float shieldsRegenDelay = 3.00f;

    bool IsReallyDead = false;

    private bool healthUsedThisFrame = false;
    private bool manaUsedThisFrame = false;
    private bool shieldsAffectedThisFrame = false;
    private float healthRegenTimer = 0.00f;
    private float manaRegenTimer = 0.00f;
    private float shieldsRegenTimer = 0.00f;
    private CombatState combatState = CombatState.Out;
    private float combatTimer = 0.00f;

    [HideInInspector]
    public BuffController buffs;

    void Start()
    {
        // Add buff controller
        buffs = gameObject.AddComponent<BuffController>();
    }
    void Update()
    {
        if (IsAlive())
        {
            // Update timers
            if (healthRegenTimer > 0.00f)
            {
                healthRegenTimer -= Time.deltaTime;
                if (healthRegenTimer < 0.00f) { healthRegenTimer = 0.00f; }
            }
            if (manaRegenTimer > 0.00f)
            {
                manaRegenTimer -= Time.deltaTime;
                if (manaRegenTimer < 0.00f) { manaRegenTimer = 0.00f; }
            }
            if (shieldsRegenTimer > 0.00f)
            {
                shieldsRegenTimer -= Time.deltaTime;
                if (shieldsRegenTimer < 0.00f) { shieldsRegenTimer = 0.00f; }
            }

            // Basic health regeneration
            if (healthRegenMode == RegenerationType.Always
                || (healthRegenMode == RegenerationType.WithDelay && healthRegenTimer <= 0.00f)
                || (healthRegenMode == RegenerationType.WhenNotUsed && !healthUsedThisFrame))
            {
                HealDamage(healthRegen * Time.deltaTime);
            }
            // Basic mana regeneration
            if (manaRegenMode == RegenerationType.Always
                || (manaRegenMode == RegenerationType.WithDelay && manaRegenTimer <= 0.00f)
                || (manaRegenMode == RegenerationType.WhenNotUsed && !manaUsedThisFrame))
            {
                RestoreMana(manaRegen * Time.deltaTime);
            }
            // Shield passive mode
            if (shieldsRegenMode == ShieldsRegenerationType.Always
                || (IsInCombat() && shieldsRegenMode == ShieldsRegenerationType.InCombat)
                || (!IsInCombat() && shieldsRegenMode == ShieldsRegenerationType.OutOfCombatInstant)
                || (!IsInCombat() && shieldsRegenMode == ShieldsRegenerationType.OutOfCombatWithDelay && shieldsRegenTimer <= 0.00f)
                || (!IsInCombat() && shieldsRegenMode == ShieldsRegenerationType.OutOfCombatWhenNotUsed && !shieldsAffectedThisFrame))
            {
                if (shieldsPassiveMode == ShieldsBehaviour.PassiveGeneration)
                {
                    GainShields(shieldsRegen * Time.deltaTime);
                }
                else if (shieldsPassiveMode == ShieldsBehaviour.PassiveDecay)
                {
                    DrainShields(shieldsRegen * Time.deltaTime);
                }
            }
            // Combat mode
            if (IsInCombat())
            {
                combatTimer -= Time.deltaTime;
                if (combatTimer <= 0.00f)
                {
                    combatTimer = 0.00f;
                    OnCombatLeave();
                }
            }
        }
        healthUsedThisFrame = false;
        manaUsedThisFrame = false;
        shieldsAffectedThisFrame = false;
    }
    //===================================================================================================
    // Shields
    //===================================================================================================
    public bool HasShields(float amount, float buffer = 1.00f)
    {
        return shields >= amount + buffer;
    }
    public float DrainShields(float amount)
    {
        if (amount <= 0f)
            return 0f;

        shieldsAffectedThisFrame = true;
        float damageOverflow = 0f;
        if (HasShields(amount, 0f))
        {
            shields -= amount;
        }
        else
        {
            damageOverflow = amount - shields;
            shields = 0f;
        }
        return damageOverflow;
    }
    public void GainShields(float amount)
    {
        shields += amount;
        if (shields > shieldsMax)
        {
            shields = shieldsMax;
        }

        shieldsAffectedThisFrame = true;
        if (shieldsRegenMode == ShieldsRegenerationType.OutOfCombatWithDelay)
        {
            shieldsRegenTimer = shieldsRegenDelay;
        }
    }
    //===================================================================================================
    // Health
    //===================================================================================================
    public bool HasHealth(float amount, float buffer = 1.00f)
    {
        return health >= amount + buffer;
    }
    public void DealDamage(float amount, GameObject source = null)
    {
        if (IsDead() || amount <= 0.00f)
            return;

        // Damage taken - we are in combat
        EngageCombat();

        // Deduce health
        float overflow = amount;
        if (buffs.Has(Buff.ManaShield)) { overflow = DrainMana(overflow); }
        overflow = DrainShields(overflow);
        health -= overflow;
        if (health <= 0.00f)
        {
            Kill();
        }
        else
        {
            // Enemy AI on hit
            EnemyAI enemyController = GetComponent<EnemyAI>();
            if (enemyController != null) { enemyController.OnHit(amount, source); }
        }

        // Regeneration delays
        if (healthRegenMode == RegenerationType.WhenNotUsed)
        {
            healthUsedThisFrame = true;
        }
        else if (healthRegenMode == RegenerationType.WithDelay)
        {
            healthRegenTimer = healthRegenDelay;
        }
    }
    public void HealDamage(float amount)
    {
        health += amount;
        health = Mathf.Min(health, healthMax);
    }
    //===================================================================================================
    // Mana
    //===================================================================================================
    public bool HasMana(float amount) { return mana >= amount; }
    public float DrainMana(float amount)
    {
        if (amount <= 0f)
            return 0f;

        float overflow = 0f;
        if (HasMana(amount))
        {
            mana -= amount;
        }
        else
        {
            overflow = amount - mana;
            mana = 0f;
        }

        // Regeneration delays
        if (manaRegenMode == RegenerationType.WhenNotUsed) { manaUsedThisFrame = true; }
        else if (manaRegenMode == RegenerationType.WithDelay) { manaRegenTimer = manaRegenDelay; }

        return overflow;
    }
    public void RestoreMana(float amount)
    {
        mana += amount;
        mana = Mathf.Min(mana, manaMax);
    }
    //===================================================================================================
    // Is alive
    //===================================================================================================
    public bool IsAlive() { return health > 0f; }
    public bool IsDead() { return !IsAlive(); }
    //===================================================================================================
    // Death
    //===================================================================================================
    public virtual void Kill()
    {
        if (IsReallyDead)
            return;

        // Set health
        health = 0.00f;
        IsReallyDead = true;
        if (tag != "Player")
        {
            // Destroy movement agents
            UnityEngine.AI.NavMeshAgent Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (Agent != null)
                Destroy(Agent);
            // Destroy animation agent
            Animator Anim = GetComponent<Animator>();
            if (Anim != null)
                Destroy(Anim);

            // Set ragdoll properties
            /*Rigidbody Body = GetComponent<Rigidbody>();
            Body.drag = 0.0f;
            Body.mass *= 2.50f;*/

            // Enable death script
            UnitDeath Death = GetComponent<UnitDeath>();
            if (Death != null)
                Death.OnDeath();

            // Enable corpse script
            CorpseCleanUp Corpse = gameObject.AddComponent<CorpseCleanUp>();
            Corpse.SetTimer(15.00f, 0.75f);
        }
    }
    //===================================================================================================
    // Physics
    //===================================================================================================
    public void ApplyForce(Vector3 force, ForceMode mode)
    {
        // Only works on death ragdoll
        if (IsAlive())
            return;
        UnitDeath Death = GetComponent<UnitDeath>();
        if (Death != null)
            Death.ApplyForce(force, mode);
    }
    //===================================================================================================
    // Combat
    //===================================================================================================
    public bool IsInCombat() { return combatState == CombatState.In; }
    public void EngageCombat()
    {
        if (!IsInCombat())
        {
            OnCombatEnter();
        }
        combatTimer = timeUntilOutOfCombat;
    }
    public virtual void OnCombatEnter() { combatState = CombatState.In; }
    public virtual void OnCombatLeave() { combatState = CombatState.Out; }
}

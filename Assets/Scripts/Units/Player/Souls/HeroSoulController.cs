using Units;
using UnityEngine;

public class HeroSoulController : MonoBehaviour
{
    UnitStats stats;
    HeroSoulOffensive_Old offensiveSoul;
    HeroSoulDefensive defensiveSoul;
	void Start()
    {
        stats = GetComponent<UnitStats>();
	}
	void Update ()
    {
        // Init some variables
        bool AttackSuccessful = false;
        bool IsAlternate = false;

        // Only if something is equipped
        if (offensiveSoul != null)
        {
            //=================================================================================
            // Check offensive soul cooldown
            //=================================================================================
            if (offensiveSoul.CooldownTimer > 0.00f)
            {
                offensiveSoul.CooldownTimer -= Time.deltaTime;
                if (offensiveSoul.CooldownTimer <= 0.00f)
                {
                    offensiveSoul.CooldownTimer = 0.00f;
                }
            }
            //=================================================================================
            // Offensive soul on-click
            //=================================================================================
            // Find the mouse position
            Vector3 Target = Utility.GetMouseWorldPosition(transform.position);
            // Initiate the attack

            // Main attack
            if ((Input.GetMouseButtonDown(0) || (offensiveSoul.AllowRepeatOnButtonHold && Input.GetMouseButton(0) == true))
                && !offensiveSoul.IsOnCooldown()
                && stats.HasHealth(offensiveSoul.GetHealthCost()) && stats.HasMana(offensiveSoul.GetManaCost()))
            {
                IsAlternate = false;
                AttackSuccessful = offensiveSoul.AttackPress(Target);
            }

            // Alternate attack
            if ((Input.GetMouseButtonDown(1) || (offensiveSoul.AllowAltRepeatOnButtonHold && Input.GetMouseButton(1) == true))
                && !offensiveSoul.IsOnCooldown()
                && stats.HasHealth(offensiveSoul.GetAltHealthCost()) && stats.HasMana(offensiveSoul.GetAltManaCost()))
            {
                IsAlternate = true;
                AttackSuccessful = offensiveSoul.AlternatePress(Target);
            }

            // Release main
            if (Input.GetMouseButtonUp(0))
            {
                IsAlternate = false;
                AttackSuccessful = offensiveSoul.AttackRelease(Target);
            }

            // Release alternate
            if (Input.GetMouseButtonUp(1))
            {
                IsAlternate = true;
                AttackSuccessful = offensiveSoul.AlternateRelease(Target);
            }

            // Deduce costs if successful
            if (AttackSuccessful)
            {
                if (!offensiveSoul.IsSupport()) { stats.EngageCombat(); }
                // Substract health cost
                stats.DealDamage(!IsAlternate ? offensiveSoul.GetHealthCost() : offensiveSoul.GetAltHealthCost());
                stats.DrainMana(!IsAlternate ? offensiveSoul.GetManaCost() : offensiveSoul.GetAltManaCost());
            }
        }
    }

    public void UpdateSouls(HeroSoulOffensive_Old offensive, HeroSoulDefensive defensive)
    {
        offensiveSoul = offensive;
        defensiveSoul = defensive;
    }
}

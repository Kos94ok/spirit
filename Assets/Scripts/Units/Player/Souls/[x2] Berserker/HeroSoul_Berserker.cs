using UnityEngine;
using System.Collections;
using System;

public class HeroSoul_Berserker : HeroSoulOffensive_Old
{
    const float SwordDamage = 75.00f;
    const float AlternateDamage = 75.00f;
    const float AttackArc = 270.00f;
    const float ImpactPower = 3.00f;
    const float FinalAttackRange = 2.00f;
    const float Cooldown = 0.45f;
    const float BigCooldown = 0.75f;
    const float ManaCost = 15.00f;

    private float SwingResetTimer;
    private GameObject SwordPrefab;
    private int SwingNumber = 0;
    private bool SwingReversed = false;

    // API functions
    public override float GetManaCost() { return ManaCost; }

    // Basic behaviours
    public override void Start()
    {
        base.Start();
        CooldownTimer = BigCooldown;
        SwordPrefab = Resources.Load("HeroSoulBerserkerProjectile") as GameObject;
        AllowRepeatOnButtonHold = true;
        AllowAltRepeatOnButtonHold = true;
	}
    public override void Update()
    {
        base.Update();
        // Update swing reset timer
        if (SwingResetTimer > 0.00f)
        {
            SwingResetTimer -= Time.deltaTime;
            if (SwingResetTimer <= 0.00f)
            {
                SwingResetTimer = 0.00f;
                SwingNumber = 0;
                SwingReversed = !SwingReversed;
            }
        }
    }

    // Event handlers
    public override bool AttackPress(Vector3 Target)
    {
        // Choosing sword count
        int SwordCount = 1;
        if (SwingNumber == 2) { SwordCount = 2; }
        // Going into loop
        for (int i = 0; i < SwordCount; i++)
        {
            // Creating a sword
            GameObject Sword = Instantiate(SwordPrefab, transform.position, new Quaternion()) as GameObject;
            Sword.transform.Translate(new Vector3(0, 0.15f, 0), Space.World);
            // Setting stats
            HeroSoul_Berserker_Projectile Script = Sword.GetComponent<HeroSoul_Berserker_Projectile>();
            Script.SwordDamage = SwordDamage;
            Script.AttackArc = AttackArc;
            Script.ImpactPower = ImpactPower;
            Script.LastAttackRange = FinalAttackRange;
            Script.IsAlternate = false;
            // Initial rotation
            float AngleToTarget = Vector3.Angle(Target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
            var Cross = Vector3.Cross(Target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
            if (Cross.y < 0)
                AngleToTarget = -AngleToTarget;
            Sword.transform.Rotate(new Vector3(0, -1, 0), AngleToTarget + 180);
            // Swing number
            if ((SwingNumber == 0 && !SwingReversed) || (SwingNumber == 1 && SwingReversed) || (SwingNumber == 2 && i == 0))
            {
                Sword.transform.Rotate(new Vector3(0, -1, 0), AttackArc / 2.00f);
                Sword.GetComponent<HeroSoul_Berserker_Projectile>().SetRotationAxis(transform.position, 1);
                SwingNumber += 1;
                Script.SwingDirection = 0;
            }
            else if ((SwingNumber == 1 && !SwingReversed) || (SwingNumber == 0 && SwingReversed) || (SwingNumber == 3 && i == 1))
            {
                Sword.transform.Rotate(new Vector3(0, 1, 0), AttackArc / 2.00f);
                Sword.GetComponent<HeroSoul_Berserker_Projectile>().SetRotationAxis(transform.position, -1);
                SwingNumber += 1;
                Script.SwingDirection = 1;
            }
            // Moving forward
            Sword.transform.Translate(new Vector3(-0.5f, 0, 0), Space.Self);
        }

        // Initiating cooldowns
        // After first and second swing
        if (SwingNumber == 1 || SwingNumber == 2)
        {
            CooldownTimer = Cooldown;
            SwingResetTimer = 1.00f;
        }
        // Final countdown
        if (SwingNumber == 4)
        {
            // Initiating cooldown
            CooldownTimer = BigCooldown;
            SwingReversed = !SwingReversed;

            // Swing number
            SwingNumber = 0;
            SwingResetTimer = 0.00f;
        }
        return true;
    }
    public override bool AttackRelease(Vector3 Target)
    {
        return false;
    }
    public override bool AlternatePress(Vector3 Target)
    {
        for (int i = -SwingNumber; i <= SwingNumber; i++)
        {
            // Creating a sword
            GameObject Sword = Instantiate(SwordPrefab, transform.position, new Quaternion()) as GameObject;
            Sword.transform.Translate(new Vector3(0, 0.15f, 0), Space.World);
            // Setting stats
            HeroSoul_Berserker_Projectile Script = Sword.GetComponent<HeroSoul_Berserker_Projectile>();
            Script.SwordDamage = AlternateDamage;
            Script.AttackArc = AttackArc;
            Script.ImpactPower = ImpactPower;
            Script.LastAttackRange = FinalAttackRange;
            Script.IsAlternate = true;
            // Initial rotation
            float AngleToTarget = Vector3.Angle(Target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
            var Cross = Vector3.Cross(Target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
            if (Cross.y < 0)
                AngleToTarget = -AngleToTarget;
            Sword.transform.Rotate(new Vector3(0, -1, 0), AngleToTarget + 180 + i * 30.00f);
            // Moving forward
            //Sword.transform.Translate(new Vector3(-0.5f, 0, 0), Space.Self);
        }
        if (SwingNumber == 0 || SwingNumber == 1)
        {
            SwingNumber += 1;
            CooldownTimer = Cooldown;
            SwingResetTimer = BigCooldown;
        }
        else
        {
            SwingNumber = 0;
            CooldownTimer = BigCooldown;
            SwingResetTimer = 0.00f;
            SwingReversed = !SwingReversed;
        }
        return true;
    }
    public override bool AlternateRelease(Vector3 Target)
    {
        return false;
    }
}

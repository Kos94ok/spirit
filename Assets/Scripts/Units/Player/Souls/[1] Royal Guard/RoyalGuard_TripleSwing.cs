using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGuard_TripleSwing : HeroAbility {
    const float swordDamage = 75.00f;
    const float alternateDamage = 75.00f;
    const float attackArc = 270.00f;
    const float impactPower = 3.00f;
    const float finalAttackRange = 2.00f;
    const float cooldown = 0.45f;
    const float bigCooldown = 0.75f;
    const float manaCost = 15.00f;

    private float swingResetTimer;
    private GameObject swordPrefab;
    private int swingNumber = 0;
    private bool swingReversed = false;

    public new void Start() {
        base.Start();
        SetCooldown(bigCooldown);
        swordPrefab = Resources.Load("HeroSoulBerserkerProjectile") as GameObject;
    }

    public override AbilityType GetAbilityType() {
        return AbilityType.Hold;
    }

    public override void OnTrigger() {
        // Choosing sword count
//        int SwordCount = 1;
//        if (swingNumber == 2) { SwordCount = 2; }
//        // Going into loop
//        for (int i = 0; i < SwordCount; i++) {
//            // Creating a sword
//            GameObject Sword = Instantiate(swordPrefab, transform.position, new Quaternion()) as GameObject;
//            Sword.transform.Translate(new Vector3(0, 0.15f, 0), Space.World);
//            // Setting stats
//            HeroSoul_Berserker_Projectile Script = Sword.GetComponent<HeroSoul_Berserker_Projectile>();
//            Script.SwordDamage = swordDamage;
//            Script.AttackArc = attackArc;
//            Script.ImpactPower = impactPower;
//            Script.LastAttackRange = finalAttackRange;
//            Script.IsAlternate = false;
//            // Initial rotation
//            float AngleToTarget = Vector3.Angle(target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
//            var Cross = Vector3.Cross(target - transform.position, new Vector3(1.00f, 0.00f, 0.00f));
//            if (Cross.y < 0)
//                AngleToTarget = -AngleToTarget;
//            Sword.transform.Rotate(new Vector3(0, -1, 0), AngleToTarget + 180);
//            // Swing number
//            if ((swingNumber == 0 && !swingReversed) || (swingNumber == 1 && swingReversed) || (swingNumber == 2 && i == 0)) {
//                Sword.transform.Rotate(new Vector3(0, -1, 0), attackArc / 2.00f);
//                Sword.GetComponent<HeroSoul_Berserker_Projectile>().SetRotationAxis(transform.position, 1);
//                swingNumber += 1;
//                Script.SwingDirection = 0;
//            }
//            else if ((swingNumber == 1 && !swingReversed) || (swingNumber == 0 && swingReversed) || (swingNumber == 3 && i == 1)) {
//                Sword.transform.Rotate(new Vector3(0, 1, 0), attackArc / 2.00f);
//                Sword.GetComponent<HeroSoul_Berserker_Projectile>().SetRotationAxis(transform.position, -1);
//                swingNumber += 1;
//                Script.SwingDirection = 1;
//            }
//            // Moving forward
//            Sword.transform.Translate(new Vector3(-0.5f, 0, 0), Space.Self);
//        }
//
//        // Initiating cooldowns
//        // After first and second swing
//        if (swingNumber == 1 || swingNumber == 2) {
//            SetCooldown(cooldown);
//            swingResetTimer = 1.00f;
//        }
//        // Final countdown
//        if (swingNumber == 4) {
//            // Initiating cooldown
//            SetCooldown(bigCooldown);
//            swingReversed = !swingReversed;
//
//            // Swing number
//            swingNumber = 0;
//            swingResetTimer = 0.00f;
//        }
    }
}
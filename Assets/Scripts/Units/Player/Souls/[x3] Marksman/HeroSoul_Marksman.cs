using UnityEngine;
using System.Collections;

public class HeroSoul_Marksman : HeroSoulOffensive_Old
{
    const float arrowDamage = 50f;
    const float arrowLifetime = 2f;
    const float drawDelay = 0.50f;
    const float bowHoldingDistance = 0.30f;
    const float arrowTravelSpeed = 12.00f;
    const float manaCost = 10f;

    const float altArrowDamage = 8f;
    const float altArrowLifetime = 0.5f;
    const int altArrowCount = 12;
    const float altConeAngle = 45.00f;
    const float manaCostAlternate = 40f;

    float bowRealAngle;
    float bowTargetAngle;

    bool isHoldingMain = false;
    float drawTimer = 0.00f;
    float bowDecayTimer = 0.00f;

    GameObject visualBow;
    GameObject preparedArrow;

    GameObject arrowPrefab;
    GameObject visualBowPrefab;

    // API functions
    public override float GetManaCost() { return manaCost; }
    public override float GetAltManaCost() { return manaCostAlternate; }

    // Basic behaviours
    public override void Start()
    {
        base.Start();
        arrowPrefab = Resources.Load("HeroSoulMarksmanArrow") as GameObject;
        visualBowPrefab = Resources.Load("HeroSoulMarksmanBow") as GameObject;
    }
    public override void Update()
    {
        base.Update();

        if (visualBow != null)
        {
            // Draw delay
            if (drawTimer > 0f && isHoldingMain)
            {
                drawTimer -= Time.deltaTime;
                if (drawTimer <= 0f)
                {
                    drawTimer = 0f;
                    visualBow.GetComponent<HeroSoul_Marksman_Bow>().Draw();
                    preparedArrow = Instantiate(arrowPrefab);
                }
            }
            // Mana drain
            else if (drawTimer == 0f && isHoldingMain)
            {
                stats.DrainMana(0.01f * Time.deltaTime);
            }

            // Position and rotation
            Vector3 mousePos = Utility.GetMouseWorldPosition(player.transform.position);
            bowTargetAngle = Math.GetAngleRaw(player.transform.position, mousePos);
            float bowRotationDifference = Math.UnityRotationToGeneralRotation(visualBow.transform.rotation.eulerAngles.y) - bowTargetAngle;

            visualBow.transform.position = Math.PolarVector2D(player.transform.position, bowRealAngle, bowHoldingDistance);
            visualBow.transform.Rotate(Vector3.up, bowRotationDifference);

            if (preparedArrow != null)
            {
                float arrowRotationDifference = Math.UnityRotationToGeneralRotation(preparedArrow.transform.rotation.eulerAngles.y) - bowTargetAngle;
                preparedArrow.transform.position = Math.PolarVector2D(player.transform.position, bowRealAngle, bowHoldingDistance);
                preparedArrow.transform.Rotate(Vector3.up, arrowRotationDifference);
            }

            bowRealAngle += Math.GetAngleDifference(bowRealAngle, bowTargetAngle) * 10.00f * Time.deltaTime;
            if (bowRealAngle > 360.00f) { bowRealAngle -= 360.00f; }
            if (bowRealAngle < -360.00f) { bowRealAngle += 360.00f; }


            // Some stuff concerning vertical angles and quaternions.
            if (true == false)
            {
                /*Vector3 playerGroundPos = Utility.GetGroundPosition(player.transform.position);

                Vector3 mousePos = Utility.GetMouseWorldPosition(player.transform.position);
                //mousePos.y = Utility.GetGroundHeight(mousePos);
                //if (mousePos.y == 0.00f) { mousePos.y = playerGroundPos.y; }

                bowTargetAngle = Math.GetAngleRaw(player.transform.position, mousePos);
                float bowRotationDifference = Math.UnityRotationToGeneralRotation(preparedArrow.transform.rotation.eulerAngles.y) - bowTargetAngle;

                visualBow.transform.position = Math.PolarVector2D(player.transform.position, bowRealAngle, bowHoldingDistance);

                Debug.Log(mousePos.y);
                Quaternion rotation = Quaternion.LookRotation(mousePos - playerGroundPos);
                visualBow.transform.rotation = rotation;
                visualBow.transform.Rotate(Vector3.up, -90.00f);

                //visualBow.transform.Rotate(Vector3.up, bowRotationDifference);
                //visualBow.transform.Rotate(Vector3.right, )

                preparedArrow.transform.position = Math.PolarVector2D(player.transform.position, bowRealAngle, bowHoldingDistance);
                //preparedArrow.transform.Rotate(Vector3.up, bowRotationDifference);
                preparedArrow.transform.rotation = rotation;
                preparedArrow.transform.Rotate(Vector3.up, -90.00f);

                bowRealAngle += Math.GetAngleDifference(bowRealAngle, bowTargetAngle) * 10.00f * Time.deltaTime;
                if (bowRealAngle > 360.00f) { bowRealAngle -= 360.00f; }
                if (bowRealAngle < -360.00f) { bowRealAngle += 360.00f; }*/
            }
        }
    }

    void OnDestroy()
    {
        if (visualBow != null) { visualBow.GetComponent<HeroSoul_Marksman_Bow>().Kill(); }
        if (preparedArrow != null) { preparedArrow.GetComponent<HeroSoul_Marksman_Arrow>().KillProjectile(); }
        buffs.Remove(Buff.DrawingBow);
    }

    // Event handlers
    public override bool AttackPress(Vector3 Target)
    {
        isHoldingMain = true;
        CreateVisualBow();
        drawTimer = drawDelay;
        buffs.Add(Buff.DrawingBow);

        return false;
    }
    public override bool AttackRelease(Vector3 Target)
    {
        bool drainMana = false;

        isHoldingMain = false;
        if (visualBow != null)
        {
            if (drawTimer == 0f)
            {
                preparedArrow.GetComponent<HeroSoul_Marksman_Arrow>().Launch(arrowDamage, arrowTravelSpeed, bowTargetAngle, 0f, arrowLifetime);
                preparedArrow = null;
                drainMana = true;
            }

            visualBow.GetComponent<HeroSoul_Marksman_Bow>().Kill();
        }
        buffs.Remove(Buff.DrawingBow);
        return drainMana;
    }
    public override bool AlternatePress(Vector3 Target)
    {
        float angleToTarget = Math.GetAngleRaw(player.transform.position, Target);
        CreateVisualBow();
        visualBow.GetComponent<HeroSoul_Marksman_Bow>().InstaFade();
        visualBow.GetComponent<HeroSoul_Marksman_Bow>().Kill();

        for (int i = -altArrowCount / 2; i < altArrowCount / 2 + (altArrowCount % 2 != 0 ? 1 : 0); i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            float localTargetAngle = angleToTarget + i * (altConeAngle / altArrowCount);
            if (altArrowCount % 2 == 0) { localTargetAngle += altConeAngle / altArrowCount / 2f; }
            float arrowRotationDifference = Math.UnityRotationToGeneralRotation(arrow.transform.rotation.eulerAngles.y) - localTargetAngle;
            arrow.transform.position = Math.PolarVector2D(player.transform.position, angleToTarget, bowHoldingDistance);
            arrow.transform.Rotate(Vector3.up, arrowRotationDifference);
            arrow.GetComponent<HeroSoul_Marksman_Arrow>().Launch(altArrowDamage, arrowTravelSpeed, localTargetAngle, 0f, altArrowLifetime);
        }

        return true;
    }
    public override bool AlternateRelease(Vector3 Target)
    {
        return false;
    }

    void CreateVisualBow()
    {
        Vector3 target = Utility.GetMouseWorldPosition(player.transform.position);
        if (visualBow == null)
        {
            visualBow = Instantiate(visualBowPrefab);
            bowTargetAngle = Math.GetAngleRaw(player.transform.position, target);
            bowRealAngle = bowTargetAngle;
        }
        else
        {
            visualBow.GetComponent<HeroSoul_Marksman_Bow>().Ressurect();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using Units.Enemies;

public class EternalGuardianAI : EnemyAI
{
    public float bladeDamageOnImpact;
    public float bladeDamagePerSecond;
    public float bladeOrbitDistance;
    public float combatEngageRange;
    public float combatDisengageRange;

    public float bladeAngleBasic;
    public float bladeAngleCombat;
    public float rotationSpeedBasic;
    public float rotationSpeedCombat;

    public float enrageSpeedMod;
    public float enrageAccelMod;
    public float enrageDelay;
    public float enrageDuration;

    bool inCombat = false;
    float bladeAngle = 90.00f;
    float rotationSpeed = 25.00f;

    float bladesRotation = 0.00f;
    float bladesOffsetToCenter = 0.00f;

    float bladeAngleDelta;
    float rotationSpeedDelta;

    float movementSpeed;
    float movementAccel;

    float enrageCounter = 0.00f;
    float enrageDelayTimer = 0.00f;

    float staggerTimer = 0.00f;
    Vector3 staggerHoldVelocity;

    GameObject player;
    UnityEngine.AI.NavMeshAgent agent;
    GameObject body;
    List<GameObject> blades = new List<GameObject>();
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Not a good solution, better to generalize later
        body = transform.GetChild(0).gameObject;
        blades.Add(transform.GetChild(1).gameObject);
        blades.Add(transform.GetChild(2).gameObject);
        blades.Add(transform.GetChild(3).gameObject);
        for (int i = 0; i < blades.Count; i++)
        {
            blades[i].transform.parent = blades[i].transform.parent.parent;
            blades[i].GetComponentInChildren<EternalGuardianAttackHitbox>().SetDamage(bladeDamageOnImpact, bladeDamagePerSecond);
        }

        bladeAngle = bladeAngleBasic;
        rotationSpeed = rotationSpeedBasic;
        bladeAngleDelta = ((bladeAngleCombat - bladeAngleBasic) / bladeAngleCombat) * 5.00f;
        rotationSpeedDelta = ((rotationSpeedCombat - rotationSpeedBasic) / rotationSpeedCombat) * 20.00f;

        movementSpeed = agent.speed;
        movementAccel = agent.acceleration;
    }

    public override void OnHit(float damage, GameObject source)
    {
        //staggerTimer = 0.15f;

        /*GetComponentInChildren<ParticleSystem>().Pause();
        for (int i = 0; i < blades.Count; i++)
        {
            blades[i].GetComponentInChildren<ParticleSystem>().Pause();
        }
        staggerHoldVelocity = agent.velocity;
        agent.velocity = Vector3.zero;
        agent.speed = 0.00f;
        agent.acceleration = 50.00f;*/

        //agent.velocity = (agent.transform.position - player.transform.position).normalized * 1.00f;
        agent.velocity *= Mathf.Max(0.50f, 1f - (damage / 150f));
        rotationSpeed *= Mathf.Max(0.80f, 1f - (damage / 95f));
    }

    void Update ()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Stagger update
        if (staggerTimer > 0.00f)
        {
            /*staggerTimer -= Time.deltaTime;
            if (staggerTimer < 0.00f)
            {
                staggerTimer = 0.00f;

                GetComponentInChildren<ParticleSystem>().Play();
                for (int i = 0; i < blades.Count; i++)
                {
                    blades[i].GetComponentInChildren<ParticleSystem>().Play();
                }
                agent.velocity = staggerHoldVelocity;
                agent.speed = movementSpeed;
                agent.acceleration = movementAccel;
            }
            return;*/
        }

        // Combat mode
        if ((distanceToPlayer <= combatEngageRange && !inCombat) || (distanceToPlayer < combatDisengageRange && inCombat))
        {
            agent.SetDestination(player.transform.position);

            // Initiate combat
            if (!inCombat)
            {
                inCombat = true;
                if (enrageDuration > 0.00f)
                {
                    enrageDelayTimer = enrageDelay;
                } 
            }
            // Else, check enrage timers
            else
            {
                // Enrage delay
                if (enrageDelayTimer > 0.00f)
                {
                    enrageDelayTimer -= Time.deltaTime;
                    if (enrageDelayTimer < 0.00f)
                    {
                        enrageDelayTimer = 0.00f;
                        enrageCounter = 0.00f;
                    }
                }
                // Enrage started
                else if (enrageDuration > 0.00f)
                {
                    enrageCounter += Time.deltaTime;
                    if (enrageCounter > enrageDuration) { enrageCounter = enrageDuration; }

                    agent.acceleration = movementAccel + movementAccel * (enrageAccelMod * (enrageCounter / enrageDuration));
                    agent.speed = movementSpeed + movementSpeed * (enrageSpeedMod * (enrageCounter / enrageDuration));
                }
            }


            // Move to combat mode, if necessary
            if (bladeAngle > bladeAngleCombat) { bladeAngle += bladeAngleDelta * Time.deltaTime; }
            if (rotationSpeed < rotationSpeedCombat) { rotationSpeed += rotationSpeedDelta * Time.deltaTime; }
            // If player is too close, pull in the blades
            if (Math.GetDistance2D(agent.transform.position, player.transform.position) < 0.65f)
            {
                bladesOffsetToCenter += 1.00f * Time.deltaTime;
                if (bladesOffsetToCenter > 0.59f) { bladesOffsetToCenter = 0.59f; }
            }
            else if (bladesOffsetToCenter > 0.00f)
            {
                bladesOffsetToCenter -= 1.00f * Time.deltaTime;
                if (bladesOffsetToCenter < 0.00f) { bladesOffsetToCenter = 0.00f; }
            }
        }
        else if (distanceToPlayer > combatDisengageRange)
        {
            agent.SetDestination(agent.transform.position);
            inCombat = false;
            agent.speed = movementSpeed;
            agent.acceleration = movementAccel;

            if (bladeAngle < bladeAngleBasic) { bladeAngle -= bladeAngleDelta * Time.deltaTime; }
            if (rotationSpeed > rotationSpeedBasic) { rotationSpeed -= rotationSpeedDelta * Time.deltaTime; }
        }

        // Core rotation
        body.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);

        // Blades routine
        bladesRotation += rotationSpeed * 1.00f * Time.deltaTime;
        if (bladesRotation >= 360.00f) { bladesRotation -= 360.00f; }
        for (int i = 0; i < blades.Count; i++)
        {
            // Blades position
            Vector3 bladePos = Math.PolarVector2D(agent.transform.position, -120.00f * i - bladesRotation - bladeAngle, bladeOrbitDistance - bladesOffsetToCenter);
            blades[i].transform.position = bladePos;

            // Blades rotation
            blades[i].transform.Rotate(Vector3.up, rotationSpeed * 1.00f * Time.deltaTime);
        }
	}
    public List<GameObject> AskForBlades()
    {
        return blades;
    }
}

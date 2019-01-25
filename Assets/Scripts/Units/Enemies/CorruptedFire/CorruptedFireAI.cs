using UnityEngine;
using System.Collections.Generic;

public class CorruptedFireAI : MonoBehaviour
{
    public float contactDamage;
    public float chargeDamage;
    public float contactDamageRadius;
    public float chargeDamageRadius;

    public float chargeRange;
    public float chargeDelay;
    public float chargeDuration;
    public float chargeCooldown;
    public float chargeSpeed;

    public float combatEngageRange;
    public float combatDisengageRange;

    bool isChasing = false;
    bool charging = false;

    float movementSpeed;
    float movementAccel;
    float movementAngularSpeed;

    float chargeDelayTimer = 0.00f;
    float chargeDurationTimer = 0.00f;
    float chargeCooldownTimer = 0.00f;
    Vector3 chargeTargetPoint;
    GameObject chargeCastPE;
    List<GameObject> chargeHitObjectsList = new List<GameObject>();

    GameObject player;
    UnityEngine.AI.NavMeshAgent agent;
	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = true;

        movementSpeed = agent.speed;
        movementAccel = agent.acceleration;
        movementAngularSpeed = agent.angularSpeed;
        
	}
    void Update()
    {
        // Data update
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (agent == null)
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Objects not found
        if (player == null || agent == null)
            return;

        // Movement
        float Distance = Vector3.Distance(player.transform.position, transform.position);
        if ((Distance <= combatEngageRange && !isChasing) || (Distance < combatDisengageRange && isChasing) || charging)
        {
            isChasing = true;
            // If not charging - follow normally
            if (charging == false)
            {
                agent.SetDestination(player.transform.position);
            }

            // Check for contact damage
            if (Math.GetDistance2D(player.transform.position, agent.transform.position) <= contactDamageRadius)
            {
                player.GetComponent<UnitStats>().DealDamage(contactDamage * Time.deltaTime, gameObject);
                GameObject playerHitPE = Instantiate(Resources.Load("CorruptedFirePEOnPlayerHitSmall")) as GameObject;
                playerHitPE.transform.position = player.transform.position;
            }

            // Charge initialize
            if (Distance <= chargeRange && charging == false && chargeCooldownTimer == 0.00f && !Utility.IsTargetObstructed(agent.transform.position, player.transform.position))
            {
                charging = true;
                agent.acceleration = 1000.00f;
                agent.angularSpeed = 1000.00f;
                agent.SetDestination(agent.transform.position);
                //agent.Stop();
                chargeDelayTimer = chargeDelay;
                ResetHitObjectsList();

                chargeCastPE = Instantiate(Resources.Load("CorruptedFirePEChargeCast")) as GameObject;
                chargeCastPE.transform.position = agent.transform.GetChild(0).position;
            }

            
            // Charge prepare
            else if (charging == true && chargeDelayTimer > 0.00f)
            {
                chargeDelayTimer -= Time.deltaTime;
                if (agent.acceleration > 0.00f) { agent.acceleration = 0.00f; }

                if (Utility.IsTargetObstructed(agent.transform.position, player.transform.position))
                {
                    chargeDelayTimer = 0.00f;
                    StopAllCharge();
                    chargeCastPE.GetComponent<ParticleSystem>().enableEmission = false;
                }
                else if (chargeDelayTimer <= 0.00f)
                {
                    chargeDelayTimer = 0.00f;
                    // Chaaaaaaaaaaaaarge
                    agent.acceleration = 0.00f;
                    agent.angularSpeed = 0.0f;
                    agent.speed = 0.00f;

                    chargeTargetPoint = player.transform.position + (player.transform.position - agent.transform.position).normalized * 3.00f;

                    agent.SetDestination(chargeTargetPoint);

                    chargeDurationTimer = chargeDuration;
                }
            }

            // Charge in progress
            else if (charging == true && chargeDelayTimer == 0.00f && chargeDurationTimer > 0.00f)
            {
                chargeDurationTimer -= Time.deltaTime;
                Vector3 oldPos = agent.transform.position;

                // Move the unit
                agent.Move((chargeTargetPoint - agent.transform.position).normalized * chargeSpeed * Time.deltaTime);

                // Check for collision
                if (!chargeHitObjectsList.Contains(player) && Math.GetDistance2D(agent.transform.position, player.transform.position) <= chargeDamageRadius)
                {
                    player.GetComponent<UnitStats>().DealDamage(chargeDamage, gameObject);
                    chargeHitObjectsList.Add(player);
                    GameObject playerBlood = Instantiate(Resources.Load("CorruptedFirePEOnPlayerHit")) as GameObject;
                    playerBlood.transform.position = player.transform.position;

                    // Please don't ask me why it has to be like that...
                    playerBlood.transform.RotateAround(Vector3.down, (180 + Math.GetAngle(player.transform.position, chargeTargetPoint)) * Mathf.Deg2Rad);
                }

                // Check for stop conditions
                if (chargeDurationTimer <= 0.00f || (Vector3.Distance(oldPos, agent.transform.position) < (chargeSpeed * Time.deltaTime) * 0.50f))
                {
                    chargeDurationTimer = 0.00f;
                    StopAllCharge();
                }
            }

            // Charge in cooldown
            else if (chargeCooldownTimer > 0.00f)
            {
                chargeCooldownTimer -= Time.deltaTime;
                if (chargeCooldownTimer < 0.00f) { chargeCooldownTimer = 0.00f; }
            }
        }
        else if (Distance >= combatDisengageRange)
        {
            isChasing = false;
        }

	}

    

    void StopAllCharge()
    {
        charging = false;
        chargeDurationTimer = 0.00f;
        chargeCooldownTimer = chargeCooldown;

        agent.speed = movementSpeed;
        agent.acceleration = movementAccel;
        agent.angularSpeed = movementAngularSpeed;
    }

    void ResetHitObjectsList()
    {
        chargeHitObjectsList.Clear();
    }
}

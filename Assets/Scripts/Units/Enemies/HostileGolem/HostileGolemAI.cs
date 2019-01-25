using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostileGolemAI : MonoBehaviour
{
    const float chaseRange = 5.00f;
    const float attackRange = 4.00f;
    const float attackBaseTime = 1.50f;

    bool attacking = false;
    float attackTimer;
    bool attackImpactDone = false;

    float movementSpeed;
    float movementAccel;

    GameObject Player;
    UnityEngine.AI.NavMeshAgent Agent;
    Animator Anim;
	void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Agent.updateRotation = true;
        Anim = GetComponent<Animator>();

        movementSpeed = Agent.speed;
        movementAccel = Agent.acceleration;
	}
	void Update()
    {
        // Data update
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
        if (Agent == null)
            Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (Anim == null)
            Anim = GetComponent<Animator>();

        // Objects not found
        if (Player == null || Agent == null || Anim == null)
            return;

        // Freeze until birth completed
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Birth") == false)
        {
            // Movement
            Anim.SetBool("Attacking", false);
            float Distance = Vector3.Distance(Player.transform.position, transform.position);
            if (Distance <= chaseRange)
            {
                Agent.SetDestination(Player.transform.position);
                float desiredAngle = Quaternion.LookRotation(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z) - transform.position).eulerAngles.y;
                float angleDifference = Mathf.Abs(transform.rotation.eulerAngles.y - desiredAngle);
                //Debug.Log(attacking.ToString() + " / " + Distance.ToString());
                // Attacking
                if (Distance <= attackRange && attacking == false && angleDifference <= 15.00f)
                {
                    attacking = true;
                    Agent.acceleration = 1000.00f;
                    Agent.SetDestination(Agent.transform.position);
                    Agent.Stop();
                    Anim.SetBool("Attacking", true);
                    attackTimer = 0.00f;
                    ResetHitObjectsList();
                    Agent.updateRotation = false;
                    transform.rotation = Quaternion.LookRotation(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z) - transform.position);
                }
                /*else if (Distance <= 0.75f && attacking == false && angleDifference > 15.00f)
                {
                    //float angleToRotate = 45.00f * Time.deltaTime;
                    //if (transform.rotation.eulerAngles.y > desiredAngle)
                    //transform.Rotate(Vector3.up)
                }*/
                else if (attacking == true && Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false && attackTimer >= 0.50f)
                {
                    attacking = false;
                    attackImpactDone = false;
                    Agent.acceleration = movementAccel;
                    Agent.updateRotation = true;
                    Agent.Resume();
                    Anim.SetBool("Attacking", false);
                }
                // Attack impact
                if (attacking == true && attackImpactDone == false && attackTimer >= 0.90f)
                {
					attackImpactDone = true;
					GameObject bloodEmitter = Instantiate(Resources.Load("HostileGolemPEVoidBurst")) as GameObject;
                    bloodEmitter.transform.position = transform.position + transform.forward * 1.00f;
                    bloodEmitter.transform.rotation = transform.rotation;
                    //transform.rotation = Quaternion.LookRotation(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z) - transform.position);
                }
                //if (attacking == true && attackImpactDone == false && attackTimer >= )
            }
        }

        if (attackTimer < attackBaseTime)
            attackTimer += Time.deltaTime;


        Anim.SetFloat("Speed", Vector3.Magnitude(Agent.velocity));
	}
    void ResetHitObjectsList()
    {
        foreach (HostileGolemAttackHitbox Arm in GetComponentsInChildren<HostileGolemAttackHitbox>())
        {
            Arm.ResetHitObjects();
        }
    }
}

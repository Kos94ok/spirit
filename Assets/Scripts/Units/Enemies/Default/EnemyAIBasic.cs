using UnityEngine;
using System.Collections;

public class EnemyAIBasic : MonoBehaviour
{
    GameObject Player;
    UnityEngine.AI.NavMeshAgent Agent;
	void Start()
    {
	}
	void Update()
    {
        // Data update
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
        if (Agent == null)
            Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Objects not found
        if (Player == null || Agent == null)
            return;

        // Movement
        if (Vector3.Distance(Player.transform.position, transform.position) <= 5.00f)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z) - transform.position);
            Agent.SetDestination(Player.transform.position);
        }
	}
}

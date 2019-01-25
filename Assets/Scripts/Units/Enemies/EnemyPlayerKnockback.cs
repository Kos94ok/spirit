using UnityEngine;
using System.Collections;

public class EnemyPlayerKnockback : MonoBehaviour
{
    public float radius = 1.00f;

    GameObject Player;
    UnityEngine.AI.NavMeshAgent Agent;
	void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}
	void Update()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= radius)
        {
            Player.GetComponent<PlayerMovement>().ApplyForce(Player.transform.position - transform.position, 3.00f);
        }
	}
}

using UnityEngine;
using System.Collections;
using Units;

public class PlayerDeath : MonoBehaviour {

    UnitStats stats;
	void Start ()
    {
        stats = GetComponent<UnitStats>();
	}
	
	void Update ()
    {
	    if (!stats.IsAlive())
        {
            // Create an explosion
            GameObject explosion = Instantiate(Resources.Load("PlayerPEDeath")) as GameObject;
            explosion.transform.position = transform.position;
            // Disable all particle systems
            ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < systems.Length; i++)
            {
                systems[i].enableEmission = false;
            }

            Destroy(this);
        }
	}
}

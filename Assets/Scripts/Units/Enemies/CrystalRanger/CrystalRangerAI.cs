using UnityEngine;
using System.Collections.Generic;

public class CrystalRangerAI : MonoBehaviour
{
    const int sphereMaxCount = 6;

    public float sphereDamage;
    public float sphereTravelSpeed;
    public float sphereMaxTravelDistance;
    public float sphereShootingDelay;
    public float rotationSpeed;
    public float sphereOrbitDistance;
    public float sphereRespawnDelay;
    public float combatEngageRange;
    public float combatDisengageRange;
    public float combatExtraSpeed;

    bool inCombat = false;
    bool isShooting = false;

    float enrageCounter = 0.00f;
    float enrageDelayTimer = 0.00f;
    float sphereRespawnTimer = 0.00f;

    float rotationValue = 0.00f;
    float shootingRotationAccelerate = 0.00f;

    float shootingTimer = 0.00f;

    GameObject player;
    UnityEngine.AI.NavMeshAgent agent;
    GameObject body;
    GameObject bodyBase;
    float bodyFlyingHeight;
    List<GameObject> spheres = new List<GameObject>();

    GameObject spherePrefab;
    GameObject sphereSpawnPEPrefab;
    GameObject spherePrelaunchPEPrefab;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        body = transform.GetChild(0).gameObject;
        bodyBase = transform.GetChild(1).gameObject;
        body.transform.parent = agent.transform.parent;
        spherePrefab = Resources.Load("CrystalRangerProjectile") as GameObject;
        sphereSpawnPEPrefab = Resources.Load("CrystalRangerProjectilePESpawn") as GameObject;
        spherePrelaunchPEPrefab = Resources.Load("CrystalRangerProjectilePEPrepare") as GameObject;

        bodyFlyingHeight = body.transform.position.y - agent.transform.position.y;

        // Random rotation
        float randomAngle = Random.Range(0.00f, 360.00f);
        body.transform.Rotate(Vector3.up, randomAngle, Space.World);
        rotationValue = randomAngle;
    }

	void Update()
    {
        float distanceToPlayer = Math.GetDistance2D(player.transform.position, agent.transform.position);
        // Engage combat
        if (distanceToPlayer <= combatEngageRange && !inCombat)
        {
            inCombat = true;
        }
        // Disengage combat
        else if (distanceToPlayer > combatDisengageRange && inCombat)
        {
            inCombat = false;
        }
        // Be in combat
        else if (inCombat && spheres.Count == sphereMaxCount && !isShooting && !Utility.IsTargetObstructed(agent.transform.GetChild(0).position, player.transform.position))
        {
            isShooting = true;
            shootingTimer = sphereShootingDelay;
            SpawnParticleEmitterOnNextSphere();
        }
        else if (spheres.Count == 0 && isShooting)
        {
            StopShooting();
        }

        // Visual routines
        rotationValue += rotationSpeed * Time.deltaTime;
        if (isShooting)
        {
            shootingRotationAccelerate += 6.00f * Time.deltaTime;
            if (shootingRotationAccelerate > combatExtraSpeed) { shootingRotationAccelerate = combatExtraSpeed; }
            rotationValue += rotationSpeed * shootingRotationAccelerate * Time.deltaTime;
        }
        if (rotationValue >= 360.00f) { rotationValue -= 360.00f; }

        // Core rotation
        Vector3 bodyPos = agent.transform.position;
        bodyPos.y += bodyFlyingHeight;
        body.transform.position = bodyPos;
        body.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Creating new spheres
        if (sphereRespawnTimer == 0.00f && spheres.Count < sphereMaxCount && !isShooting)
        {
            GameObject newSphere = Instantiate(spherePrefab);
            newSphere.transform.parent = agent.transform.parent;
            newSphere.transform.position = Math.PolarVector2D(agent.transform.position, (360.00f / sphereMaxCount) * GetSpherePosition(spheres.Count) + rotationValue, sphereOrbitDistance);
            spheres.Add(newSphere);

            GameObject spawnPE = Instantiate(sphereSpawnPEPrefab);
            spawnPE.transform.parent = newSphere.transform;
            spawnPE.transform.position = newSphere.transform.GetChild(0).position;

            sphereRespawnTimer = sphereRespawnDelay;
        }
        else if (sphereRespawnTimer > 0.00f)
        {
            sphereRespawnTimer -= Time.deltaTime;
            if (sphereRespawnTimer < 0.00f) { sphereRespawnTimer = 0.00f; }
        }

        // Sphere management
        for (int i = 0; i < spheres.Count; i++)
        {
            int spherePosition = GetSpherePosition(i);
            // Spheres position
            if (true)
            {
                Vector3 spherePos = Math.PolarVector2D(agent.transform.position, (360.00f / sphereMaxCount) * spherePosition + rotationValue, sphereOrbitDistance);
                spheres[i].transform.position = spherePos;
            }
        }
        // Shooting spheres
        if (isShooting && shootingTimer > 0.00f)
        {
            // Only shoot if the target is visible
            if (!Utility.IsTargetObstructed(agent.transform.GetChild(0).position, player.transform.position))
            {
                shootingTimer -= Time.deltaTime;
                if (shootingTimer <= 0.00f)
                {
                    GameObject sphere = spheres[spheres.Count - 1];

                    // Calculate the launch vector
                    Vector3 playerPosition = player.transform.position;
                    //Vector3 playerVelocity = player.GetComponent<CharacterController>().velocity;
                    Vector3 playerVelocity = player.GetComponent<CharacterControllerExtension>().averageVelocity;
                    float interceptionTime = Utility.FirstOrderInterceptTime(sphereTravelSpeed, playerPosition - sphere.transform.GetChild(0).position, playerVelocity);
                    if (interceptionTime == 0.00f) { interceptionTime = Vector3.Distance(playerPosition, sphere.transform.GetChild(0).position) / sphereTravelSpeed; }
                    float verticalSpeed = (playerPosition.y - sphere.transform.GetChild(0).position.y) / interceptionTime;
                    float lifeTime = sphereMaxTravelDistance / sphereTravelSpeed;
                    float launchAngle;

                    launchAngle = Math.GetAngleRaw(sphere.transform.GetChild(0).position, playerPosition + interceptionTime * playerVelocity);

                    // Second and fourth spheres go directly to the unit
                    //if (spheres.Count % 2 != 0) { launchAngle = Math.GetAngleRaw(sphere.transform.GetChild(0).position, playerPosition); }
                    // First and third spheres go to intercept point
                    //else { launchAngle = Math.GetAngleRaw(sphere.transform.GetChild(0).position, playerPosition + interceptionTime * playerVelocity); }

                    // Launch the last sphere
                    sphere.GetComponent<CrystalRangerProjectile>().Launch(sphereDamage, sphereTravelSpeed, launchAngle, verticalSpeed, lifeTime);
                    sphere.AddComponent<TimedLife>().Timer = lifeTime + 0.50f;
                    spheres.Remove(sphere);

                    // If some spheres left, reset the timer
                    if (spheres.Count > 0)
                    {
                        shootingTimer = sphereShootingDelay;
                        SpawnParticleEmitterOnNextSphere();
                    }
                    else { shootingTimer = 0.00f; }
                }
            }
            else
            {
                StopShooting();
            }
        }
    }
    void SpawnParticleEmitterOnNextSphere()
    {
        GameObject PE = Instantiate(spherePrelaunchPEPrefab);
        PE.transform.position = spheres[spheres.Count - 1].transform.GetChild(0).transform.position;
        PE.transform.parent = spheres[spheres.Count - 1].transform;
    }
    int GetSpherePosition(int index)
    {
        switch (index)
        {
            case 0:
                return 0;
            case 1:
                return 3;
            case 2:
                return 1;
            case 3:
                return 4;
            case 4:
                return 2;
            case 5:
                return 5;
        }
        return index;
    }
    void StopShooting()
    {
        isShooting = false;
        shootingRotationAccelerate = 0.00f;
        sphereRespawnTimer = sphereRespawnDelay;
    }

    public GameObject AskForBody() { return body; }
    public GameObject AskForBase() { return bodyBase; }
    public List<GameObject> AskForSpheres() { return spheres; }
}

using UnityEngine;
using System.Collections;
using Units.Common;

public class CrystalRangerProjectile : MonoBehaviour
{
    bool isLaunched = false;
    float collisionDamage;
    float movingAngle;
    float movingSpeed;
    float movingSpeedVertical;
    float timeRemaining;

    GameObject player;
    GameObject deathPEPrefab;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        deathPEPrefab = Resources.Load("CrystalRangerProjectilePEDeath") as GameObject;
    }

    void Update()
    {
        if (isLaunched == true)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining > 0.00f)
            {
                float angleToPlayer = Vector3.Angle(transform.position, player.transform.position);
                Vector3 polarVector = Math.PolarVector2D(transform.position, movingAngle, movingSpeed * Time.deltaTime);
                polarVector.y = transform.position.y + movingSpeedVertical * Time.deltaTime;

                transform.position = polarVector;

                // Check for ground collision
                Vector3 bodyPosition = transform.GetChild(0).position;
                polarVector = Math.PolarVector2D(bodyPosition, movingAngle, movingSpeed * Time.deltaTime);
                polarVector.y = bodyPosition.y + movingSpeedVertical * Time.deltaTime;
                Vector3 direction = polarVector - bodyPosition;
                float distanceToObstacle = Utility.GetDistanceToObstacle(bodyPosition, direction);
                if (distanceToObstacle <= 0.2f) { KillProjectile(); }
            }
            else
            {
                KillProjectile();
            }
        }
    }

    public void Launch(float damage, float speed, float angle, float verticalSpeed, float lifeTime)
    {
        isLaunched = true;
        collisionDamage = damage;
        movingAngle = angle;
        movingSpeed = speed;
        movingSpeedVertical = verticalSpeed;
        timeRemaining = lifeTime;
        GetComponentInChildren<CrystalRangerProjectileHitbox>().SetDamage(collisionDamage);
        GetComponentInChildren<CrystalRangerProjectileHitbox>().SetVisual(movingAngle);
    }

    public void KillProjectile()
    {
        isLaunched = false;
        timeRemaining = 0.00f;
        GetComponent<TimedLife>().Timer = 1.00f;
        GetComponentInChildren<ParticleSystem>().enableEmission = false;
        Destroy(GetComponentInChildren<CrystalRangerProjectileHitbox>());

        GameObject deathPE = Instantiate(deathPEPrefab);
        deathPE.transform.position = transform.GetChild(0).position;
        deathPE.transform.Rotate(Vector3.up, -movingAngle);
        deathPE.GetComponent<ParticleSystem>().Play();
    }

    public bool IsLaunched()
    {
        return isLaunched;
    }
}

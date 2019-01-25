using UnityEngine;
using System.Collections;

public class HeroSoul_Marksman_Arrow : MonoBehaviour
{
    const int projectileInterpolation = 8;

    bool isFadingIn = true;
    bool isFadingOut = false;
    float limitAlpha;

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

        // Save max alpha and reset the initial value
        Color color = GetComponent<SkinnedMeshRenderer>().material.color;
        limitAlpha = color.a;
        color.a = 0.00f;
        GetComponent<SkinnedMeshRenderer>().material.color = color;
    }

    void Update()
    {
        if (isLaunched == true)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining > 0.00f)
            {
                float angleToPlayer = Vector3.Angle(transform.position, player.transform.position);

                float interpolatedDelta = Time.deltaTime / projectileInterpolation;
                Vector3 projectilePosition = transform.position;

                for (int i = 0; i < projectileInterpolation; i++)
                {
                    Vector3 polarVector = Math.PolarVector2D(projectilePosition, movingAngle, movingSpeed * interpolatedDelta);
                    polarVector.y = projectilePosition.y + movingSpeedVertical * interpolatedDelta;

                    projectilePosition = polarVector;

                    // Check for ground collision
                    polarVector = Math.PolarVector2D(projectilePosition, movingAngle, movingSpeed * interpolatedDelta);
                    polarVector.y += movingSpeedVertical * interpolatedDelta;
                    Vector3 direction = polarVector - projectilePosition;
                    float distanceToObstacle = Utility.GetDistanceToObstacle(projectilePosition, direction);
                    if (distanceToObstacle <= 0.03f)
                    {
                        KillProjectile();
                        break;
                    }
                }
                transform.position = projectilePosition;
            }
            else
            {
                KillProjectile();
            }
        }
        // Fading
        if (isFadingIn || isFadingOut)
        {
            Color renderColor = GetComponent<SkinnedMeshRenderer>().material.color;
            if (isFadingIn && renderColor.a < limitAlpha)
            {
                renderColor.a += Time.deltaTime * 1.00f;
                if (renderColor.a >= limitAlpha)
                {
                    isFadingIn = false;
                    renderColor.a = limitAlpha;
                }
            }
            if (isFadingOut && renderColor.a > 0.00f)
            {
                renderColor.a -= Time.deltaTime * 0.50f;
                if (renderColor.a < 0.00f)
                {
                    isFadingOut = false;
                    renderColor.a = 0.00f;
                }
            }
            GetComponent<SkinnedMeshRenderer>().material.color = renderColor;
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
        GetComponentInChildren<HeroSoul_Marksman_ArrowHitbox>().SetDamage(collisionDamage);
        GetComponentInChildren<HeroSoul_Marksman_ArrowHitbox>().SetVisual(movingAngle);
    }

    public void KillProjectile()
    {
        isLaunched = false;
        isFadingOut = true;
        timeRemaining = 0.00f;
        gameObject.AddComponent<TimedLife>().Timer = 1.00f;

        GetComponentInChildren<ParticleSystem>().enableEmission = false;
        Destroy(GetComponentInChildren<HeroSoul_Marksman_ArrowHitbox>());

        /*GameObject deathPE = Instantiate(deathPEPrefab);
        deathPE.transform.position = transform.position;
        deathPE.transform.Rotate(Vector3.up, -movingAngle);
        deathPE.GetComponent<ParticleSystem>().Play();*/
    }

    public bool IsLaunched()
    {
        return isLaunched;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroSoul_Berserker_Projectile : MonoBehaviour
{
    [HideInInspector]
    public float SwordDamage;
    [HideInInspector]
    public float AttackArc;
    [HideInInspector]
    public float ImpactPower;
    [HideInInspector]
    public float LastAttackRange;
    [HideInInspector]
    public int CurrentSwing;
    [HideInInspector]
    public bool IsAlternate;
    [HideInInspector]
    public int SwingDirection;

    float LifeTimer = 0.50f;
    float LifeTimerMax = 0.50f;
    Vector3 RotationAxis;
    int RotationDirection;

    Color OldColor;
    List<GameObject> HitObjects;
	void Start()
    {
        transform.Rotate(new Vector3(1, 0, 0), 90.00f);
        OldColor = GetComponent<SkinnedMeshRenderer>().material.color;
        OldColor.a = 0.00f;
        GetComponent<SkinnedMeshRenderer>().material.color = OldColor;
        HitObjects = new List<GameObject>();
	}
	void Update()
    {
        // Update rotation or position
        if (!IsAlternate)
        {
            if (RotationAxis != null)
                transform.RotateAround(RotationAxis, Vector3.up, Time.deltaTime * AttackArc / LifeTimerMax * RotationDirection);
        }
        else
        {
            transform.Translate(new Vector3(-Time.deltaTime * LastAttackRange / LifeTimerMax, 0.00f, 0.00f), Space.Self);
        }

        // Update color
        float Opacity = 1.00f;
        if (LifeTimer < LifeTimerMax * 0.50f)
            Opacity = LifeTimer / (LifeTimerMax * 0.5f);
        else if (LifeTimer > LifeTimerMax * 0.50f)
            Opacity = (LifeTimerMax - LifeTimer) / (LifeTimerMax * 0.5f);
        Opacity *= 0.50f;
        OldColor.a = Opacity;
        GetComponent<SkinnedMeshRenderer>().material.color = OldColor;

        // Update life timer
        LifeTimer -= Time.deltaTime;
        if (LifeTimer <= 0.00f)
        {
            Destroy(gameObject.GetComponentInChildren<TrailRenderer>(), 2.00f);
            Destroy(gameObject.GetComponentInChildren<ParticleSystem>(), 2.00f);
            ParticleSystem[] EmitterList = gameObject.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < EmitterList.Length; i++ )
            {
                EmitterList[i].transform.parent = null;
            }
            Destroy(gameObject);
        }
	}
    void OnTriggerEnter(Collider hit)
    {
        var EnemyHit = hit.gameObject.GetComponent<UnitHitbox>();
        var EnemyBody = hit.gameObject.GetComponent<Rigidbody>();
        if (HitObjects.Contains(hit.gameObject))
            return;
        if (EnemyHit != null && EnemyHit.Stats.alliance == UnitAlliance.Enemy && EnemyHit.Stats.IsAlive())
        {
            EnemyHit.DealDamage(SwordDamage, gameObject);
            HitObjects.Add(hit.gameObject);
            UnitBleeding Bleeding = hit.GetComponent<UnitBleeding>();
            if (Bleeding != null)
            {
                GameObject bloodEmitter = Instantiate(Resources.Load("EnemyPEBlood")) as GameObject;
                bloodEmitter.GetComponent<ParticleSystem>().startColor = Bleeding.bloodColor;
                bloodEmitter.transform.rotation = transform.rotation;
                bloodEmitter.transform.position = transform.position;
                if (!IsAlternate && SwingDirection == 0) // Right
                    bloodEmitter.transform.Rotate(Vector3.up, -90, Space.World);
                else if (!IsAlternate && SwingDirection == 1) // Left
                    bloodEmitter.transform.Rotate(Vector3.up, 90, Space.World);
                else if (IsAlternate) // Forward
                    bloodEmitter.transform.Rotate(Vector3.up, 180, Space.World);
            }
            Vector3 Force = Vector3.MoveTowards(transform.position, hit.gameObject.transform.position, 1.00f) - transform.position;
            EnemyHit.ApplyForce(Force * ImpactPower, ForceMode.Impulse);
        }
        else if (EnemyBody != null)
        {
            Vector3 Force = Vector3.MoveTowards(transform.position, hit.gameObject.transform.position, 1.00f) - transform.position;
            EnemyBody.AddForce(Force * ImpactPower, ForceMode.Impulse);
        }
    }
    public void SetRotationAxis(Vector3 Axis, int Direction)
    {
        RotationAxis = Axis;
        RotationDirection = Direction;
    }
}

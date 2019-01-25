using UnityEngine;
using System.Collections;

public class LightSourceController : MonoBehaviour
{
    const float defaultIntensityChangeSpeed = 1.00f;
    const float defaultRangeChangeSpeed = 1.00f;

    bool Initialized = false;
    Light me;
    float defaultLightIntensity;
    float defaultLightRange;
    float desiredLightIntensity;
    float desiredLightRange;

    float intensityChangeSpeed;
    float rangeChangeSpeed;

    float savedIntensity;
    float savedRange;
    void Initialize()
    {
        if (Initialized)
            return;

        me = GetComponent<Light>();
        Initialized = true;
    }
	void Start()
    {
        Initialize();
        defaultLightIntensity = me.intensity;
        defaultLightRange = me.range;
        desiredLightIntensity = defaultLightIntensity;
        desiredLightRange = defaultLightRange;
	}
	
    void Update()
    {
        me.intensity += (desiredLightIntensity - me.intensity) * intensityChangeSpeed * Time.deltaTime;
        me.range += (desiredLightRange - me.range) * rangeChangeSpeed * Time.deltaTime;

        if (me.enabled && (me.intensity <= 0.00f || me.range <= 0.00f))
            me.enabled = false;
        else if (!me.enabled && me.intensity > 0.00f && me.range > 0.00f)
            me.enabled = true;
	}

    public float GetLightIntensity()
    {
        return me.intensity;
    }
    public float GetLightRange()
    {
        return me.range;
    }
    public void SetLightIntensity(float intensity, float speed = 0.00f)
    {
        if (speed > 0.00f)
        {
            desiredLightIntensity = intensity;
            intensityChangeSpeed = speed;
        }
        else
        {
            me.intensity = intensity;
            desiredLightIntensity = intensity;
        }
    }
    public void SetLightRange(float range, float speed = 0.00f)
    {
        if (speed > 0.00f)
        {
            desiredLightRange = range;
            rangeChangeSpeed = speed;
        }
        else
        {
            me.range = range;
            desiredLightRange = range;
        }
    }
    public void ResetLightPower()
    {
        SetLightIntensity(defaultLightIntensity, defaultIntensityChangeSpeed);
        SetLightRange(defaultLightRange, defaultRangeChangeSpeed);
    }
    public void ResetLightPower(float speed)
    {
        SetLightIntensity(defaultLightIntensity, speed);
        SetLightRange(defaultLightRange, speed);
    }
    public void Enable(float speed = 0.00f)
    {
        SetLightIntensity(savedIntensity, speed);
        SetLightRange(savedRange, speed);
    }
    public void Disable(float speed = 0.00f)
    {
        Initialize();
        savedIntensity = me.intensity;
        savedRange = me.range;
        SetLightIntensity(0.00f, speed);
        SetLightRange(0.00f, speed);
    }
}

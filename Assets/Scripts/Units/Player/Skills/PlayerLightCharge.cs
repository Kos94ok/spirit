using UnityEngine;
using System.Collections;

public class PlayerLightCharge : MonoBehaviour
{
    const float ChargeSpeed = 2.50f;
    const float ChargeMaxValue = 5.00f;
    const float AutoReleaseValue = 4.50f;
    const float Cooldown = 2.00f;
    const float ChargeBurstSpeed = 5.00f;
    const float ChargeFadeSpeed = 1.00f;
    const float ChargeCancelSpeed = 1.00f;
    const float ResetSpeed = 0.15f;

    bool IsCharging = false;
    float ChargedPower = 0.00f;
    float CooldownTimer = 0.00f;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.F) && !IsCharging && CooldownTimer <= 0.00f)
        {
            IsCharging = true;
            ChargedPower = 1.00f;
            GetComponentInChildren<LightSourceController>().SetLightRange(3.00f, ChargeFadeSpeed);
            GetComponentInChildren<LightSourceController>().SetLightIntensity(0.50f, ChargeFadeSpeed);
            GameObject Emitter = Instantiate(Resources.Load("PlayerPELightCharge")) as GameObject;
            StartCoroutine(DestroyEmitter(Emitter));
        }
        else if ((!Input.GetKey(KeyCode.F) && IsCharging) || (IsCharging && ChargedPower >= AutoReleaseValue))
        {
            IsCharging = false;
            CooldownTimer = Cooldown;
            if (ChargedPower >= 1.50f)
            {
                GetComponentInChildren<LightSourceController>().SetLightRange(5.00f + 2.50f * ChargedPower, ChargeBurstSpeed);
                GetComponentInChildren<LightSourceController>().SetLightIntensity(1.00f + 0.25f * ChargedPower, ChargeBurstSpeed);
                StartCoroutine(RestoreLightCoroutine(0.50f + ChargedPower * 0.10f));
                GameObject Emitter = Instantiate(Resources.Load("PlayerPELightChargeBurst")) as GameObject;
                Emitter.transform.position = transform.position;
                ChargedPower = 0.00f;
            }
            else
            {
                GetComponentInChildren<LightSourceController>().ResetLightPower(ChargeCancelSpeed);
            }
        }
        // Charge the value
        if (IsCharging)
            ChargedPower += Time.deltaTime * ChargeSpeed * ((ChargeMaxValue - ChargedPower) / ChargeMaxValue);
        // Cooldown
        if (CooldownTimer > 0.00f)
        {
            CooldownTimer -= Time.deltaTime;
            CooldownTimer = Mathf.Max(0.00f, CooldownTimer);
        }
    }
    IEnumerator RestoreLightCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponentInChildren<LightSourceController>().ResetLightPower(ResetSpeed);
    }

    IEnumerator DestroyEmitter(GameObject Emitter)
    {
        while (IsCharging)
            yield return null;
        Destroy(Emitter);
    }
}

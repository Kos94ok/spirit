using Units.Common;
using UnityEngine;

namespace Units.Enemies.ForgottenDweller {
    public class ForgottenDwellerDeath : UnitDeath {
        public override void OnDeath() {
            // Add burst decay particle emitter
            var decayPE = (GameObject) Instantiate(Resources.Load("ParticleEmitters/ForgottenDwellerPEDeath"));
            decayPE.transform.position = GetComponent<UnitStats>().GetHitTargetPosition();
            decayPE.transform.rotation = transform.rotation;
            decayPE.transform.Rotate(Vector3.up, -90, Space.World);

            // Set free the main particle emitter
            var child = transform.GetChild(0).gameObject;
            child.transform.parent = null;
            child.AddComponent<TimedLife>().Timer = 2.00f;
            child.GetComponent<ParticleSystem>().enableEmission = false;

            Destroy(gameObject);
        }
        public override void ApplyForce(Vector3 force, ForceMode mode) { }
    }
}

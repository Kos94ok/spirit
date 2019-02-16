using System.Collections.Generic;
using UnityEngine;

namespace Units.Player.Movement {
    public class CharacterControllerExtension : MonoBehaviour {
        private Vector3 AverageVelocity;
        private readonly List<Vector3> VelocityHistory = new List<Vector3>();

        private CharacterController Original;

        private void Start() {
            Original = GetComponent<CharacterController>();
        }

        private void Update() {
            while (VelocityHistory.Count > 60) {
                VelocityHistory.RemoveAt(0);
            }
            VelocityHistory.Add(Original.velocity);

            AverageVelocity = new Vector3(0.00f, 0.00f, 0.00f);
            foreach (var entry in VelocityHistory) {
                AverageVelocity += entry;
            }
            AverageVelocity /= VelocityHistory.Count;
        }

        public Vector3 GetCurrentVelocity() {
            return Original.velocity;
        }
        
        public Vector3 GetAverageVelocity() {
            return AverageVelocity;
        }
    }
}

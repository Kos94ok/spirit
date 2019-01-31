using Misc;
using UnityEngine;
using Texture = Misc.Texture;

namespace UI {
    public class OverheadHealthBar : MonoBehaviour
    {
        private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();

        public Vector2 Size = new Vector2(60, 10);
        public Vector3 OffsetFromCenter = new Vector3(0, 0, 0);

        private float CurrentPercentage;
        private Vector2 CurrentPosition;
        
        private UnitStats Stats;
        private Camera MainCamera;
        private Texture2D HealthBarEmpty;
        private Texture2D HealthBarFull;

        private void Start() {
            CurrentPosition = OffsetFromCenter;
            
            Stats = GetComponent<UnitStats>();
            MainCamera = Camera.main;
            HealthBarEmpty = Assets.Get(Texture.HealthBarEmpty);
            HealthBarFull = Assets.Get(Texture.HealthBarFull);
        }
        private void Update() {
            if (Stats.IsAlive() && Stats.Health < Stats.HealthMax) {
                CurrentPercentage = Stats.Health / Stats.HealthMax;
                CurrentPosition = MainCamera.WorldToScreenPoint(Stats.GetHitTargetPosition() + OffsetFromCenter);
                CurrentPosition.y = Screen.height - CurrentPosition.y;
            } else {
                CurrentPercentage = 1;
            }
        }

        private void OnGUI() {
            GUI.BeginGroup(new Rect(CurrentPosition - Size / 2, Size));

            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.50f);
            GUI.DrawTexture(new Rect(0, 0, Size.x, Size.y), HealthBarEmpty);
            GUI.DrawTexture(new Rect(0, 0, Size.x * CurrentPercentage, Size.y), HealthBarFull);

            GUI.EndGroup();
        }
    }
}

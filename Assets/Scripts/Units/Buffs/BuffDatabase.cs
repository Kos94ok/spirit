using System.Collections.Generic;
using UnityEngine;

namespace Units.Buffs {
    public enum BuffAlignment {
        Positive,
        Neutral,
        Negative,
    }

    public enum BuffStackType {
        NoStacks,
        StacksDuration,
        StacksIntensity,
    }

    public enum Buff {
        ManaShield,
        DrawingBow,
    }

    internal class BuffData {
        public int DefaultStacks { get; }
        public float DefaultDuration { get; }
        public BuffAlignment Alignment { get; }
        public BuffStackType StackType { get; }
        public BuffData(int defaultStacks, float defaultDuration, BuffAlignment alignment, BuffStackType stackType) {
            DefaultStacks = defaultStacks;
            DefaultDuration = defaultDuration;
            Alignment = alignment;
            StackType = stackType;
        }
    }

    public class BuffDatabase : MonoBehaviour {
        private static readonly Dictionary<Buff, BuffData> BuffLibrary = new Dictionary<Buff, BuffData>();

        // Registration
        private void Start() {
            Register(Buff.ManaShield, BuffStackType.NoStacks, 1, 0.1f, BuffAlignment.Positive);
            Register(Buff.DrawingBow, BuffStackType.NoStacks, 1, 999f, BuffAlignment.Neutral);

            // Register all the remaining buffs
            foreach (Buff id in System.Enum.GetValues(typeof(Buff))) {
                if (!BuffLibrary.ContainsKey(id)) {
                    Register(id, BuffStackType.NoStacks, 1, 1f, BuffAlignment.Neutral);
                }
            }
        }

        private void Register(Buff id, BuffStackType stackType, int defaultStacks, float defaultDuration, BuffAlignment alignment) {
            var buffData = new BuffData(defaultStacks, defaultDuration, alignment, stackType);
            BuffLibrary.Add(id, buffData);
        }

        // Public API
        public static int GetDefaultStacks(Buff id) { return BuffLibrary[id].DefaultStacks; }
        public static float GetDefaultDuration(Buff id) { return BuffLibrary[id].DefaultDuration; }
        public static BuffAlignment GetAlignment(Buff id) { return BuffLibrary[id].Alignment; }
        public static BuffStackType GetStackType(Buff id) { return BuffLibrary[id].StackType; }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Units.Buffs {
    public class BuffController : MonoBehaviour {
        private readonly List<BuffInstance> BuffList = new List<BuffInstance>();

        private void Update() {
            for (var i = 0; i < BuffList.Count; i++) {
                BuffList[i].timeLeft -= Time.deltaTime;
                if (BuffList[i].timeLeft <= 0f) {
                    BuffList.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        private int GetIndex(Buff id) {
            for (var i = 0; i < BuffList.Count; i++) {
                if (BuffList[i].id == id) { return i; }
            }
            return -1;
        }

        private void AddInternal(Buff id, int intensity, float duration) {
            var index = GetIndex(id);
            if (index == -1 || BuffDatabase.GetStackType(id) == BuffStackType.StacksIntensity) {
                var instance = new BuffInstance {
                    id = id,
                    stacks = intensity,
                    timeLeft = duration,
                    alignment = BuffDatabase.GetAlignment(id)
                };
                BuffList.Add(instance);
            } else if (BuffDatabase.GetStackType(id) == BuffStackType.NoStacks) {
                BuffList[index].timeLeft = Mathf.Max(duration, BuffList[index].timeLeft);
            } else if (BuffDatabase.GetStackType(id) == BuffStackType.StacksDuration) {
                BuffList[index].timeLeft += duration;
            }
        }

        // Public API
        public void Add(Buff id) { AddInternal(id, BuffDatabase.GetDefaultStacks(id), BuffDatabase.GetDefaultDuration(id)); }
        public void AddIntensity(Buff id, int intensity) { AddInternal(id, intensity, BuffDatabase.GetDefaultDuration(id)); }
        public void AddDuration(Buff id, float duration) { AddInternal(id, BuffDatabase.GetDefaultStacks(id), duration); }
        public void Add(Buff id, int intensity, float duration) { AddInternal(id, intensity, duration); }

        public bool Has(Buff id) {
            return GetIndex(id) != -1;
        }
        
        public int GetIntensity(Buff id) {
            var intensity = 0;
            foreach (var buff in BuffList) {
                if (buff.id == id)
                    intensity += buff.stacks;
            }
            return intensity;
        }
        
        public float GetRemainingDuration(Buff id) {
            var longestTime = 0f;
            foreach (var buff in BuffList) {
                if (buff.id == id && buff.timeLeft > longestTime)
                    longestTime = buff.timeLeft;
            }
            return longestTime;
        }
        
        public void Remove(Buff id) {
            for (var i = 0; i < BuffList.Count; i++) {
                if (BuffList[i].id == id) {
                    BuffList.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public void RemoveAll() {
            BuffList.Clear();
        }
        
        public void RemoveAll(BuffAlignment alignment) {
            for (var i = 0; i < BuffList.Count; i++) {
                if (BuffList[i].alignment == alignment) {
                    BuffList.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }
}

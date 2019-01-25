using UnityEngine;
using System.Collections.Generic;

public enum BuffAlignment
{
    Positive,
    Neutral,
    Negative,
}

public enum BuffStackType
{
    NoStacks,
    StacksDuration,
    StacksIntensity,
}

public enum Buff
{
    ManaShield,
    DrawingBow,
    Slowed,
    Incapacitated,
    PhasedOut,
    Burning,
    Cursed,
}

public class BuffDatabase : MonoBehaviour
{
    // Data
    static List<Buff> registeredBuffList = new List<Buff>();
    static Dictionary<Buff, BuffStackType> buffStackType = new Dictionary<Buff, BuffStackType>();
    static Dictionary<Buff, int> buffDefaultStacks = new Dictionary<Buff, int>();
    static Dictionary<Buff, float> buffDefaultDuration = new Dictionary<Buff, float>();
    static Dictionary<Buff, BuffAlignment> buffAlignment = new Dictionary<Buff, BuffAlignment>();

    // Registration
    void Start()
    {
        Register(Buff.ManaShield, BuffStackType.NoStacks, 1, 0.1f, BuffAlignment.Positive);
        Register(Buff.DrawingBow, BuffStackType.NoStacks, 1, 999f, BuffAlignment.Neutral);
        Register(Buff.Slowed, BuffStackType.StacksIntensity, 1, 5f, BuffAlignment.Negative);
        Register(Buff.Incapacitated, BuffStackType.StacksDuration, 1, 5f, BuffAlignment.Negative);
        Register(Buff.PhasedOut, BuffStackType.StacksDuration, 1, 5f, BuffAlignment.Negative);
        Register(Buff.Burning, BuffStackType.StacksIntensity, 1, 5f, BuffAlignment.Negative);
        Register(Buff.Cursed, BuffStackType.StacksIntensity, 1, 5f, BuffAlignment.Negative);

        // Register all the remaining buffs
        foreach (Buff id in System.Enum.GetValues(typeof(Buff)))
        {
            if (!buffStackType.ContainsKey(id))
            {
                Register(id, BuffStackType.NoStacks, 1, 1f, BuffAlignment.Neutral);
            }
        }
    }

    void Register(Buff id, BuffStackType stackType, int defaultStacks, float defaultDuration, BuffAlignment alignment)
    {
        registeredBuffList.Add(id);
        buffStackType.Add(id, stackType);
        buffDefaultStacks.Add(id, defaultStacks);
        buffDefaultDuration.Add(id, defaultDuration);
        buffAlignment.Add(id, alignment);
    }

    // Public API
    public static BuffStackType GetStackType(Buff id) { return buffStackType[id]; }
    public static int GetDefaultStacks(Buff id) { return buffDefaultStacks[id]; }
    public static float GetDefaultDuration(Buff id) { return buffDefaultDuration[id]; }
    public static BuffAlignment GetAlignment(Buff id) { return buffAlignment[id]; }
}

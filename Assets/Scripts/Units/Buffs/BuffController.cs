using UnityEngine;
using System.Collections.Generic;

public class BuffController : MonoBehaviour
{
    List<BuffInstance> buffList = new List<BuffInstance>();

    // Default behaviour
	void Start()
    {
	
	}
	void Update()
    {
	    for (int i = 0; i < buffList.Count; i++)
        {
            buffList[i].timeLeft -= Time.deltaTime;
            if (buffList[i].timeLeft <= 0f)
            {
                buffList.RemoveAt(i);
                i -= 1;
            }
        }
	}

    // Internal functionality
    int GetIndex(Buff id)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id) { return i; }
        }
        return -1;
    }

    void AddInternal(Buff id, int intensity, float duration)
    {
        int index = GetIndex(id);
        // Not buffed or stacks intensity
        if (index == -1 || BuffDatabase.GetStackType(id) == BuffStackType.StacksIntensity)
        {
            BuffInstance instance = new BuffInstance();
            instance.id = id;
            instance.stacks = intensity;
            instance.timeLeft = duration;
            instance.alignment = BuffDatabase.GetAlignment(id);

            buffList.Add(instance);
        }
        // Does not stack
        else if (BuffDatabase.GetStackType(id) == BuffStackType.NoStacks)
        {
            buffList[index].timeLeft = Mathf.Max(duration, buffList[index].timeLeft);
        }
        // Stacks duration
        else if (BuffDatabase.GetStackType(id) == BuffStackType.StacksDuration)
        {
            buffList[index].timeLeft += duration;
        }
    }

    // Public API
    public void Add(Buff id) { AddInternal(id, BuffDatabase.GetDefaultStacks(id), BuffDatabase.GetDefaultDuration(id)); }
    public void AddIntensity(Buff id, int intensity) { AddInternal(id, intensity, BuffDatabase.GetDefaultDuration(id)); }
    public void AddDuration(Buff id, float duration) { AddInternal(id, BuffDatabase.GetDefaultStacks(id), duration); }
    public void Add(Buff id, int intensity, float duration) { AddInternal(id, intensity, duration); }
    public bool Has(Buff id) { return GetIndex(id) != -1; }
    public int GetIntensity(Buff id)
    {
        int intensity = 0;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id)
                intensity += buffList[i].stacks;
        }
        return intensity;
    }
    public float GetRemainingDuration(Buff id)
    {
        float longestTime = 0f;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id && buffList[i].timeLeft > longestTime)
                longestTime = buffList[i].timeLeft;
        }
        return longestTime;
    }
    public void Remove(Buff id)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                buffList.RemoveAt(i);
                i -= 1;
            }
        }
    }
    public void RemoveAll() { buffList.Clear(); }
    public void RemoveAll(BuffAlignment alignment)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].alignment == alignment)
            {
                buffList.RemoveAt(i);
                i -= 1;
            }
        }
    }
}

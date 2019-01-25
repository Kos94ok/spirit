using UnityEngine;
using System.Collections;

public class HeroSoul_Shieldmaiden : HeroSoulOffensive_Old
{
    public override bool AttackPress(Vector3 Target)
    {
        return false;
    }
    public override bool AttackRelease(Vector3 Target)
    {
        return false;
    }
    public override bool AlternatePress(Vector3 Target)
    {
        return false;
    }
    public override bool AlternateRelease(Vector3 Target)
    {
        return false;
    }
}
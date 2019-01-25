using UnityEngine;
using System.Collections;
using System;

public class HeroSoul_Knight : HeroSoulOffensive_Old
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSoul_RoyalGuard : HeroSoulOffensive {

	new public void Start() {
        base.Start();
        RegisterAbility(AbilitySlot.LeftClick, new RoyalGuard_TripleSwing());
    }
}

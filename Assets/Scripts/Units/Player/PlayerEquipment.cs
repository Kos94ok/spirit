﻿using System;
using System.Collections.Generic;
using UI;
using UI.ChatLog;
using UnityEngine;

//=============================================================================================================
// Description:
//=============================================================================================================
// - Souls are the pieces of equipment. A player can use up to two souls at the same time, those being
//   an offensive soul, responsible for primary and secondary attacks, and a defensive soul, responsible
//   for health, armor, resistances, damage responses and such matters.
//
// - Runes are active abilities that the player can use. Those are either cooldown- or mana-based,
//   possibly both. I believe they are going to be bound to 1, 2, 3 and 4 keys. Player may have multiple
//   copies of the same rune and they will operate separately from each other.
//
// - Glyphs are passive slots that change the way some abilities, skills or attacks work. Those may be
//   generic or designed to fit specific souls or soul classes. Glyphs do not stack.
//
// - Marks are passive stackable upgrades that increase basic stats of souls or hero herself. Those
//   are generally easy to find and their bonuses are small, but the chance to find the marks is
//   significant.
//=============================================================================================================

public enum EquipmentType
{
    Rune,
    Glyph,
    SoulOffensive,
    SoulDefensive,
}
public enum SoulOffensive
{
    None,

    Knight,
    Berserker,
    Marksman,
    Priestess,
    Shieldmaiden,
}
public enum SoulDefensive
{
    None,
}
public enum Rune
{
    None,
}
public enum Glyph
{
    None,
    BlinkCost,
    BlinkPrecision,
}

public enum Mark
{
    
}

public class PlayerEquipment : MonoBehaviour
{
    const float equipmentSwapCooldown = 0.00f;

    SoulOffensive equippedSoulOffensive = SoulOffensive.None;
    SoulDefensive equippedSoulDefensive = SoulDefensive.None;
    List<Glyph> equippedGlyphs = new List<Glyph>();

    Dictionary<SoulOffensive, Type> soulMapOffensive = new Dictionary<SoulOffensive, Type>();
    Dictionary<SoulDefensive, Type> soulMapDefensive = new Dictionary<SoulDefensive, Type>();

    Dictionary<KeyCode, SoulOffensive> offensiveBinding = new Dictionary<KeyCode, SoulOffensive>();

    float equipmentSwapTimer = 0.00f;

    private ChatLog chatLog = AutowireFactory.GetInstanceOf<ChatLog>();
    private Localization localization = AutowireFactory.GetInstanceOf<Localization>();

	private void Start ()
    {
        // Initialize the dictionaries
        soulMapOffensive.Add(SoulOffensive.Knight, typeof(HeroSoul_Knight));
        soulMapOffensive.Add(SoulOffensive.Berserker, typeof(HeroSoul_Berserker));
        soulMapOffensive.Add(SoulOffensive.Marksman, typeof(HeroSoul_Marksman));
        soulMapOffensive.Add(SoulOffensive.Priestess, typeof(HeroSoul_Priestess));
        soulMapOffensive.Add(SoulOffensive.Shieldmaiden, typeof(HeroSoul_Shieldmaiden));

        // Temporary equipment system
        offensiveBinding.Add(KeyCode.Alpha1, SoulOffensive.Knight);
        offensiveBinding.Add(KeyCode.Alpha2, SoulOffensive.Berserker);
        offensiveBinding.Add(KeyCode.Alpha3, SoulOffensive.Marksman);
        offensiveBinding.Add(KeyCode.Alpha4, SoulOffensive.Priestess);

        // Equip default stuff
        EquipSoulOffensive(SoulOffensive.Berserker);
        //EquipGlyph(Glyph.BlinkCost);
        //EquipGlyph(Glyph.BlinkPrecision);
	}
	
	void Update ()
    {
	    // Temporary equipment swap system
        if (equipmentSwapTimer > 0.00f)
        {
            // Adjust the timer
            equipmentSwapTimer -= Time.deltaTime;

            // Display error message if one of the keys is pressed
            foreach (KeyCode key in offensiveBinding.Keys)
            {
                if (Input.GetKeyDown(key) && equippedSoulOffensive != offensiveBinding[key])
                {
                    chatLog.PostRaw(localization.Get("error_soulEquipCooldown") + Math.Round(equipmentSwapTimer, 2) + " " + localization.Get("sec"), ChatWindow.Error);
                    break;
                }
            }
        }
        else
        {
            // Check if one of the keys is pressed and equip corresponding soul
            foreach (KeyCode key in offensiveBinding.Keys)
            {
                if (Input.GetKeyDown(key) && equippedSoulOffensive != offensiveBinding[key])
                {
                    equipmentSwapTimer = equipmentSwapCooldown;
                    EquipSoulOffensive(offensiveBinding[key]);
                    chatLog.Post("soulEquipped_" + offensiveBinding[key].ToString().ToLower());
                }
            }
        }
	}

    //================================================================
    // Souls
    //================================================================
    public void EquipSoulOffensive(SoulOffensive soul)
    {
        UnequipSoul(EquipmentType.SoulOffensive);
        equippedSoulOffensive = soul;
        gameObject.AddComponent(soulMapOffensive[soul]);
        UpdateSoulController();
    }
    public void EquipSoulDefensive(SoulDefensive soul)
    {
        UnequipSoul(EquipmentType.SoulDefensive);
        equippedSoulDefensive = soul;
        gameObject.AddComponent(soulMapDefensive[soul]);
        UpdateSoulController();
    }

    public void UnequipSoul(EquipmentType type)
    {
        // Check if the thing is equipped in the first place
        if ((type == EquipmentType.SoulOffensive && equippedSoulOffensive != SoulOffensive.None) || (type == EquipmentType.SoulDefensive && equippedSoulDefensive != SoulDefensive.None))
        {
            // Find component class
            Type soulClass = null;
            if (type == EquipmentType.SoulOffensive)
            {
                soulClass = soulMapOffensive[equippedSoulOffensive];
                equippedSoulOffensive = SoulOffensive.None;
            }
            else if (type == EquipmentType.SoulDefensive)
            {
                soulClass = soulMapDefensive[equippedSoulDefensive];
                equippedSoulDefensive = SoulDefensive.None;
            }
            // Destroy the component
            Component component = gameObject.GetComponent(soulClass);
            if (component != null) { Destroy(component); }
        }
    }

    public void UpdateSoulController()
    {
        // Offensive
        HeroSoulOffensive_Old soulOffensive;
        if (equippedSoulOffensive != SoulOffensive.None) { soulOffensive = GetComponent(soulMapOffensive[equippedSoulOffensive]) as HeroSoulOffensive_Old; }
        else { soulOffensive = null; }
        // Defensive
        HeroSoulDefensive soulDefensive;
        if (equippedSoulDefensive != SoulDefensive.None) { soulDefensive = GetComponent(soulMapDefensive[equippedSoulDefensive]) as HeroSoulDefensive; }
        else { soulDefensive = null; }
        // Send
        GetComponent<HeroSoulController>().UpdateSouls(soulOffensive, soulDefensive);
    }

    //================================================================
    // Glyphs
    //================================================================
    public void EquipGlyph(Glyph glyph)
    {
        // Duplicates are not allowed
        if (!HasGlyph(glyph))
        {
            equippedGlyphs.Add(glyph);
        }
    }
    public void UnequipGlyph(Glyph glyph)
    {
        equippedGlyphs.Remove(glyph);
    }
    public bool HasGlyph(Glyph glyph)
    {
        return equippedGlyphs.Contains(glyph);
    }
}

﻿using UI;
using UI.ChatLog;
using UnityEngine;

public class UnlockEquipmentOnInteract : Interact
{
    public EquipmentType unlockType;
    public Rune unlockedRune;
    public Glyph unlockedGlyph;
    public string messageLink;

    private ChatLog chatLog = AutowireFactory.GetInstanceOf<ChatLog>();

    public override void OnInteract()
    {
        PlayerEquipment equipment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEquipment>();
        if (unlockType == EquipmentType.Glyph)
        {
            equipment.EquipGlyph(unlockedGlyph);
        }
        DisableInteraction();
        // Send message
        if (messageLink != null && messageLink != "")
        {
            chatLog.Post(messageLink);
        }
        // Destroy
        Destroy(this);
    }
}
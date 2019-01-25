using UnityEngine;
using System.Collections;

public class HeroStatsUI : MonoBehaviour
{
    public Vector2 pos = new Vector2(0, 0);
    public Vector2 size = new Vector2(60, 10);
    public Texture2D healthBarEmpty;
    public Texture2D healthBarFull;
    public Texture2D manaBarEmpty;
    public Texture2D manaBarFull;

    UnitStats stats;
	void Start()
    {
        stats = GetComponent<UnitStats>();
	}
    void OnGUI()
    {
        // Health
        GUI.BeginGroup(new Rect(pos, size));
            GUI.DrawTexture(new Rect(0, 0, size.x, size.y), healthBarEmpty);
            GUI.DrawTexture(new Rect(0, 0, size.x * (stats.health / stats.healthMax), size.y), healthBarFull);
        GUI.EndGroup();

        // Mana
        GUI.BeginGroup(new Rect(new Vector2(pos.x, pos.y + size.y + 10), size));
            GUI.DrawTexture(new Rect(0, 0, size.x, size.y), manaBarEmpty);
            GUI.DrawTexture(new Rect(0, 0, size.x * (stats.mana / stats.manaMax), size.y), manaBarFull);
        GUI.EndGroup();

        // Shields (over health)
        GUI.BeginGroup(new Rect(pos, size));
            GUI.DrawTexture(new Rect(0, 0, size.x * (stats.shields / stats.shieldsMax), size.y), manaBarFull);
        GUI.EndGroup();
    }
}

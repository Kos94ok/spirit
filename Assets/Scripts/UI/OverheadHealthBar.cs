using UnityEngine;
using System.Collections;

public class OverheadHealthBar : MonoBehaviour
{
    UnitStats Stats;

    public Vector2 pos = new Vector2(0, 0);
    public Vector2 size = new Vector2(60, 10);
    public Texture2D progressBarEmpty;
    public Texture2D progressBarFull;
    Vector2 offset;

    void Start()
    {
        Stats = GetComponent<UnitStats>();
        offset = pos;
    }
    void OnGUI()
    {
//        if (Stats.Health < Stats.HealthMax && Stats.Health > 0.00f)
//        {
//            GUI.BeginGroup(new Rect(pos - size / 2 - offset, size));
//
//            GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.50f);
//            GUI.DrawTexture(new Rect(0, 0, size.x, size.y), progressBarEmpty);
//            GUI.DrawTexture(new Rect(0, 0, size.x * (Stats.Health / Stats.HealthMax), size.y), progressBarFull);
//
//            GUI.EndGroup();
//        }
    } 
 
    void Update()
    {
        if (Stats.Health < Stats.HealthMax && Stats.Health > 0.00f)
        {
            pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y;
        }
    }
}

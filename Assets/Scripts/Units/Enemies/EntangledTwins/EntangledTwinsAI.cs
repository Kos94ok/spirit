using UnityEngine;
using System.Collections.Generic;
using Units.Enemies;

[RequireComponent(typeof(LineRenderer))]
public class EntangledTwinsAI : EnemyAI
{
    static float connectRange = 5f;
    static float disconnectRange = 6f;

    int twinId;
    static int twinCounter = 0;

    public Material lineMaterial;

    List<GameObject> twinsInRange = new List<GameObject>();
    Dictionary<GameObject, LineRenderer> weaponLines = new Dictionary<GameObject, LineRenderer>();

    void Start()
    {
        twinId = twinCounter++;
    }
	
	void Update()
    {
        // Check for new twins in range
        Collider[] detectedObjects;
        detectedObjects = Physics.OverlapSphere(transform.position, connectRange, 1);
        foreach (Collider collider in detectedObjects)
        {
            // Another twin found
            if (collider.gameObject != gameObject && collider.GetComponent<EntangledTwinsAI>() != null
                && !twinsInRange.Contains(collider.gameObject))
            {
                twinsInRange.Add(collider.gameObject);

                GameObject weaponHandle = new GameObject();
                LineRenderer weapon = weaponHandle.AddComponent<LineRenderer>();
                weapon.SetVertexCount(2);
                weapon.material = lineMaterial;
                weapon.SetWidth(0.1f, 0.25f);
                weapon.enabled = true;
                weaponLines.Add(collider.gameObject, weapon);
            }
        }

        for (int i = 0; i < twinsInRange.Count; i++)
        {
            if (Math.GetDistance2D(transform.position, twinsInRange[i].transform.position) < disconnectRange)
            {
                weaponLines[twinsInRange[i]].SetPosition(0, transform.position);
                weaponLines[twinsInRange[i]].SetPosition(1, twinsInRange[i].transform.position);
            }
            else
            {
                Destroy(weaponLines[twinsInRange[i]].gameObject);
                weaponLines.Remove(twinsInRange[i]);
                twinsInRange.RemoveAt(i);
                i -= 1;
            }
        }
        
    }

    public bool IsMinorTo(int id)
    {
        return twinId > id;
    }
}

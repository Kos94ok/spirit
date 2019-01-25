using UnityEngine;
using System.Collections;

public class PlayerLightFollow : MonoBehaviour {

    public float Height;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject Parent = GameObject.FindGameObjectWithTag("Player");
        Vector3 Delta = new Vector3(Parent.transform.position.x - transform.position.x, (Parent.transform.position.y + Height) - transform.position.y, Parent.transform.position.z - transform.position.z);
        transform.Translate(Delta, Space.World);
	}
}

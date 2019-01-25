using UnityEngine;
using System.Collections;

public class TimedLife : MonoBehaviour {

    public float Timer = 1.00f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Timer -= Time.deltaTime;
        if (Timer <= 0.00f)
        {
            Destroy(gameObject);
        }
	}
}

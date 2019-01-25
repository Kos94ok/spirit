using UnityEngine;
using System.Collections;

public class BasicFollow : MonoBehaviour
{
    private GameObject Target;
	public void Follow(GameObject Target)
    {
        this.Target = Target;
    }
    void Start()
    {
    }
	void Update()
    {
        if (Target != null && Target.transform != null)
        {
            transform.position = Target.transform.position;
        }
        else
        {
            Destroy(GetComponent<BasicFollow>());
        }
	}
}

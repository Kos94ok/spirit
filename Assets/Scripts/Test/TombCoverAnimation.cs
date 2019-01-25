using UnityEngine;
using System.Collections;

public class TombCoverAnimation : MonoBehaviour
{
	void Start()
    {
	
	}
	
	void Update()
    {
	    if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            GetComponent<Animator>().SetBool("Opening", true);
        }
	}
}

using UnityEngine;
using System.Collections;

public class ProbablyWorkingPauseButton : MonoBehaviour
{
	void Update()
    {
	    if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Break();
        }
	}
}

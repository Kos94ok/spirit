using UnityEngine;
using System.Collections;

public class PlayerLightChargeFollow : MonoBehaviour
{
    GameObject PlayerVisualData;
	void Start()
    {
        PlayerVisualData = GameObject.FindGameObjectWithTag("PlayerVisualData");
	}
	void Update()
    {
        transform.position = PlayerVisualData.transform.position;
	}
}
